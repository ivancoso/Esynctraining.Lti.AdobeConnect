using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Esynctraining.Zoom.ApiWrapper.Model;
using RestSharp;
using RestSharp.Authenticators;
using RestSharp.Serializers;

namespace Esynctraining.Zoom.ApiWrapper
{
    public class ZoomApiWrapper
    {
        private readonly IZoomAuthParamsAccessor _authParamsAccessor;
        private RestClient _webClient;

        private async Task<RestClient> GetWebClient()
        {
            return _webClient ?? (_webClient = new RestClient(await _authParamsAccessor.GetApiUrl()));
        }

        public ZoomApiWrapper(IZoomAuthParamsAccessor authParamsAccessor)
        {
            _authParamsAccessor = authParamsAccessor;
        }

        public async Task<UserInfo> GetUser(string idOrEmail)
        {
            RestRequest restRequest = await BuildRequestAuthorization("/users/{idOrEmail}", Method.GET);
            restRequest.AddParameter(nameof(idOrEmail), (object)idOrEmail, ParameterType.UrlSegment);
            IRestResponse<UserInfo> restResponse = await (await GetWebClient()).ExecuteTaskAsync<UserInfo>((IRestRequest)restRequest);

            if (restResponse.ResponseStatus == ResponseStatus.Completed && restResponse.StatusCode == HttpStatusCode.OK)
                return restResponse.Data;

            throw new ZoomApiException
            {
                Content = restResponse.Content,
                ErrorMessage = restResponse.ErrorMessage,
                StatusDescription = restResponse.StatusDescription
            };
        }

        public async Task<UserInfo> GetCurrentUser()
        {
            RestRequest restRequest = await BuildRequestAuthorization("/users/me", Method.GET);
            IRestResponse<UserInfo> restResponse = await (await GetWebClient()).ExecuteTaskAsync<UserInfo>(restRequest);

            if (restResponse.ResponseStatus == ResponseStatus.Completed && restResponse.StatusCode == HttpStatusCode.OK)
                return restResponse.Data;

            throw new ZoomApiException
            {
                Content = restResponse.Content,
                ErrorMessage = restResponse.ErrorMessage,
                StatusDescription = restResponse.StatusDescription,
                StatusCode = restResponse.StatusCode
            };
        }

        public async Task<User> CreateUser(CreateUser createUser, string action)
        {
            List<string> stringList = createUser.Validate();
            if (stringList.Count > 0)
                throw new Exception(string.Format("CreateUser request does not pass validation. {0}", (object)string.Join(" :: ", (IEnumerable<string>)stringList)));
            if (!action.Equals(CreateUserAction.Create, StringComparison.InvariantCultureIgnoreCase) && !action.Equals(CreateUserAction.AutoCreate, StringComparison.InvariantCultureIgnoreCase) && (!action.Equals(CreateUserAction.CustCreate, StringComparison.InvariantCultureIgnoreCase) && !action.Equals(CreateUserAction.SsoCreate, StringComparison.InvariantCultureIgnoreCase)))
                throw new Exception(string.Format("CreateUser action allowed values are [{0},{1},{2},{3}]", new object[4]
                {
          (object) CreateUserAction.Create,
          (object) CreateUserAction.AutoCreate,
          (object) CreateUserAction.CustCreate,
          (object) CreateUserAction.SsoCreate
                }));
            if (string.IsNullOrWhiteSpace(createUser.Password) && !string.IsNullOrWhiteSpace(action) && action.Equals(CreateUserAction.AutoCreate, StringComparison.InvariantCultureIgnoreCase))
                throw new Exception(string.Format("{0} property is required for creating user when action is set to {1}", (object)"Password", (object)CreateUserAction.AutoCreate));
            RestRequest restRequest = await BuildRequestAuthorization("users", Method.POST);
            restRequest.AddJsonBody((object)new
            {
                action = action,
                user_info = createUser
            });
            IRestResponse<User> restResponse = await (await GetWebClient()).ExecuteTaskAsync<User>((IRestRequest)restRequest);
            if (restResponse.ResponseStatus == ResponseStatus.Completed && restResponse.StatusCode == HttpStatusCode.Created)
                return restResponse.Data;
            throw new ZoomApiException
            {
                Content = restResponse.Content,
                ErrorMessage = restResponse.ErrorMessage,
                StatusDescription = restResponse.StatusDescription
            };
        }

