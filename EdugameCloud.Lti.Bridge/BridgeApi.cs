using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using EdugameCloud.HttpClient;
using EdugameCloud.Lti.Domain.Entities;

namespace EdugameCloud.Lti.Bridge
{
    public class BridgeApi : IBridgeApi
    {

        private string GetBasicHeader(ILmsLicense lmsCompany)
        {
            return "NTJjODlhMGItMDhlYy00NWVhLTlkM2ItOGJlZDRhNTU5YjNlOmYwM2ZkYTk4LTMwYTAtNGRmYy04YzZiLWJmYTc4MTBhYTI3MQ==";
        }

        public async Task<List<BridgeApiUser>> GetCourseUsers(string courseId, ILmsLicense lmsCompany)
        {
            var basicHeader = GetBasicHeader(lmsCompany);
            var apiUrl = "https://" + lmsCompany.LmsDomain;
            var relativeUrl = $"api/author/live_courses/{courseId}/learners";
            var users = await GetRestCall<LiveCourseEnrollmentsResponse>(basicHeader, apiUrl, relativeUrl);
            return await GetUsersById(users?.enrollments?.Select(x => x.user_id), lmsCompany);
        }

        public async Task<BridgeApiUser> GetUserProfile(string uuid, ILmsLicense lmsCompany)
        {
            var basicHeader = GetBasicHeader(lmsCompany);
            var apiUrl = "https://" + lmsCompany.LmsDomain;
            var relativeUrl = "/api/author/users?search=" + uuid;
            var users = await GetRestCall<BridgeApiUsersResponse>(basicHeader, apiUrl, relativeUrl);
            return users?.Users?.FirstOrDefault();
        }

        public async Task<List<BridgeApiUser>> GetUsersById(IEnumerable<string> ids, ILmsLicense lmsCompany)
        {
            var basicHeader = GetBasicHeader(lmsCompany);
            var apiUrl = "https://" + lmsCompany.LmsDomain;
            var relativeUrl = "/api/author/users";
            var users = await GetRestCall<BridgeApiUsersResponse>(basicHeader, apiUrl, relativeUrl);
            return users?.Users?.Where(x => ids.Any(id => id == x.Id)).ToList();
        }

        public async Task<T> GetRestCall<T>(string basicHeader, string apiUrl, string relativeUrl)
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, relativeUrl);

            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Basic", basicHeader);
            var response = await CreateClient(apiUrl).SendAsync(requestMessage);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsAsync<T>();
        }

        private HttpClientWrapper CreateClient(string apiUrl)
        {
            return new HttpClientWrapper(new Uri(apiUrl));
        }
    }
}