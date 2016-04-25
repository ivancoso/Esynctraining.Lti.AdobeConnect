using System.Collections.Generic;
using Esynctraining.Core.Logging;
using EdugameCloud.Lti.API.BrainHoney;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.DTO;
using EdugameCloud.Lti.API;
using Esynctraining.Core.Domain;

namespace EdugameCloud.Lti.BrainHoney
{
    public class BrainHoneyLmsUserService : LmsUserServiceBase
    {
        private readonly IBrainHoneyApi dlapApi;


        public BrainHoneyLmsUserService(ILogger logger, IBrainHoneyApi dlapApi) : base(logger)
        {
            this.dlapApi = dlapApi;
        }


        public override OperationResultWithData<List<LmsUserDTO>> GetUsers(LmsCompany lmsCompany,
            LmsUser lmsUser, int courseId, object extraData = null)
        {
            string error;
            var users = GetUsersOldStyle(lmsCompany, lmsUser.UserId, courseId, out error, extraData);
            return OperationResultWithData<List<LmsUserDTO>>.Success(users);
        }

        public override List<LmsUserDTO> GetUsersOldStyle(LmsCompany lmsCompany,
            string lmsUserId, int courseId, out string error, object param = null)
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