        public async Task<ListUsers> GetUsers(UserStatus status = UserStatus.Active, int pageSize = 30, int pageNumber = 1)
        {
            if (pageSize > 300)
                throw new Exception("GetUsers page size max 300");
            RestRequest restRequest = await BuildRequestAuthorization("users", Method.GET);
            restRequest.AddParameter(nameof(status), (object)status.ToString().ToLowerInvariant(), ParameterType.QueryString);
            restRequest.AddParameter("page_size", (object)pageSize, ParameterType.QueryString);
            restRequest.AddParameter("page_number", (object)pageNumber, ParameterType.QueryString);
            IRestResponse<ListUsers> restResponse = await (await GetWebClient()).ExecuteTaskAsync<ListUsers>((IRestRequest)restRequest);

            if (restResponse.ResponseStatus == ResponseStatus.Completed && restResponse.StatusCode == HttpStatusCode.OK)
                return restResponse.Data;

            throw new ZoomApiException
            {
                Content = restResponse.Content,
                ErrorMessage = restResponse.ErrorMessage,
                StatusDescription = restResponse.StatusDescription
            };
        }

        public async Task<ZoomApiResultWithData<Meeting>> GetMeeting(string meetingId)
        {
            RestRequest restRequest = await BuildRequestAuthorization("meetings/{meetingId}", Method.GET);
            restRequest.AddParameter(nameof(meetingId), meetingId, ParameterType.UrlSegment);
            IRestResponse<Meeting> restResponse = await (await GetWebClient()).ExecuteGetTaskAsync<Meeting>(restRequest);

            if (restResponse.ResponseStatus == ResponseStatus.Completed && restResponse.StatusCode == HttpStatusCode.OK)
                return restResponse.Data.ToSuccessZoomApiResult();

            return await HandleErrorRequest(restResponse);
        }

        private async Task<ZoomApiResultWithData<T>> HandleErrorRequest<T>(IRestResponse<T> restResponse) where T : class
        {
            //todo: log
            if (!string.IsNullOrWhiteSpace(restResponse.Content))
                return ZoomApiResultWithData<T>.ApiError(restResponse.Content);

            throw new ZoomApiException
            {
                Content = restResponse.Content,
                ErrorMessage = restResponse.ErrorMessage,
                StatusDescription = restResponse.StatusDescription
            };
        }

        public async Task<ZoomApiResultWithData<ListMeetings>> GetMeetings(string userId, MeetingListTypes type = MeetingListTypes.Scheduled, int pageSize = 30, int pageNumber = 1)
        {
            if (pageSize > 300)
                throw new Exception("GetMeetings page size max 300");
            RestRequest restRequest = await BuildRequestAuthorization("users/{userId}/meetings", Method.GET);
            restRequest.AddParameter(nameof(userId), (object)userId, ParameterType.UrlSegment);
            restRequest.AddParameter(nameof(type), (object)type.ToString().ToLowerInvariant(), ParameterType.QueryString);
            restRequest.AddParameter("page_size", (object)pageSize, ParameterType.QueryString);
            restRequest.AddParameter("page_number", (object)pageNumber, ParameterType.QueryString);

            IRestResponse<ListMeetings> restResponse = await (await GetWebClient()).ExecuteTaskAsync<ListMeetings>((IRestRequest)restRequest);
            if (restResponse.ResponseStatus == ResponseStatus.Completed && restResponse.StatusCode == HttpStatusCode.OK)
                return restResponse.Data.ToSuccessZoomApiResult();

            if (restResponse.ResponseStatus == ResponseStatus.Completed && restResponse.StatusCode == HttpStatusCode.NotFound)
            {
                return ZoomApiResultWithData<ListMeetings>.ApiError(restResponse.Content);
            }
                

            if (!string.IsNullOrWhiteSpace(restResponse.ErrorMessage))
                return ZoomApiResultWithData<ListMeetings>.Error(restResponse.ErrorMessage);

            if (!string.IsNullOrWhiteSpace(restResponse.StatusDescription) && !string.IsNullOrWhiteSpace(restResponse.Content))
                return ZoomApiResultWithData<ListMeetings>.Error(string.Format("{0} || {1}", (object)restResponse.StatusDescription, (object)restResponse.Content));

            return ZoomApiResultWithData<ListMeetings>.Error($"Not found meetings for user {userId}");
        }

