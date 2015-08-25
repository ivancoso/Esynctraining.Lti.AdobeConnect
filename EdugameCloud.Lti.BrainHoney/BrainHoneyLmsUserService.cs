using System.Collections.Generic;
using Castle.Core.Logging;
using EdugameCloud.Lti.API;
using EdugameCloud.Lti.API.BrainHoney;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.DTO;

namespace EdugameCloud.Lti.BrainHoney
{
    public class BrainHoneyLmsUserService : LmsUserServiceBase
    {
        private readonly IBrainHoneyApi dlapApi;

        public BrainHoneyLmsUserService(ILogger logger, IBrainHoneyApi dlapApi) : base(logger)
        {
            this.dlapApi = dlapApi;
        }

        public override OperationResult<List<LmsUserDTO>> GetUsers(LmsCompany lmsCompany,
            LmsUser lmsUser, int courseId, object extraData = null, bool forceUpdate = false)
        {
            string error;
            var users = GetUsersOldStyle(lmsCompany, lmsUser.UserId, courseId, out error, forceUpdate, extraData);
            return OperationResult<List<LmsUserDTO>>.Success(users);
        }

        public override List<LmsUserDTO> GetUsersOldStyle(LmsCompany lmsCompany,
            string lmsUserId, int courseId, out string error, bool forceUpdate = false, object param = null)
        {
            List<LmsUserDTO> users = this.dlapApi.GetUsersForCourse(
                lmsCompany,
                courseId,
                out error,
                param);
            return GroupUsers(users);
        }
    }
}