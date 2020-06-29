using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Esynctraining.Lti.Lms.Common.Constants;
using Newtonsoft.Json;

namespace EdugameCloud.Lti.Bridge
{
    public class BridgeApi : IBridgeApi
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public BridgeApi(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        }

        // https://api.bridgeapp.com/doc/api/html/file.API_Overview.html
        private string GetBasicHeader(Dictionary<string, object> licenseSettings)
        {
            var key = licenseSettings[LmsLicenseSettingNames.BridgeApiTokenKey];
            var secret = licenseSettings[LmsLicenseSettingNames.BridgeApiTokenSecret];
            return Convert.ToBase64String(Encoding.UTF8.GetBytes($"{key}:{secret}"));
        }

        public async Task<List<BridgeApiUser>> GetCourseUsers(string courseId, Dictionary<string, object> licenseSettings)
        {
            var basicHeader = GetBasicHeader(licenseSettings);
            var apiUrl = "https://" + licenseSettings[LmsLicenseSettingNames.LmsDomain];
            var relativeUrl = $"api/author/live_courses/{courseId}/learners";
            var users = await GetRestCall<LiveCourseEnrollmentsResponse>(basicHeader, apiUrl, relativeUrl);
            return await GetUsersById(users?.enrollments?.Select(x => x.user_id), licenseSettings);
        }

        public async Task<BridgeApiUser> GetUserProfile(string uuid, Dictionary<string, object> licenseSettings)
        {
            var basicHeader = GetBasicHeader(licenseSettings);
            var apiUrl = "https://" + licenseSettings[LmsLicenseSettingNames.LmsDomain];
            var relativeUrl = "/api/author/users?search=" + uuid;
            var users = await GetRestCall<BridgeApiUsersResponse>(basicHeader, apiUrl, relativeUrl);
            return users?.Users?.FirstOrDefault();
        }

        public async Task<List<BridgeApiUser>> GetUsersById(IEnumerable<string> ids, Dictionary<string, object> licenseSettings)
        {
            var basicHeader = GetBasicHeader(licenseSettings);
            var apiUrl = "https://" + licenseSettings[LmsLicenseSettingNames.LmsDomain];
            var relativeUrl = "/api/author/users";
            var users = await GetRestCall<BridgeApiUsersResponse>(basicHeader, apiUrl, relativeUrl);
            return users?.Users?.Where(x => ids.Any(id => id == x.Id.ToString())).ToList();
        }

        public async Task<IEnumerable<LiveSessionResponse>> ListSessions(string courseId, Dictionary<string, object> licenseSettings)
        {
            if(courseId == null)
                throw new ArgumentNullException(nameof(courseId));

            var basicHeader = GetBasicHeader(licenseSettings);
            var apiUrl = "https://" + licenseSettings[LmsLicenseSettingNames.LmsDomain];
            var relativeUrl = $"/api/author/live_courses/{courseId}/sessions";
            var response = await GetRestCall<ListSessionsResponse>(basicHeader, apiUrl, relativeUrl);
            return response?.Sessions;
        }

        public async Task<IEnumerable<LiveSessionResponse>> CreateSessions(string courseId, IEnumerable<LiveSessionRequest> sessions, Dictionary<string, object> licenseSettings)
        {
            if (courseId == null)
                throw new ArgumentNullException(nameof(courseId));

            var basicHeader = GetBasicHeader(licenseSettings);
            var apiUrl = "https://" + licenseSettings[LmsLicenseSettingNames.LmsDomain];
            var relativeUrl = $"/api/author/live_courses/{courseId}/sessions";
            var jsonData = JsonConvert.SerializeObject(new {sessions = sessions.ToArray()});
            var response = await GetRestCall<ListSessionsResponse>(basicHeader, apiUrl, relativeUrl, HttpMethod.Post, jsonData);
            return response?.Sessions;
        }