        public async Task<ZoomListRegistrants> GetMeetingRegistrants(string meetingId, string occurrenceId = null, string status="approved", int pageSize = 300, int pageNumber = 1)
        {
            if (pageSize > 300)
                throw new Exception("GetMeetingRegistrants page size max 300");
            RestRequest restRequest = await BuildRequestAuthorization("/meetings/{meetingId}/registrants", Method.GET);
            restRequest.AddParameter(nameof(meetingId), (object)meetingId, ParameterType.UrlSegment);
            if (!string.IsNullOrEmpty(occurrenceId))
            {
                restRequest.AddParameter(nameof(occurrenceId), occurrenceId, ParameterType.QueryString);
            }
            restRequest.AddParameter("page_size", (object)pageSize, ParameterType.QueryString);
            restRequest.AddParameter("page_number", (object)pageNumber, ParameterType.QueryString);
            restRequest.AddParameter("status", (object)status, ParameterType.QueryString);
            IRestResponse<ZoomListRegistrants> restResponse = await (await GetWebClient()).ExecuteTaskAsync<ZoomListRegistrants>((IRestRequest)restRequest);
            if (restResponse.ResponseStatus == ResponseStatus.Completed && restResponse.StatusCode == HttpStatusCode.OK)
                return restResponse.Data;
            if (!string.IsNullOrWhiteSpace(restResponse.ErrorMessage))
                throw new Exception(restResponse.ErrorMessage);
            if (!string.IsNullOrWhiteSpace(restResponse.StatusDescription) && !string.IsNullOrWhiteSpace(restResponse.Content))
                throw new Exception(string.Format("{0} || {1}", (object)restResponse.StatusDescription, (object)restResponse.Content));
            return (ZoomListRegistrants)null;
        }

        public async Task<ZoomApiResultWithData<ZoomAddRegistrantResponse>> AddRegistrant(string meetingId, ZoomAddRegistrantRequest registrant, string occurenceIds = null)
        {
            RestRequest restRequest = await BuildRequestAuthorization("meetings/{meetingId}/registrants", Method.POST);
            restRequest.AddParameter(nameof(meetingId), (object)meetingId, ParameterType.UrlSegment);
            restRequest.AddJsonBody((object)registrant);
            IRestResponse<ZoomAddRegistrantResponse> restResponse = await (await GetWebClient()).ExecuteTaskAsync<ZoomAddRegistrantResponse>((IRestRequest)restRequest);
            if (restResponse.ResponseStatus == ResponseStatus.Completed && restResponse.StatusCode == HttpStatusCode.Created)
                return restResponse.Data.ToSuccessZoomApiResult();

            if (restResponse.ResponseStatus == ResponseStatus.Completed && restResponse.StatusCode == HttpStatusCode.BadRequest)
                return ZoomApiResultWithData<ZoomAddRegistrantResponse>.ApiError(restResponse.Content); ;

            if (!string.IsNullOrWhiteSpace(restResponse.ErrorMessage))
                throw new Exception(restResponse.ErrorMessage);
            if (!string.IsNullOrWhiteSpace(restResponse.StatusDescription) && !string.IsNullOrWhiteSpace(restResponse.Content))
                throw new Exception(string.Format("{0} || {1}", (object)restResponse.StatusDescription, (object)restResponse.Content));
            return null;
        }

