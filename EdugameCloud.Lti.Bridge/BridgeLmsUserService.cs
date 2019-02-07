using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Esynctraining.Core.Domain;
using Esynctraining.Core.Logging;
using Esynctraining.Lti.Lms.Common.API;
using Esynctraining.Lti.Lms.Common.Dto;

namespace EdugameCloud.Lti.Bridge
{
    public class BridgeLmsUserService : LmsUserServiceBase
    {
        private readonly IBridgeApi api;
        public BridgeLmsUserService(ILogger logger, IBridgeApi api) : base(logger)
        {
            this.api = api;
        }

        public override async Task<OperationResultWithData<LmsUserDTO>> GetUser(Dictionary<string, object> licenseSettings, string lmsUserId, string courseId, ILtiUserListParam extraData = null)
        {
            var result = await api.GetUserProfile(lmsUserId, licenseSettings);
            return new LmsUserDTO
            {
                Email = result?.Email,
                Id = result?.Id.ToString(),
                LmsRole = result?.Roles?.FirstOrDefault(),
                Name = result?.Name,
                LtiId = result?.Id.ToString(),
                Login = result?.Uid
            }.ToSuccessResult();
        }


        public override async Task<OperationResultWithData<List<LmsUserDTO>>> GetUsers(Dictionary<string, object> licenseSettings,
            string courseId, ILtiUserListParam param = null)
        {
            var res = await api.GetCourseUsers(courseId, licenseSettings);
            var users = res.Select(result => new LmsUserDTO()
            {
                Email = result?.Email,
                Id = result?.Id.ToString(),
                LmsRole = result?.Roles?.FirstOrDefault(),
                Name = result?.Name,
                LtiId = result?.Id.ToString(),
                Login = result?.Uid
            });
            return users.ToList().ToSuccessResult();
        }
    }
}