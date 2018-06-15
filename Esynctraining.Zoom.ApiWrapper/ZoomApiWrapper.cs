using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Text;
using Esynctraining.Zoom.ApiWrapper.Model;
using Microsoft.IdentityModel.Tokens;
using RestSharp;
using RestSharp.Authenticators;
using RestSharp.Serializers;

namespace Esynctraining.Zoom.ApiWrapper
{
    public class ZoomApiWrapper
    {
        private ZoomApiOptions Options { get; set; }

        private RestClient WebClient { get; set; }

        public ZoomApiWrapper(IZoomOptionsAccessor accessor)
        {
            Options = accessor.Options;
            if (string.IsNullOrWhiteSpace(Options.ZoomApiBaseUrl))
                Options.ZoomApiBaseUrl = "https://api.zoom.us/v2/";
            this.WebClient = new RestClient(Options.ZoomApiBaseUrl);
        }

        public ZoomApiWrapper(ZoomApiOptions options)
        {
            if (string.IsNullOrWhiteSpace(options.ZoomApiBaseUrl))
                options.ZoomApiBaseUrl = "https://api.zoom.us/v2/";
            this.WebClient = new RestClient(options.ZoomApiBaseUrl);
            this.Options = options;
        }

        public UserInfo GetUser(string idOrEmail)
        {
            RestRequest restRequest = this.BuildRequestAuthorization("users/{idOrEmail}", Method.GET);
            restRequest.AddParameter(nameof(idOrEmail), (object)idOrEmail, ParameterType.UrlSegment);
            IRestResponse<UserInfo> restResponse = this.WebClient.Execute<UserInfo>((IRestRequest)restRequest);
            if (restResponse.ResponseStatus == ResponseStatus.Completed && restResponse.StatusCode == HttpStatusCode.OK)
                return restResponse.Data;
            if (!string.IsNullOrWhiteSpace(restResponse.ErrorMessage))
                throw new Exception(restResponse.ErrorMessage);
            if (!string.IsNullOrWhiteSpace(restResponse.StatusDescription) && !string.IsNullOrWhiteSpace(restResponse.Content))
                throw new Exception(string.Format("{0} || {1}", (object)restResponse.StatusDescription, (object)restResponse.Content));
            return (UserInfo)null;
        }

        public User CreateUser(CreateUser createUser, string action)
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
            RestRequest restRequest = this.BuildRequestAuthorization("users", Method.POST);
            restRequest.AddJsonBody((object)new
            {
                action = action,
                user_info = createUser
            });
            IRestResponse<User> restResponse = this.WebClient.Execute<User>((IRestRequest)restRequest);
            if (restResponse.ResponseStatus == ResponseStatus.Completed && restResponse.StatusCode == HttpStatusCode.Created)
                return restResponse.Data;
            if (!string.IsNullOrWhiteSpace(restResponse.ErrorMessage))
                throw new Exception(restResponse.ErrorMessage);
            if (!string.IsNullOrWhiteSpace(restResponse.StatusDescription) && !string.IsNullOrWhiteSpace(restResponse.Content))
                throw new Exception(string.Format("{0} || {1}", (object)restResponse.StatusDescription, (object)restResponse.Content));
            return (User)null;
        }

        public ListUsers GetUsers(UserStatuses status = UserStatuses.Active, int pageSize = 30, int pageNumber = 1)
        {
            if (pageSize > 300)
                throw new Exception("GetUsers page size max 300");
            RestRequest restRequest = this.BuildRequestAuthorization("users", Method.GET);
            restRequest.AddParameter(nameof(status), (object)status.ToString().ToLowerInvariant(), ParameterType.QueryString);
            restRequest.AddParameter("page_size", (object)pageSize, ParameterType.QueryString);
            restRequest.AddParameter("page_number", (object)pageNumber, ParameterType.QueryString);
            IRestResponse<ListUsers> restResponse = this.WebClient.Execute<ListUsers>((IRestRequest)restRequest);
            if (restResponse.ResponseStatus == ResponseStatus.Completed && restResponse.StatusCode == HttpStatusCode.OK)
                return restResponse.Data;
            if (!string.IsNullOrWhiteSpace(restResponse.ErrorMessage))
                throw new Exception(restResponse.ErrorMessage);
            if (!string.IsNullOrWhiteSpace(restResponse.StatusDescription) && !string.IsNullOrWhiteSpace(restResponse.Content))
                throw new Exception(string.Format("{0} || {1}", (object)restResponse.StatusDescription, (object)restResponse.Content));
            return (ListUsers)null;
        }