        /*
         * 
         * 
         occurrence_ids
optional
Occurrence IDs, could get this value from Meeting Get API. Multiple value separated by comma.
             */
        //public Meeting CreateMeeting(string userId, Meeting meeting)
        //{
        //    RestRequest restRequest = await BuildRequestAuthorization("users/{userId}/meetings", Method.POST);
        //    restRequest.AddParameter(nameof(userId), (object)userId, ParameterType.UrlSegment);
        //    restRequest.AddJsonBody((object)meeting);
        //    IRestResponse<Meeting> restResponse = await (await GetWebClient()).ExecuteTaskAsync<Meeting>((IRestRequest)restRequest);
        //    if (restResponse.ResponseStatus == ResponseStatus.Completed && restResponse.StatusCode == HttpStatusCode.Created)
        //        return restResponse.Data;
        //    if (!string.IsNullOrWhiteSpace(restResponse.ErrorMessage))
        //        throw new Exception(restResponse.ErrorMessage);
        //    if (!string.IsNullOrWhiteSpace(restResponse.StatusDescription) && !string.IsNullOrWhiteSpace(restResponse.Content))
        //        throw new Exception(string.Format("{0} || {1}", (object)restResponse.StatusDescription, (object)restResponse.Content));
        //    return (Meeting)null;
        //}

        public async Task<bool> UpdateRegistrantsStatus(string meetingId, ZoomUpdateRegistrantStatusRequest updateStatusRequest, string occurenceId)
        {
            RestRequest restRequest = await BuildRequestAuthorization("meetings/{meetingId}/registrants/status", Method.PUT);
            restRequest.AddParameter(nameof(meetingId), (object)meetingId, ParameterType.UrlSegment);
            restRequest.AddJsonBody((object)updateStatusRequest);
            IRestResponse restResponse = await (await GetWebClient()).ExecuteTaskAsync((IRestRequest)restRequest);
            if (restResponse.ResponseStatus == ResponseStatus.Completed && restResponse.StatusCode == HttpStatusCode.NoContent)
                return true;
            if (!string.IsNullOrWhiteSpace(restResponse.ErrorMessage))
                throw new Exception(restResponse.ErrorMessage);
            if (!string.IsNullOrWhiteSpace(restResponse.StatusDescription) && !string.IsNullOrWhiteSpace(restResponse.Content))
                throw new Exception(string.Format("{0} || {1}", (object)restResponse.StatusDescription, (object)restResponse.Content));
            return false;
        }

        public async Task<bool> DeleteMeeting(string meetingId, string occurrenceId = null)
        {
            RestRequest restRequest = await BuildRequestAuthorization("meetings/{meetingId}", Method.DELETE);
            restRequest.AddParameter(nameof(meetingId), (object)meetingId, ParameterType.UrlSegment);
            if (!string.IsNullOrWhiteSpace(occurrenceId))
                restRequest.AddParameter("occurrence_id", (object)occurrenceId, ParameterType.QueryString);
            IRestResponse restResponse = await (await GetWebClient()).ExecuteTaskAsync((IRestRequest)restRequest);
            if (restResponse.ResponseStatus == ResponseStatus.Completed && restResponse.StatusCode == HttpStatusCode.NoContent)
                return true;
            if (!string.IsNullOrWhiteSpace(restResponse.ErrorMessage))
                throw new Exception(restResponse.ErrorMessage);
            if (!string.IsNullOrWhiteSpace(restResponse.StatusDescription) && !string.IsNullOrWhiteSpace(restResponse.Content))
                throw new Exception(string.Format("{0} || {1}", (object)restResponse.StatusDescription, (object)restResponse.Content));
            return false;
        }

