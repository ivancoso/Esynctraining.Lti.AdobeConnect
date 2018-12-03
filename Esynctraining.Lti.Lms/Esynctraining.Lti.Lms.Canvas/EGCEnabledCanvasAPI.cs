using EdugameCloud.Lti.Canvas;
using Esynctraining.Core.Json;
using Esynctraining.Core.Logging;
using Esynctraining.Lti.Lms.Common;
using Esynctraining.Lti.Lms.Common.API;
using Esynctraining.Lti.Lms.Common.API.Canvas;
using Esynctraining.Lti.Lms.Common.Constants;
using Esynctraining.Lti.Lms.Common.Dto;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using Esynctraining.Lti.Lms.Canvas.Convertors;
using Esynctraining.Lti.Lms.Canvas.DTOs;

namespace Esynctraining.Lti.Lms.Canvas
{
    /// <summary>
    /// The Canvas API for EGC.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public sealed class EGCEnabledCanvasAPI : CanvasAPI, IEGCEnabledLmsAPI, IEGCEnabledCanvasAPI
    {
        public EGCEnabledCanvasAPI(ILogger logger, IJsonDeserializer jsonDeserializer)
            : base(logger, jsonDeserializer)
        {
        }

        public async Task<LmsUserDTO> GetCourseUser(string userId, Dictionary<string, object> licenseSettings, string courseId)
        {
            var userToken = (string)licenseSettings[LmsUserSettingNames.Token];
            var refreshToken = (string)licenseSettings[LmsUserSettingNames.RefreshToken];

            try
            {
                Validate((string)licenseSettings[LmsLicenseSettingNames.LmsDomain], userToken);

                if (!licenseSettings.ContainsKey(LmsUserSettingNames.Token))
                {
                    _logger.Error($"There is no user token provided with license parameters for license '{licenseSettings[LmsLicenseSettingNames.LicenseKey]}'.");
                    throw new WarningMessageException(EdugameCloud.Lti.Canvas.Resources.Messages.NoLicenseAdmin);
                }

                var token = (string)licenseSettings[LmsUserSettingNames.Token];

                if (string.IsNullOrWhiteSpace(token))
                {
                    _logger.Error($"Empty token provided with license parameters for license '{licenseSettings[LmsLicenseSettingNames.LicenseKey]}'.");
                    throw new WarningMessageException(EdugameCloud.Lti.Canvas.Resources.Messages.NoLicenseAdmin);
                }

                LmsUserDTO user = await FetchCourseUser(userId, 
                                                        (string)licenseSettings[LmsLicenseSettingNames.LmsDomain], 
                                                        token, 
                                                        courseId,
                                                        (string)licenseSettings[LmsLicenseSettingNames.CanvasOAuthId],
                                                        (string)licenseSettings[LmsLicenseSettingNames.CanvasOAuthKey],
                                                        refreshToken);
                return user;
            }
            catch (Exception ex)
            {
                _logger.Error($"[EGCEnabledCanvasAPI.GetCourseUser] API:{licenseSettings["LmsDomain"]}. UserToken:{userToken}. CourseId:{courseId}. UserId:{userId}.", ex);
                throw;
            }
        }

        private async Task<LmsUserDTO> FetchCourseUser(string userId, string domain, string userToken, string courseId, string oauthId, string oauthKey, string refreshToken)
        {
            try
            {
                Validate(domain, userToken);

                var client = CreateRestClient(domain);

                var link = string.Format("/api/v1/courses/{0}/users/{1}?include[]=email&include[]=enrollments",
                        courseId, userId);

                RestRequest request = await CreateRequest(domain, link, Method.GET, userToken, oauthId, oauthKey, refreshToken);

                IRestResponse<CanvasLmsUserDTO> response = await client.ExecuteTaskAsync<CanvasLmsUserDTO>(request);

                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    // NOTE: user not longer exists in Canvas - return null;
                    return null;
                }

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    _logger.ErrorFormat("[EGCEnabledCanvasAPI.FetchCourseUser] API:{0}. UserToken:{1}. CourseId:{2}. UserId:{3}. {4}",
                        domain, userToken, courseId, userId, BuildInformation(response));
                    throw new InvalidOperationException(string.Format("[EGCEnabledCanvasAPI.FetchCourseUser] Canvas returns '{0}'", response.StatusDescription));
                }

                var user = response.Data;
                if (user == null)
                    return null;
                if (!user.enrollments.Any(x => x.course_id == courseId && x.enrollment_state == CanvasEnrollment.EnrollmentState.Active))
                    return null;

                var result = new LmsUserDTO
                {
                    Id = user.Id,
                    Email = user.Email,
                    Login = user.login_id,
                    Name = user.Name,
                    PrimaryEmail = user.Email,
                    LmsRole = SetRole(user, courseId)
                };

                return result;
            }
            catch (Exception ex)
            {
                _logger.ErrorFormat(ex, "[EGCEnabledCanvasAPI.FetchCourseUser] API:{0}. UserToken:{1}. CourseId:{2}. UserId:{3}.", domain, userToken, courseId, userId);
                throw;
            }
        }


        public async Task<List<LmsUserDTO>> GetUsersForCourse(string domain, string courseId, Dictionary<string, object> licenseSettings)
        {
            string userToken = (string)licenseSettings[LmsUserSettingNames.Token];
            string refreshToken = (string)licenseSettings[LmsUserSettingNames.RefreshToken];

            try
            {
                Validate(domain, userToken);

                var link = string.Format("/api/v1/courses/{0}/users?per_page={1}&include[]=email&include[]=enrollments",
                    courseId, 100); // default is 10 records per page, max - 100
                var users = await GetAllPages<CanvasLmsUserDTO>(domain, link, userToken, 
                    licenseSettings[LmsLicenseSettingNames.CanvasOAuthId].ToString(),
                    licenseSettings[LmsLicenseSettingNames.CanvasOAuthKey].ToString(), refreshToken, domain,
                    x => x.enrollments.Any(enr =>
                        enr.course_id == courseId && enr.enrollment_state == CanvasEnrollment.EnrollmentState.Active));
                return users.Select(user => new LmsUserDTO
                    {
                        Id = user.Id,
                        Email = user.Email,
                        Login = user.login_id,
                        Name = user.Name,
                        PrimaryEmail = user.Email,
                        LmsRole = SetRole(user, courseId),
                        SectionIds = user.enrollments.Select(x => x.course_section_id.ToString()).ToList()
                    }).ToList();

            }
            catch (Exception ex)
            {
                _logger.ErrorFormat(ex, "[EGCEnabledCanvasAPI.GetUsersForCourse] API:{0}. UserToken:{1}. CourseId:{2}.", domain, userToken, courseId);
                throw;
            }
        }

        private async Task<List<T>> GetAllPages<T>(string domain, string link, string userToken, string oauthId, string oauthKey, string refreshToken, string lmsDomain, Func<T, bool> filter = null)
        {
            var result = new List<T>();
            var client = CreateRestClient(domain);

            while (!string.IsNullOrEmpty(link))
            {
                RestRequest request = await CreateRequest(domain, link, Method.GET, userToken, oauthId, oauthKey, refreshToken);

                IRestResponse<List<T>> response = await client.ExecuteTaskAsync<List<T>>(request);

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    var errorData = JsonConvert.DeserializeObject<CanvasApiErrorWrapper>(response.Content);
                    if (errorData?.errors != null && errorData.errors.Any())
                    {
                        _logger.Error(
                            $"[Canvas API error] StatusCode:{response.StatusCode}, StatusDescription:{response.StatusDescription}, link: {link}, domain:{domain}.");
                        foreach (var error in errorData.errors)
                        {
                            _logger.Error($"[Canvas API error] Response error: {error.message}");
                        }
                    }
                    return result;
                }

                link = ExtractNextPageLink(response);
                IEnumerable<T> records = response.Data;
                if (filter != null)
                {
                    records = records.Where(filter);
                }

                result.AddRange(records);
            }

            return result;
        }

        private static string ExtractNextPageLink<T>(IRestResponse<List<T>> response)
        {
            var link = string.Empty;

            foreach (var h in response.Headers)
            {
                if (h.Name.Equals("Link", StringComparison.InvariantCultureIgnoreCase))
                {
                    link = h.Value.ToString();
                    var index = link.IndexOf("rel=\"next\"");
                    if (index > -1)
                    {
                        link = link.Substring(0, index - 2);
                        index = link.LastIndexOf(">");
                        if (index > -1)
                        {
                            link = link.Substring(0, index);
                            index = link.LastIndexOf("<");
                            if (index > -1)
                            {
                                link = link.Substring(index + 1);
                                index = link.IndexOf("/api/v1/");
                                if (index > -1)
                                {
                                    link = link.Substring(index);
                                }
                                break;
                            }
                        }
                    }
                }
                link = string.Empty;
            }
            return link;
        }

        private string SetRole(CanvasLmsUserDTO canvasDto, string courseid)
        {
            if (canvasDto.enrollments != null)
            {
                var enrollment = canvasDto.enrollments.FirstOrDefault(x => x.course_id == courseid);
                if (enrollment != null)
                {
                    return enrollment.role.Replace("Enrollment", String.Empty);
                }
            }

            _logger.WarnFormat("[Canvas API] User without role. CourseId:{0}, UserId:{1}",
                        courseid, canvasDto.Id);
            return null;
        }

        Task IEGCEnabledCanvasAPI.UpdateCalendarEvent(string domain, string courseId, Dictionary<string, object> licenseSettings, DateTime startTime, string endTime, string title)
        {
            throw new NotImplementedException();
        }

        public async Task<LmsCalendarEventDTO> CreateCalendarEvent(string courseId, Dictionary<string, object> licenseSettings, LmsCalendarEventDTO lmsEvent)
        {
            var domain = (string)licenseSettings[LmsLicenseSettingNames.LmsDomain];
            string userToken = (string)licenseSettings[LmsUserSettingNames.Token];
            string refreshToken = (string)licenseSettings[LmsUserSettingNames.RefreshToken];

            var client = CreateRestClient(domain);

            string startTimeUtc = lmsEvent.StartAt.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss");
            string endTimeUtc = lmsEvent.EndAt.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss");
            string eventTitle = HttpUtility.UrlEncode(lmsEvent.Title);
            var link = $"/api/v1/calendar_events?calendar_event[context_code]=course_{courseId}&calendar_event[title]={eventTitle}&calendar_event[start_at]={startTimeUtc}&calendar_event[end_at]={endTimeUtc}";

            string oauthId = licenseSettings[LmsLicenseSettingNames.CanvasOAuthId].ToString();
            string oauthKey = licenseSettings[LmsLicenseSettingNames.CanvasOAuthKey].ToString();

            RestRequest request = await CreateRequest(domain, link, Method.POST, userToken, oauthId, oauthKey, refreshToken);
            IRestResponse<CanvasCalendarEventDTO> response = await client.ExecuteTaskAsync<CanvasCalendarEventDTO>(request);

            if (response.StatusCode != HttpStatusCode.Created)
            {
                _logger.Error($"[Canvas API error] Response error: {response.Content}");
                return null;
            }
            LmsCalendarEventDTO lmsCalendarEvent = CanvasCalendarConvertor.ConvertToLmsCalendarEvent(response.Data);
            return lmsCalendarEvent;
        }

        public async Task<LmsCalendarEventDTO> UpdateCalendarEvent(string courseId, Dictionary<string, object> licenseSettings, LmsCalendarEventDTO lmsEvent)
        {
            var domain = (string)licenseSettings[LmsLicenseSettingNames.LmsDomain];
            string userToken = (string)licenseSettings[LmsUserSettingNames.Token];
            string refreshToken = (string)licenseSettings[LmsUserSettingNames.RefreshToken];

            var client = CreateRestClient(domain);

            string startTimeUtc = lmsEvent.StartAt.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss");
            string endTimeUtc = lmsEvent.EndAt.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss");
            string eventTitle = HttpUtility.UrlEncode(lmsEvent.Title);
            var link = $"/api/v1/calendar_events/{lmsEvent.Id}?calendar_event[title]={eventTitle}&calendar_event[start_at]={startTimeUtc}&calendar_event[end_at]={endTimeUtc}";

            string oauthId = licenseSettings[LmsLicenseSettingNames.CanvasOAuthId].ToString();
            string oauthKey = licenseSettings[LmsLicenseSettingNames.CanvasOAuthKey].ToString();

            RestRequest request = await CreateRequest(domain, link, Method.PUT, userToken, oauthId, oauthKey, refreshToken);
            IRestResponse<CanvasCalendarEventDTO> response = await client.ExecuteTaskAsync<CanvasCalendarEventDTO>(request);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                var errorData = JsonConvert.DeserializeObject<CanvasApiErrorWrapper>(response.Content);
                if (errorData?.errors != null && errorData.errors.Any())
                {
                    _logger.Error(
                        $"[Canvas API error] StatusCode:{response.StatusCode}, StatusDescription:{response.StatusDescription}, link: {link}, domain:{domain}.");
                    foreach (var error in errorData.errors)
                    {
                        _logger.Error($"[Canvas API error] Response error: {error.message}");
                    }
                }
            }
            LmsCalendarEventDTO lmsCalendarEvent = CanvasCalendarConvertor.ConvertToLmsCalendarEvent(response.Data);
            return lmsCalendarEvent;

        }


        public async Task<IEnumerable<LmsCalendarEventDTO>> GetUserCalendarEvents(string userId, Dictionary<string, object> licenseSettings)
        {

            var domain = (string)licenseSettings[LmsLicenseSettingNames.LmsDomain];
            string userToken = (string)licenseSettings[LmsUserSettingNames.Token];
            string refreshToken = (string)licenseSettings[LmsUserSettingNames.RefreshToken];
            string oauthId = licenseSettings[LmsLicenseSettingNames.CanvasOAuthId].ToString();
            string oauthKey = licenseSettings[LmsLicenseSettingNames.CanvasOAuthKey].ToString();

            var client = CreateRestClient(domain);
            var link = $"/api/v1/users/{userId}/calendar_events";

            RestRequest request = await CreateRequest(domain, link, Method.GET, userToken, oauthId, oauthKey, refreshToken);
            IRestResponse<IEnumerable<CanvasCalendarEventDTO>> response = await client.ExecuteTaskAsync<IEnumerable<CanvasCalendarEventDTO>>(request);
            
            if (response.StatusCode != HttpStatusCode.OK)
            {
                var errorData = JsonConvert.DeserializeObject<CanvasApiErrorWrapper>(response.Content);
                if (errorData?.errors != null && errorData.errors.Any())
                {
                    _logger.Error(
                        $"[Canvas API error] StatusCode:{response.StatusCode}, StatusDescription:{response.StatusDescription}, link: {link}, domain:{domain}.");
                    foreach (var error in errorData.errors)
                    {
                        _logger.Error($"[Canvas API error] Response error: {error.message}");
                    }
                }
            }

            IEnumerable<CanvasCalendarEventDTO> events = response.Data;
            return events.Select(e => CanvasCalendarConvertor.ConvertToLmsCalendarEvent(e));
        }

        public async Task DeleteCalendarEvents(int eventId, Dictionary<string, object> licenseSettings)
        {
            var domain = (string)licenseSettings[LmsLicenseSettingNames.LmsDomain];
            string userToken = (string)licenseSettings[LmsUserSettingNames.Token];
            string refreshToken = (string)licenseSettings[LmsUserSettingNames.RefreshToken];
            string oauthId = licenseSettings[LmsLicenseSettingNames.CanvasOAuthId].ToString();
            string oauthKey = licenseSettings[LmsLicenseSettingNames.CanvasOAuthKey].ToString();

            var client = CreateRestClient(domain);
            var link = $"/api/v1/calendar_events/{eventId}";

            RestRequest request = await CreateRequest(domain, link, Method.DELETE, userToken, oauthId, oauthKey, refreshToken);
            IRestResponse<IEnumerable<CanvasCalendarEventDTO>> response = await client.ExecuteTaskAsync<IEnumerable<CanvasCalendarEventDTO>>(request);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                var errorData = JsonConvert.DeserializeObject<CanvasApiErrorWrapper>(response.Content);
                if (errorData?.errors != null && errorData.errors.Any())
                {
                    _logger.Error(
                        $"[Canvas API error] StatusCode:{response.StatusCode}, StatusDescription:{response.StatusDescription}, link: {link}, domain:{domain}.");
                    foreach (var error in errorData.errors)
                    {
                        _logger.Error($"[Canvas API error] Response error: {error.message}");
                    }
                }
            }
        }
    }

}
