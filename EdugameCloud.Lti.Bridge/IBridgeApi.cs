using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using EdugameCloud.Lti.Domain.Entities;

namespace EdugameCloud.Lti.Bridge
{
    public interface IBridgeApi
    {
        Task<List<BridgeApiUser>> GetCourseUsers(string courseId, ILmsLicense lmsCompany);
        Task<BridgeApiUser> GetUserProfile(string uuid, ILmsLicense lmsCompany);
        Task<List<BridgeApiUser>> GetUsersById(IEnumerable<string> ids, ILmsLicense lmsCompany);

        Task<T> GetRestCall<T>(string basicHeader, string apiUrl, string relativeUrl, HttpMethod httpMethod = null,
            string jsonData = null);

        Task<LiveSessionResponse> GetSession(string courseId, string sessionId, ILmsLicense lmsCompany);
        Task<bool> DeleteSession(string courseId, string sessionId, ILmsLicense lmsCompany);

        Task<IEnumerable<WebConferenceResponse>> UpdateSessionWebconference(string courseId, int sessionId,
            WebConferenceRequest conference, ILmsLicense lmsCompany);

        Task<bool> RegisterUserToSession(int sessionId, int userId, ILmsLicense lmsCompany);

        Task<IEnumerable<LiveSessionResponse>> CreateSessions(string courseId, IEnumerable<LiveSessionRequest> sessions,
            ILmsLicense lmsLicense);

        Task<IEnumerable<LiveSessionResponse>> PublishSession(string courseId, int sessionId, ILmsLicense lmsCompany);
        Task<IEnumerable<LiveSessionResponse>> ListSessions(string courseId, ILmsLicense lmsCompany);
    }
}