        public async Task<ZoomApiRecordingList> GetRecordings(string meetingId, bool trash = false)
        {
            RestRequest restRequest = await BuildRequestAuthorization("meetings/{meetingId}/recordings", Method.GET);
            restRequest.AddParameter(nameof(meetingId), meetingId, ParameterType.UrlSegment);
            restRequest.AddParameter(nameof(trash), trash, ParameterType.QueryString);
            IRestResponse<ZoomApiRecordingList> restResponse = await (await GetWebClient()).ExecuteTaskAsync<ZoomApiRecordingList>((IRestRequest)restRequest);
            if (restResponse.ResponseStatus == ResponseStatus.Completed && restResponse.StatusCode == HttpStatusCode.OK)
                return restResponse.Data;
            if(restResponse.StatusCode == HttpStatusCode.NotFound)
                return new ZoomApiRecordingList{RecordingFiles = new List<ZoomRecordingFile>()};
            if (!string.IsNullOrWhiteSpace(restResponse.ErrorMessage))
                throw new Exception(restResponse.ErrorMessage);
            if (!string.IsNullOrWhiteSpace(restResponse.StatusDescription) && !string.IsNullOrWhiteSpace(restResponse.Content))
                throw new Exception(string.Format("{0} || {1}", (object)restResponse.StatusDescription, (object)restResponse.Content));
            return (ZoomApiRecordingList)null;
        }

        public async Task<ZoomRecordingSessionList> GetUserRecordings(string userId, DateTime? from = null, DateTime? to = null, int pageSize = 300, string nextPageToken = null, bool mc = false, bool trash = false)
        {
            RestRequest restRequest = await BuildRequestAuthorization($"users/{userId}/recordings", Method.GET);
            restRequest.AddParameter("page_size", pageSize, ParameterType.QueryString);
            if(from.HasValue)
                restRequest.AddParameter(nameof(from), from.Value.ToString("yyyy-MM-dd"), ParameterType.QueryString);
            if(to.HasValue)
                restRequest.AddParameter(nameof(to), to.Value.ToString("yyyy-MM-dd"), ParameterType.QueryString);
            if (!string.IsNullOrWhiteSpace(nextPageToken))
                restRequest.AddParameter("next_page_token", (object)nextPageToken, ParameterType.QueryString);
            restRequest.AddParameter(nameof(mc), mc.ToString().ToLower(), ParameterType.QueryString);
            restRequest.AddParameter(nameof(trash), trash.ToString().ToLower(), ParameterType.QueryString);
            IRestResponse<ZoomRecordingSessionList> restResponse = await (await GetWebClient()).ExecuteTaskAsync<ZoomRecordingSessionList>((IRestRequest)restRequest);
            if (restResponse.ResponseStatus == ResponseStatus.Completed && restResponse.StatusCode == HttpStatusCode.OK)
                return restResponse.Data;
            if (restResponse.StatusCode == HttpStatusCode.NotFound)
                return new ZoomRecordingSessionList { Meetings = new List<ZoomRecordingSession>() };
            if (!string.IsNullOrWhiteSpace(restResponse.ErrorMessage))
                throw new Exception(restResponse.ErrorMessage);
            if (!string.IsNullOrWhiteSpace(restResponse.StatusDescription) && !string.IsNullOrWhiteSpace(restResponse.Content))
                throw new Exception(string.Format("{0} || {1}", (object)restResponse.StatusDescription, (object)restResponse.Content));
            return (ZoomRecordingSessionList)null;
        }

