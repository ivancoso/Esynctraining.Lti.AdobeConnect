using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EdugameCloud.Lti.API;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.DTO;
using Esynctraining.Core.Domain;
using Esynctraining.Core.Logging;

namespace EdugameCloud.Lti.Bridge
{
    public class BridgeLmsUserService : LmsUserServiceBase
    {
        private readonly IBridgeApi api;
        public BridgeLmsUserService(ILogger logger, IBridgeApi api) : base(logger)
        {
            this.api = api;
        }

        public override async Task<(LmsUserDTO user, string error)> GetUser(ILmsLicense lmsCompany, string lmsUserId, int courseId, LtiParamDTO extraData = null)
        {
            var result = await api.GetUserProfile(lmsUserId, lmsCompany);
            return (new LmsUserDTO
            {
                Email = result?.Email,
                Id = result?.Id,
                LmsRole = result?.Roles?.FirstOrDefault(),
                Name = result?.Name,
                LtiId = result?.Id,
                Login = result?.Uid,
                PrimaryEmail = result?.Email
            }, null);
        }

        public override async Task<OperationResultWithData<List<LmsUserDTO>>> GetUsers(ILmsLicense lmsCompany,
            int courseId, LtiParamDTO extraData = null)
        {
            return (await GetUsersOldStyle(lmsCompany, courseId, extraData)).users.ToSuccessResult();
        }

        public override async Task<(List<LmsUserDTO> users, string error)> GetUsersOldStyle(ILmsLicense lmsCompany,
            int courseId, LtiParamDTO param = null)
        {
            var res = await api.GetCourseUsers(courseId.ToString(), lmsCompany);
            var users = res.Select(result => new LmsUserDTO()
            {
                Email = result?.Email,
                Id = result?.Id,
                LmsRole = result?.Roles?.FirstOrDefault(),
                Name = result?.Name,
                LtiId = result?.Id,
                Login = result?.Uid,
                PrimaryEmail = result?.Email
            });
            return (users.ToList(), null);
        }
    }
}