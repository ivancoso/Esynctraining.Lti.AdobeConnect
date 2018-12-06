using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace EdugameCloud.Lti.Bridge
{
    public interface IBridgeApi
    {
        Task<List<BridgeApiUser>> GetCourseUsers(string courseId, Dictionary<string, object> licenseSettings);
        Task<BridgeApiUser> GetUserProfile(string uuid, Dictionary<string, object> licenseSettings);
        Task<List<BridgeApiUser>> GetUsersById(IEnumerable<string> ids, Dictionary<string, object> licenseSettings);

        Task<T> GetRestCall<T>(string basicHeader, string apiUrl, string relativeUrl, HttpMethod httpMethod = null,
            string jsonData = null);

        Task<IEnumerable<LiveSessionResponse>> ListSessions(string courseId, Dictionary<string, object> licenseSettings);
        Task<LiveSessionResponse> GetSession(string courseId, string sessionId, Dictionary<string, object> licenseSettings);
        Task<IEnumerable<LiveSessionResponse>> CreateSessions(string courseId, IEnumerable<LiveSessionRequest> sessions,
            Dictionary<string, object> licenseSettings);

        Task<LiveSessionResponse> UpdateSession(string courseId, int sessionId, LiveSessionRequest session,
            Dictionary<string, object> licenseSettings);

        Task<bool> DeleteSession(string courseId, string sessionId, Dictionary<string, object> licenseSettings);

        Task<IEnumerable<WebConferenceResponse>> UpdateSessionWebconference(string courseId, int sessionId,
            WebConferenceRequest conference, Dictionary<string, object> licenseSettingse);

        Task<bool> RegisterUserToSession(int sessionId, int userId, Dictionary<string, object> licenseSettings);

        Task<IEnumerable<LiveSessionResponse>> PublishSession(string courseId, int sessionId, Dictionary<string, object> licenseSettings);
    }
}