        public async Task<bool> DeleteRecording(string meetingId, string recordingId = null, bool trash = true)
        {
            RestRequest restRequest = await BuildRequestAuthorization($"meetings/{meetingId}/recordings/{recordingId}?action={(object)(trash ? "trash" : "delete")}", Method.DELETE);

            //restRequest.AddParameter(nameof(meetingId), (object)meetingId, ParameterType.UrlSegment);

            //if (!string.IsNullOrWhiteSpace(recordingId))
            //    restRequest.AddParameter(nameof(recordingId), (object)recordingId, ParameterType.UrlSegment);

            //restRequest.AddParameter("action", (object)(trash?"trash":"delete"), ParameterType.QueryString);
            IRestResponse restResponse = await (await GetWebClient()).ExecuteTaskAsync((IRestRequest)restRequest);
            if (restResponse.ResponseStatus == ResponseStatus.Completed && restResponse.StatusCode == HttpStatusCode.NoContent)
                return true;

            if (!string.IsNullOrWhiteSpace(restResponse.ErrorMessage))
                throw new Exception(restResponse.ErrorMessage);

            if (!string.IsNullOrWhiteSpace(restResponse.StatusDescription) && !string.IsNullOrWhiteSpace(restResponse.Content))
                throw new Exception(string.Format("{0} || {1}", (object)restResponse.StatusDescription, (object)restResponse.Content));

            return false;
        }
        public async Task<bool> RecoverRecording(string meetingId, string recordingId = null)
        {
            RestRequest restRequest = await BuildRequestAuthorization($"meetings/{meetingId}/recordings/{(recordingId == null? "" : recordingId + "/")}status", Method.PUT);
            restRequest.AddJsonBody(new {action = "recover"});
            IRestResponse restResponse = await (await GetWebClient()).ExecuteTaskAsync((IRestRequest)restRequest);
            if (restResponse.ResponseStatus == ResponseStatus.Completed && restResponse.StatusCode == HttpStatusCode.NoContent)
                return true;
            if (!string.IsNullOrWhiteSpace(restResponse.ErrorMessage))
                throw new Exception(restResponse.ErrorMessage);
            if (!string.IsNullOrWhiteSpace(restResponse.StatusDescription) && !string.IsNullOrWhiteSpace(restResponse.Content))
                throw new Exception(string.Format("{0} || {1}", (object)restResponse.StatusDescription, (object)restResponse.Content));
            return false;
        }

        public class ZoomToken
        {
            public string Token { get; set; }
        }

        public async Task<string> GetUserToken(string userId, string type)
        {
            RestRequest restRequest = await BuildRequestAuthorization("users/{userId}/token", Method.GET);
            restRequest.AddParameter(nameof(userId), (object)userId, ParameterType.UrlSegment);
            restRequest.AddParameter("type", type, ParameterType.QueryString);
            IRestResponse<ZoomToken> restResponse = await (await GetWebClient()).ExecuteTaskAsync<ZoomToken>((IRestRequest)restRequest);
            return restResponse.Data.Token;
        }

        public async Task<ZoomApiResultWithData<Meeting>> CreateMeeting(string userId, Meeting meeting)
        {
            RestRequest restRequest = await BuildRequestAuthorization("users/{userId}/meetings", Method.POST);
            restRequest.AddParameter(nameof(userId), (object)userId, ParameterType.UrlSegment);
            restRequest.AddJsonBody((object)meeting);
            IRestResponse<Meeting> restResponse = await (await GetWebClient()).ExecuteTaskAsync<Meeting>((IRestRequest)restRequest);
            if (restResponse.ResponseStatus == ResponseStatus.Completed && restResponse.StatusCode == HttpStatusCode.Created)
                return restResponse.Data.ToSuccessZoomApiResult();

            if (restResponse.ResponseStatus == ResponseStatus.Completed 
                && (restResponse.StatusCode == HttpStatusCode.NotFound || restResponse.StatusCode == HttpStatusCode.BadRequest))
            {
                return ZoomApiResultWithData<Meeting>.ApiError(restResponse.Content);
            }

            if (!string.IsNullOrWhiteSpace(restResponse.ErrorMessage))
                throw new Exception(restResponse.ErrorMessage);

            if (!string.IsNullOrWhiteSpace(restResponse.StatusDescription) && !string.IsNullOrWhiteSpace(restResponse.Content))
                throw new Exception(string.Format("{0} || {1}", (object)restResponse.StatusDescription, (object)restResponse.Content));

            return ZoomApiResultWithData<Meeting>.Error($"Faild with creating meeting {userId}");
        }

        public async Task<ZoomApiResultWithData<bool>> UpdateMeeting(string meetingId, Meeting meeting)
        {
            //HACK: ZOOM API does not update AGENDA field with empty value. I  requested zoom support
            //https://devforum.zoom.us/t/update-agenda-field-in-meeting/2800
            if (string.IsNullOrEmpty(meeting.Agenda))
                meeting.Agenda = " ";

            RestRequest restRequest = await BuildRequestAuthorization("meetings/{meetingId}", Method.PATCH);
            restRequest.AddParameter(nameof(meetingId), (object)meetingId, ParameterType.UrlSegment);
            restRequest.AddJsonBody((object)meeting);
            IRestResponse restResponse = await (await GetWebClient()).ExecuteTaskAsync((IRestRequest)restRequest);
            if (restResponse.ResponseStatus == ResponseStatus.Completed && restResponse.StatusCode == HttpStatusCode.NoContent)
                return true.ToSuccessZoomApiResult();

            if (restResponse.ResponseStatus == ResponseStatus.Completed && restResponse.StatusCode == HttpStatusCode.BadRequest)
                return ZoomApiResultWithData<bool>.ApiError(restResponse.Content);

            return ZoomApiResultWithData<bool>.Error($"Faild during update meeting {meetingId}");
        }

