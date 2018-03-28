using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using EdugameCloud.Lti.Domain.Entities;

namespace EdugameCloud.Lti.Bridge
{
    public interface IBridgeApi
    {
        Task<List<BridgeApiUser>> GetCourseUsers(string courseId, ILmsLicense lmsLicense);
        Task<BridgeApiUser> GetUserProfile(string uuid, ILmsLicense lmsLicense);
        Task<List<BridgeApiUser>> GetUsersById(IEnumerable<string> ids, ILmsLicense lmsLicense);

        Task<T> GetRestCall<T>(string basicHeader, string apiUrl, string relativeUrl, HttpMethod httpMethod = null,
            string jsonData = null);

        Task<IEnumerable<LiveSessionResponse>> ListSessions(string courseId, ILmsLicense lmsLicense);
        Task<LiveSessionResponse> GetSession(string courseId, string sessionId, ILmsLicense lmsLicense);
        Task<IEnumerable<LiveSessionResponse>> CreateSessions(string courseId, IEnumerable<LiveSessionRequest> sessions,
            ILmsLicense lmsLicense);

        Task<LiveSessionResponse> UpdateSession(string courseId, int sessionId, LiveSessionRequest session,
            ILmsLicense lmsCompany);

        Task<bool> DeleteSession(string courseId, string sessionId, ILmsLicense lmsLicense);

        Task<IEnumerable<WebConferenceResponse>> UpdateSessionWebconference(string courseId, int sessionId,
            WebConferenceRequest conference, ILmsLicense lmsLicense);

        Task<bool> RegisterUserToSession(int sessionId, int userId, ILmsLicense lmsLicense);

        Task<IEnumerable<LiveSessionResponse>> PublishSession(string courseId, int sessionId, ILmsLicense lmsLicense);
    }
}