        public Meeting GetMeeting(string meetingId)
        {
            RestRequest restRequest = this.BuildRequestAuthorization("meetings/{meetingId}", Method.GET);
            restRequest.AddParameter(nameof(meetingId), (object)meetingId, ParameterType.UrlSegment);
            IRestResponse<Meeting> restResponse = this.WebClient.Execute<Meeting>((IRestRequest)restRequest);
            if (restResponse.ResponseStatus == ResponseStatus.Completed && restResponse.StatusCode == HttpStatusCode.OK)
                return restResponse.Data;
            if (!string.IsNullOrWhiteSpace(restResponse.ErrorMessage))
                throw new Exception(restResponse.ErrorMessage);
            if (!string.IsNullOrWhiteSpace(restResponse.StatusDescription) && !string.IsNullOrWhiteSpace(restResponse.Content))
                throw new Exception(string.Format("{0} || {1}", (object)restResponse.StatusDescription, (object)restResponse.Content));
            return (Meeting)null;
        }

        public ListMeetings GetMeetings(string userId, MeetingListTypes type = MeetingListTypes.Scheduled, int pageSize = 30, int pageNumber = 1)
        {
            if (pageSize > 300)
                throw new Exception("GetMeetings page size max 300");
            RestRequest restRequest = this.BuildRequestAuthorization("users/{userId}/meetings", Method.GET);
            restRequest.AddParameter(nameof(userId), (object)userId, ParameterType.UrlSegment);
            restRequest.AddParameter(nameof(type), (object)type.ToString().ToLowerInvariant(), ParameterType.QueryString);
            restRequest.AddParameter("page_size", (object)pageSize, ParameterType.QueryString);
            restRequest.AddParameter("page_number", (object)pageNumber, ParameterType.QueryString);
            IRestResponse<ListMeetings> restResponse = this.WebClient.Execute<ListMeetings>((IRestRequest)restRequest);
            if (restResponse.ResponseStatus == ResponseStatus.Completed && restResponse.StatusCode == HttpStatusCode.OK)
                return restResponse.Data;
            if (!string.IsNullOrWhiteSpace(restResponse.ErrorMessage))
                throw new Exception(restResponse.ErrorMessage);
            if (!string.IsNullOrWhiteSpace(restResponse.StatusDescription) && !string.IsNullOrWhiteSpace(restResponse.Content))
                throw new Exception(string.Format("{0} || {1}", (object)restResponse.StatusDescription, (object)restResponse.Content));
            return (ListMeetings)null;
        }

        public ZoomListRegistrants GetMeetingRegistrants(string meetingId, string occurrenceId = null, string status="approved", int pageSize = 100, int pageNumber = 1)
        {
            if (pageSize > 300)
                throw new Exception("GetMeetingRegistrants page size max 300");
            RestRequest restRequest = this.BuildRequestAuthorization("/meetings/{meetingId}/registrants", Method.GET);
            restRequest.AddParameter(nameof(meetingId), (object)meetingId, ParameterType.UrlSegment);
            if (!string.IsNullOrEmpty(occurrenceId))
            {
                restRequest.AddParameter(nameof(occurrenceId), occurrenceId, ParameterType.QueryString);
            }
            restRequest.AddParameter("page_size", (object)pageSize, ParameterType.QueryString);
            restRequest.AddParameter("page_number", (object)pageNumber, ParameterType.QueryString);
            IRestResponse<ZoomListRegistrants> restResponse = this.WebClient.Execute<ZoomListRegistrants>((IRestRequest)restRequest);
            if (restResponse.ResponseStatus == ResponseStatus.Completed && restResponse.StatusCode == HttpStatusCode.OK)
                return restResponse.Data;
            if (!string.IsNullOrWhiteSpace(restResponse.ErrorMessage))
                throw new Exception(restResponse.ErrorMessage);
            if (!string.IsNullOrWhiteSpace(restResponse.StatusDescription) && !string.IsNullOrWhiteSpace(restResponse.Content))
                throw new Exception(string.Format("{0} || {1}", (object)restResponse.StatusDescription, (object)restResponse.Content));
            return (ZoomListRegistrants)null;
        }