        public async Task<ZoomMeetingsReportList> GetMeetingsReport(string userId, DateTime from, DateTime to, int pageSize = 300, string nextPageToken = null)
        {
            if (pageSize > 300)
                throw new Exception("GetMeetingParticipantsReport page size max 300");
            RestRequest restRequest = await BuildRequestAuthorization("report/users/{userId}/meetings", Method.GET);
            restRequest.AddParameter(nameof(userId), (object)userId, ParameterType.UrlSegment);
            restRequest.AddParameter("page_size", (object)pageSize, ParameterType.QueryString);
            restRequest.AddParameter("from", (object)from.ToString("yyyy-MM-dd"), ParameterType.QueryString);
            restRequest.AddParameter("to", (object)to.ToString("yyyy-MM-dd"), ParameterType.QueryString);
            if (!string.IsNullOrWhiteSpace(nextPageToken))
                restRequest.AddParameter("next_page_token", (object)nextPageToken, ParameterType.QueryString);
            IRestResponse<ZoomMeetingsReportList> restResponse = await (await GetWebClient()).ExecuteTaskAsync<ZoomMeetingsReportList>((IRestRequest)restRequest);
            if (restResponse.ResponseStatus == ResponseStatus.Completed && restResponse.StatusCode == HttpStatusCode.OK)
                return restResponse.Data;
            if (!string.IsNullOrWhiteSpace(restResponse.ErrorMessage))
                throw new Exception(restResponse.ErrorMessage);
             if (!string.IsNullOrWhiteSpace(restResponse.StatusDescription) && !string.IsNullOrWhiteSpace(restResponse.Content))
                throw new Exception(string.Format("{0} || {1}", (object)restResponse.StatusDescription, (object)restResponse.Content));
            return (ZoomMeetingsReportList)null;
        }

        public async Task<ZoomMeetingPoolsReport> GetZoomMeetingPoolsReport(string meetingId)
        {
            RestRequest restRequest = await BuildRequestAuthorization("report/meetings/{meetingId}/polls", Method.GET);
            restRequest.AddParameter(nameof(meetingId), (object)meetingId, ParameterType.UrlSegment);
            var restResponse = await (await GetWebClient()).ExecuteTaskAsync<ZoomMeetingPoolsReport>((IRestRequest)restRequest);

            if (restResponse.ResponseStatus == ResponseStatus.Completed && restResponse.StatusCode == HttpStatusCode.OK)
                return restResponse.Data;

            if (!string.IsNullOrWhiteSpace(restResponse.ErrorMessage))
                throw new Exception(restResponse.ErrorMessage);

            return (ZoomMeetingPoolsReport)null;
        }

        public async Task<MeetingParticipantsReport> GetMeetingParticipantsReport(string meetingId, int pageSize = 300, string nextPageToken = null)
        {
            meetingId = WebUtility.UrlEncode(meetingId);
            if (pageSize > 300)
                throw new Exception("GetMeetingParticipantsReport page size max 300");
            RestRequest restRequest = await BuildRequestAuthorization("report/meetings/{meetingId}/participants", Method.GET);
            restRequest.AddParameter(nameof(meetingId), (object)meetingId, ParameterType.UrlSegment);
            restRequest.AddParameter("page_size", (object)pageSize, ParameterType.QueryString);
            if (!string.IsNullOrWhiteSpace(nextPageToken))
                restRequest.AddParameter("next_page_token", (object)nextPageToken, ParameterType.QueryString);
            IRestResponse<MeetingParticipantsReport> restResponse = await (await GetWebClient()).ExecuteTaskAsync<MeetingParticipantsReport>((IRestRequest)restRequest);
            if (restResponse.ResponseStatus == ResponseStatus.Completed && restResponse.StatusCode == HttpStatusCode.OK)
                return restResponse.Data;
            if (!string.IsNullOrWhiteSpace(restResponse.ErrorMessage))
                throw new Exception(restResponse.ErrorMessage);
            if (!string.IsNullOrWhiteSpace(restResponse.StatusDescription) && !string.IsNullOrWhiteSpace(restResponse.Content))
                throw new Exception(string.Format("{0} || {1}", (object)restResponse.StatusDescription, (object)restResponse.Content));
            return (MeetingParticipantsReport)null;
        }