        public async Task<bool> RegisterUserToSession(int sessionId, int userId, Dictionary<string, object> licenseSettings)
        {
            if (sessionId == 0)
                throw new ArgumentOutOfRangeException();

            var basicHeader = GetBasicHeader(licenseSettings);
            var apiUrl = "https://" + licenseSettings[LmsLicenseSettingNames.LmsDomain];
            var relativeUrl = $"/api/author/live_course_sessions/{sessionId}/registrations";
            var jsonData = JsonConvert.SerializeObject(new { user_id = userId});
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, relativeUrl);
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Basic", basicHeader);
            requestMessage.Content = new StringContent(jsonData, Encoding.UTF8, "application/json");
            var response = await CreateClient(apiUrl).SendAsync(requestMessage);
            response.EnsureSuccessStatusCode();
            return response.StatusCode == HttpStatusCode.NoContent;
        }

        public async Task<IEnumerable<LiveSessionResponse>> PublishSession(string courseId, int sessionId, Dictionary<string, object> licenseSettings)
        {
            if (courseId == null)
                throw new ArgumentNullException(nameof(courseId));
            if (sessionId == 0)
                throw new ArgumentOutOfRangeException();

            var basicHeader = GetBasicHeader(licenseSettings);
            var apiUrl = "https://" + licenseSettings[LmsLicenseSettingNames.LmsDomain];
            var relativeUrl = $"/api/author/live_courses/{courseId}/sessions/{sessionId}/publish";

            var response = await GetRestCall<ListSessionsResponse>(basicHeader, apiUrl, relativeUrl, HttpMethod.Post);
            return response?.Sessions;
        }

        public async Task<IEnumerable<WebConferenceResponse>> UpdateSessionWebconference(string courseId, int sessionId, WebConferenceRequest conference, Dictionary<string, object> licenseSettings)
        {
            if (courseId == null)
                throw new ArgumentNullException(nameof(courseId));

            var basicHeader = GetBasicHeader(licenseSettings);
            var apiUrl = "https://" + licenseSettings[LmsLicenseSettingNames.LmsDomain];
            var relativeUrl = $"/api/author/live_courses/{courseId}/sessions/{sessionId}/web_conference";
            var jsonData = JsonConvert.SerializeObject(new { web_conference = conference });

            var response = await GetRestCall<WebConferencesResponse>(basicHeader, apiUrl, relativeUrl, HttpMethod.Put, jsonData);
            return response?.web_conferences;
        }

        public async Task<bool> DeleteSession(string courseId, string sessionId, Dictionary<string, object> licenseSettings)
        {
            if (courseId == null)
                throw new ArgumentNullException(nameof(courseId));

            var basicHeader = GetBasicHeader(licenseSettings);
            var apiUrl = "https://" + licenseSettings[LmsLicenseSettingNames.LmsDomain];
            var relativeUrl = $"/api/author/live_courses/{courseId}/sessions/{sessionId}";
            var requestMessage = new HttpRequestMessage(HttpMethod.Delete, relativeUrl);

            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Basic", basicHeader);
            var response = await CreateClient(apiUrl).SendAsync(requestMessage);
            response.EnsureSuccessStatusCode();
            return response.StatusCode == HttpStatusCode.NoContent;
        }

        public async Task<LiveSessionResponse> UpdateSession(string courseId, int sessionId, LiveSessionRequest session, Dictionary<string, object> licenseSettings)
        {
            if (courseId == null)
                throw new ArgumentNullException(nameof(courseId));

            var basicHeader = GetBasicHeader(licenseSettings);
            var apiUrl = "https://" + licenseSettings[LmsLicenseSettingNames.LmsDomain];
            var relativeUrl = $"/api/author/live_courses/{courseId}/sessions/{sessionId}";
            var jsonData = JsonConvert.SerializeObject(new { session = new {session.start_at, session.end_at} });

            var response = await GetRestCall<ListSessionsResponse>(basicHeader, apiUrl, relativeUrl, HttpMethod.Put, jsonData);
            return response?.Sessions?.Single();
        }

        public async Task<LiveSessionResponse> GetSession(string courseId, string sessionId, Dictionary<string, object> licenseSettings)
        {
            if (courseId == null)
                throw new ArgumentNullException(nameof(courseId));
            if (sessionId == null)
                throw new ArgumentNullException(nameof(sessionId));

            var basicHeader = GetBasicHeader(licenseSettings);
            var apiUrl = "https://" + licenseSettings[LmsLicenseSettingNames.LmsDomain];
            var relativeUrl = $"/api/author/live_courses/{courseId}/sessions/{sessionId}";
            var response = await GetRestCall<ListSessionsResponse>(basicHeader, apiUrl, relativeUrl);
            return response?.Sessions?.FirstOrDefault();
        }

        public async Task<T> GetRestCall<T>(string basicHeader, string apiUrl, string relativeUrl, HttpMethod httpMethod = null, string jsonData = null)
        {
            var requestMessage = new HttpRequestMessage(httpMethod ?? HttpMethod.Get, relativeUrl);

            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Basic", basicHeader);
            if (jsonData != null)
            {
                requestMessage.Content = new StringContent(jsonData, Encoding.UTF8, "application/json");
            }
            var response = await CreateClient(apiUrl).SendAsync(requestMessage);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsAsync<T>();//new List<MediaTypeFormatter>{ new JilMediaTypeFormatter(JilSerializer.JilOptions) }
        }

        private HttpClient CreateClient(string apiUrl)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(apiUrl);
            return client;
        }
    }
}