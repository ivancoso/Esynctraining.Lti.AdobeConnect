using System.Collections.Generic;
using System.Threading.Tasks;
using EdugameCloud.Lti.Domain.Entities;

namespace EdugameCloud.Lti.Bridge
{
    public interface IBridgeApi
    {
        Task<List<BridgeApiUser>> GetCourseUsers(string courseId, ILmsLicense lmsCompany);
        Task<BridgeApiUser> GetUserProfile(string uuid, ILmsLicense lmsCompany);
        Task<List<BridgeApiUser>> GetUsersById(IEnumerable<string> ids, ILmsLicense lmsCompany);
        Task<T> GetRestCall<T>(string clientId, string clientSecret, string relativeUrl);
    }
}