        public async Task<ListMeetingParticipantsDetails> GetMeetingParticipantsDetails(string meetingId, int pageSize = 300, string nextPageToken = null)
        {
            if (pageSize > 300)
                throw new Exception("GetMeetingParticipantsReport page size max 300");
            RestRequest restRequest = await BuildRequestAuthorization($"metrics/meetings/{meetingId}/participants", Method.GET);
            restRequest.AddParameter("page_size", (object)pageSize, ParameterType.QueryString);
            restRequest.AddParameter("type", "past", ParameterType.QueryString);
            if (!string.IsNullOrWhiteSpace(nextPageToken))
                restRequest.AddParameter("next_page_token", (object)nextPageToken, ParameterType.QueryString);
            IRestResponse<ListMeetingParticipantsDetails> restResponse = await (await GetWebClient()).ExecuteTaskAsync<ListMeetingParticipantsDetails>((IRestRequest)restRequest);
            if (restResponse.ResponseStatus == ResponseStatus.Completed && restResponse.StatusCode == HttpStatusCode.OK)
                return restResponse.Data;
            if (!string.IsNullOrWhiteSpace(restResponse.ErrorMessage))
                throw new Exception(restResponse.ErrorMessage);
            if (!string.IsNullOrWhiteSpace(restResponse.StatusDescription) && !string.IsNullOrWhiteSpace(restResponse.Content))
                throw new Exception(string.Format("{0} || {1}", (object)restResponse.StatusDescription, (object)restResponse.Content));
            return (ListMeetingParticipantsDetails)null;
        }

        public async Task<OAuthTokenInfo> RefreshOauthToken(string refreshToken, string redirectUrl, string clientId, string sclientSecret)
        {
            RestRequest restRequest = new RestRequest($"/token?grant_type=refresh_token&&refresh_token={refreshToken}&&redirect_uri={redirectUrl}", Method.POST);
            var webClient = new RestClient("https://zoom.us/oauth");

            var token = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes($"{clientId}:{sclientSecret}"));
            restRequest.AddHeader("Accept", "application/json");
            restRequest.AddHeader("Authorization", $"Basic {token}");

            IRestResponse<OAuthTokenInfo> restResponse = await webClient.ExecuteTaskAsync<OAuthTokenInfo>(restRequest);
            if (restResponse.ResponseStatus == ResponseStatus.Completed && restResponse.StatusCode == HttpStatusCode.OK)
                return restResponse.Data;

            if (!string.IsNullOrWhiteSpace(restResponse.ErrorMessage))
                throw new Exception(restResponse.ErrorMessage);

            return (OAuthTokenInfo)null;
        }

        private async Task<RestRequest> BuildRequestAuthorization(string resource, Method method)
        {
            //return this.WebClient.BuildRequestAuthorization(this.Options, resource, method);

            RestRequest restRequest = new RestRequest(resource, method);

            var token = await _authParamsAccessor.GetAuthToken();
            var webClient = await GetWebClient();
            webClient.Authenticator = (IAuthenticator)new JwtAuthenticator(token);
            NewtonsoftJsonSerializer newtonsoftJsonSerializer = new NewtonsoftJsonSerializer();
            restRequest.JsonSerializer = (ISerializer)newtonsoftJsonSerializer;
            return restRequest;
        }
    }
}