        public ZoomAddRegistrantResponse AddRegistrant(string meetingId, ZoomAddRegistrantRequest registrant, string occurenceIds = null)
        {
            RestRequest restRequest = this.BuildRequestAuthorization("meetings/{meetingId}/registrants", Method.POST);
            restRequest.AddParameter(nameof(meetingId), (object)meetingId, ParameterType.UrlSegment);
            restRequest.AddJsonBody((object)registrant);
            IRestResponse<ZoomAddRegistrantResponse> restResponse = this.WebClient.Execute<ZoomAddRegistrantResponse>((IRestRequest)restRequest);
            if (restResponse.ResponseStatus == ResponseStatus.Completed && restResponse.StatusCode == HttpStatusCode.Created)
                return restResponse.Data;
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
        //    RestRequest restRequest = this.BuildRequestAuthorization("users/{userId}/meetings", Method.POST);
        //    restRequest.AddParameter(nameof(userId), (object)userId, ParameterType.UrlSegment);
        //    restRequest.AddJsonBody((object)meeting);
        //    IRestResponse<Meeting> restResponse = this.WebClient.Execute<Meeting>((IRestRequest)restRequest);
        //    if (restResponse.ResponseStatus == ResponseStatus.Completed && restResponse.StatusCode == HttpStatusCode.Created)
        //        return restResponse.Data;
        //    if (!string.IsNullOrWhiteSpace(restResponse.ErrorMessage))
        //        throw new Exception(restResponse.ErrorMessage);
        //    if (!string.IsNullOrWhiteSpace(restResponse.StatusDescription) && !string.IsNullOrWhiteSpace(restResponse.Content))
        //        throw new Exception(string.Format("{0} || {1}", (object)restResponse.StatusDescription, (object)restResponse.Content));
        //    return (Meeting)null;
        //}

        public bool UpdateRegistrantsStatus(string meetingId, ZoomUpdateRegistrantStatusRequest updateStatusRequest, string occurenceId)
        {
            RestRequest restRequest = this.BuildRequestAuthorization("meetings/{meetingId}/registrants/status", Method.PUT);
            restRequest.AddParameter(nameof(meetingId), (object)meetingId, ParameterType.UrlSegment);
            restRequest.AddJsonBody((object)updateStatusRequest);
            IRestResponse restResponse = this.WebClient.Execute((IRestRequest)restRequest);
            if (restResponse.ResponseStatus == ResponseStatus.Completed && restResponse.StatusCode == HttpStatusCode.NoContent)
                return true;
            if (!string.IsNullOrWhiteSpace(restResponse.ErrorMessage))
                throw new Exception(restResponse.ErrorMessage);
            if (!string.IsNullOrWhiteSpace(restResponse.StatusDescription) && !string.IsNullOrWhiteSpace(restResponse.Content))
                throw new Exception(string.Format("{0} || {1}", (object)restResponse.StatusDescription, (object)restResponse.Content));
            return false;
        }

        public bool DeleteMeeting(string meetingId, string occurrenceId = null)
        {
            RestRequest restRequest = this.BuildRequestAuthorization("meetings/{meetingId}", Method.DELETE);
            restRequest.AddParameter(nameof(meetingId), (object)meetingId, ParameterType.UrlSegment);
            if (!string.IsNullOrWhiteSpace(occurrenceId))
                restRequest.AddParameter("occurrence_id", (object)occurrenceId, ParameterType.QueryString);
            IRestResponse restResponse = this.WebClient.Execute((IRestRequest)restRequest);
            if (restResponse.ResponseStatus == ResponseStatus.Completed && restResponse.StatusCode == HttpStatusCode.NoContent)
                return true;
            if (!string.IsNullOrWhiteSpace(restResponse.ErrorMessage))
                throw new Exception(restResponse.ErrorMessage);
            if (!string.IsNullOrWhiteSpace(restResponse.StatusDescription) && !string.IsNullOrWhiteSpace(restResponse.Content))
                throw new Exception(string.Format("{0} || {1}", (object)restResponse.StatusDescription, (object)restResponse.Content));
            return false;
        }

        public ZoomApiRecordingList GetRecordings(string meetingId, bool trash = false)
        {
            RestRequest restRequest = this.BuildRequestAuthorization("meetings/{meetingId}/recordings", Method.GET);
            restRequest.AddParameter(nameof(meetingId), meetingId, ParameterType.UrlSegment);
            restRequest.AddParameter(nameof(trash), trash, ParameterType.QueryString);
            IRestResponse<ZoomApiRecordingList> restResponse = this.WebClient.Execute<ZoomApiRecordingList>((IRestRequest)restRequest);
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

        public ZoomRecordingSessionList GetUserRecordings(string userId, DateTime? from = null, DateTime? to = null, int pageSize = 30, string nextPageToken = null, bool mc = false, bool trash = false)
        {
            RestRequest restRequest = this.BuildRequestAuthorization($"users/{userId}/recordings", Method.GET);
            restRequest.AddParameter("page_size", pageSize, ParameterType.QueryString);
            if(from.HasValue)
                restRequest.AddParameter(nameof(from), from.Value.ToString("yyyy-MM-dd"), ParameterType.QueryString);
            if(to.HasValue)
                restRequest.AddParameter(nameof(to), to.Value.ToString("yyyy-MM-dd"), ParameterType.QueryString);
            if (!string.IsNullOrWhiteSpace(nextPageToken))
                restRequest.AddParameter("next_page_token", (object)nextPageToken, ParameterType.QueryString);
            restRequest.AddParameter(nameof(mc), mc.ToString().ToLower(), ParameterType.QueryString);
            restRequest.AddParameter(nameof(trash), trash.ToString().ToLower(), ParameterType.QueryString);
            IRestResponse<ZoomRecordingSessionList> restResponse = this.WebClient.Execute<ZoomRecordingSessionList>((IRestRequest)restRequest);
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

        public bool DeleteRecording(string meetingId, string recordingId = null, bool trash = true)
        {
            RestRequest restRequest = this.BuildRequestAuthorization("meetings/{meetingId}/recordings/{recordingId}", Method.DELETE);
            restRequest.AddParameter(nameof(meetingId), (object)meetingId, ParameterType.UrlSegment);
            if (!string.IsNullOrWhiteSpace(recordingId))
                restRequest.AddParameter(nameof(recordingId), (object)recordingId, ParameterType.UrlSegment);
            restRequest.AddParameter("action", (object)(trash?"trash":"delete"), ParameterType.QueryString);
            IRestResponse restResponse = this.WebClient.Execute((IRestRequest)restRequest);
            if (restResponse.ResponseStatus == ResponseStatus.Completed && restResponse.StatusCode == HttpStatusCode.NoContent)
                return true;
            if (!string.IsNullOrWhiteSpace(restResponse.ErrorMessage))
                throw new Exception(restResponse.ErrorMessage);
            if (!string.IsNullOrWhiteSpace(restResponse.StatusDescription) && !string.IsNullOrWhiteSpace(restResponse.Content))
                throw new Exception(string.Format("{0} || {1}", (object)restResponse.StatusDescription, (object)restResponse.Content));
            return false;
        }
        public bool RecoverRecording(string meetingId, string recordingId = null)
        {
            RestRequest restRequest = this.BuildRequestAuthorization($"meetings/{meetingId}/recordings/{(recordingId == null? "" : recordingId + "/")}status", Method.PUT);
            restRequest.AddJsonBody(new {action = "recover"});
            IRestResponse restResponse = this.WebClient.Execute((IRestRequest)restRequest);
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

        public string GetUserZpkToken(string userId)
        {
            RestRequest restRequest = this.BuildRequestAuthorization("users/{userId}/token", Method.GET);
            restRequest.AddParameter(nameof(userId), (object)userId, ParameterType.UrlSegment);
            restRequest.AddParameter("type", "zpk", ParameterType.QueryString);
            IRestResponse<ZoomToken> restResponse = this.WebClient.Execute<ZoomToken>((IRestRequest)restRequest);
            return restResponse.Data.Token;
        }

        public Meeting CreateMeeting(string userId, Meeting meeting)
        {
            RestRequest restRequest = this.BuildRequestAuthorization("users/{userId}/meetings", Method.POST);
            restRequest.AddParameter(nameof(userId), (object)userId, ParameterType.UrlSegment);
            restRequest.AddJsonBody((object)meeting);
            IRestResponse<Meeting> restResponse = this.WebClient.Execute<Meeting>((IRestRequest)restRequest);
            if (restResponse.ResponseStatus == ResponseStatus.Completed && restResponse.StatusCode == HttpStatusCode.Created)
                return restResponse.Data;
            if (!string.IsNullOrWhiteSpace(restResponse.ErrorMessage))
                throw new Exception(restResponse.ErrorMessage);
            if (!string.IsNullOrWhiteSpace(restResponse.StatusDescription) && !string.IsNullOrWhiteSpace(restResponse.Content))
                throw new Exception(string.Format("{0} || {1}", (object)restResponse.StatusDescription, (object)restResponse.Content));
            return (Meeting)null;
        }

        public bool UpdateMeeting(string meetingId, Meeting meeting)
        {
            RestRequest restRequest = this.BuildRequestAuthorization("meetings/{meetingId}", Method.PATCH);
            restRequest.AddParameter(nameof(meetingId), (object)meetingId, ParameterType.UrlSegment);
            restRequest.AddJsonBody((object)meeting);
            IRestResponse restResponse = this.WebClient.Execute((IRestRequest)restRequest);
            if (restResponse.ResponseStatus == ResponseStatus.Completed && restResponse.StatusCode == HttpStatusCode.NoContent)
                return true;
            if (!string.IsNullOrWhiteSpace(restResponse.ErrorMessage))
                throw new Exception(restResponse.ErrorMessage);
            if (!string.IsNullOrWhiteSpace(restResponse.StatusDescription) && !string.IsNullOrWhiteSpace(restResponse.Content))
                throw new Exception(string.Format("{0} || {1}", (object)restResponse.StatusDescription, (object)restResponse.Content));
            return false;
        }

        public ZoomMeetingsReportList GetMeetingsReport(string userId, DateTime from, DateTime to, int pageSize = 30, string nextPageToken = null)
        {
            if (pageSize > 300)
                throw new Exception("GetMeetingParticipantsReport page size max 300");
            RestRequest restRequest = this.BuildRequestAuthorization("report/users/{userId}/meetings", Method.GET);
            restRequest.AddParameter(nameof(userId), (object)userId, ParameterType.UrlSegment);
            restRequest.AddParameter("page_size", (object)pageSize, ParameterType.QueryString);
            restRequest.AddParameter("from", (object)from.ToString("yyyy-MM-dd"), ParameterType.QueryString);
            restRequest.AddParameter("to", (object)to.ToString("yyyy-MM-dd"), ParameterType.QueryString);
            if (!string.IsNullOrWhiteSpace(nextPageToken))
                restRequest.AddParameter("next_page_token", (object)nextPageToken, ParameterType.QueryString);
            IRestResponse<ZoomMeetingsReportList> restResponse = this.WebClient.Execute<ZoomMeetingsReportList>((IRestRequest)restRequest);
            if (restResponse.ResponseStatus == ResponseStatus.Completed && restResponse.StatusCode == HttpStatusCode.OK)
                return restResponse.Data;
            if (!string.IsNullOrWhiteSpace(restResponse.ErrorMessage))
                throw new Exception(restResponse.ErrorMessage);
            if (!string.IsNullOrWhiteSpace(restResponse.StatusDescription) && !string.IsNullOrWhiteSpace(restResponse.Content))
                throw new Exception(string.Format("{0} || {1}", (object)restResponse.StatusDescription, (object)restResponse.Content));
            return (ZoomMeetingsReportList)null;
        }
        public MeetingParticipantsReport GetMeetingParticipantsReport(string meetingId, int pageSize = 30, string nextPageToken = null)
        {
            if (pageSize > 300)
                throw new Exception("GetMeetingParticipantsReport page size max 300");
            RestRequest restRequest = this.BuildRequestAuthorization($"report/meetings/{meetingId}/participants", Method.GET);
            //restRequest.AddParameter(nameof(meetingId), (object)meetingId, ParameterType.UrlSegment);
            restRequest.AddParameter("page_size", (object)pageSize, ParameterType.QueryString);
            if (!string.IsNullOrWhiteSpace(nextPageToken))
                restRequest.AddParameter("next_page_token", (object)nextPageToken, ParameterType.QueryString);
            IRestResponse<MeetingParticipantsReport> restResponse = this.WebClient.Execute<MeetingParticipantsReport>((IRestRequest)restRequest);
            if (restResponse.ResponseStatus == ResponseStatus.Completed && restResponse.StatusCode == HttpStatusCode.OK)
                return restResponse.Data;
            if (!string.IsNullOrWhiteSpace(restResponse.ErrorMessage))
                throw new Exception(restResponse.ErrorMessage);
            if (!string.IsNullOrWhiteSpace(restResponse.StatusDescription) && !string.IsNullOrWhiteSpace(restResponse.Content))
                throw new Exception(string.Format("{0} || {1}", (object)restResponse.StatusDescription, (object)restResponse.Content));
            return (MeetingParticipantsReport)null;
        }

        public ListMeetingParticipantsDetails GetMeetingParticipantsDetails(string meetingId, int pageSize = 30, string nextPageToken = null)
        {
            if (pageSize > 300)
                throw new Exception("GetMeetingParticipantsReport page size max 300");
            RestRequest restRequest = this.BuildRequestAuthorization($"metrics/meetings/{meetingId}/participants", Method.GET);
            restRequest.AddParameter("page_size", (object)pageSize, ParameterType.QueryString);
            restRequest.AddParameter("type", "past", ParameterType.QueryString);
            if (!string.IsNullOrWhiteSpace(nextPageToken))
                restRequest.AddParameter("next_page_token", (object)nextPageToken, ParameterType.QueryString);
            IRestResponse<ListMeetingParticipantsDetails> restResponse = this.WebClient.Execute<ListMeetingParticipantsDetails>((IRestRequest)restRequest);
            if (restResponse.ResponseStatus == ResponseStatus.Completed && restResponse.StatusCode == HttpStatusCode.OK)
                return restResponse.Data;
            if (!string.IsNullOrWhiteSpace(restResponse.ErrorMessage))
                throw new Exception(restResponse.ErrorMessage);
            if (!string.IsNullOrWhiteSpace(restResponse.StatusDescription) && !string.IsNullOrWhiteSpace(restResponse.Content))
                throw new Exception(string.Format("{0} || {1}", (object)restResponse.StatusDescription, (object)restResponse.Content));
            return (ListMeetingParticipantsDetails)null;
        }

        private RestRequest BuildRequestAuthorization(string resource, Method method)
        {
            //return this.WebClient.BuildRequestAuthorization(this.Options, resource, method);

            RestRequest restRequest = new RestRequest(resource, method);

            var token = JwtEncode(Options.ZoomApiKey, Options.ZoomApiSecret);
            WebClient.Authenticator = (IAuthenticator)new JwtAuthenticator(token);
            NewtonsoftJsonSerializer newtonsoftJsonSerializer = new NewtonsoftJsonSerializer();
            restRequest.JsonSerializer = (ISerializer)newtonsoftJsonSerializer;
            return restRequest;
        }

        private static string JwtEncode(string key, string secret)
        {
            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var signingCredentials = new SigningCredentials(signingKey,
                SecurityAlgorithms.HmacSha256Signature, SecurityAlgorithms.Sha256Digest);

            var expires = DateTime.UtcNow.AddMinutes(1);
            var securityTokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = key,
                Expires = expires,
                SigningCredentials = signingCredentials
            };

            var tokenHandler = new JwtSecurityTokenHandler { SetDefaultTimesOnTokenCreation = false };
            var plainToken = tokenHandler.CreateToken(securityTokenDescriptor);
            var signedAndEncodedToken = tokenHandler.WriteToken(plainToken);
            return signedAndEncodedToken;
        }
    }
}