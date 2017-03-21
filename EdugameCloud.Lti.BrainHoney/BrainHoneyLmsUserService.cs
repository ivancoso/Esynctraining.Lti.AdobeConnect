using System;
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
            this.dlapApi = dlapApi ?? throw new ArgumentNullException(nameof(dlapApi));
        }


        public override OperationResultWithData<List<LmsUserDTO>> GetUsers(ILmsLicense lmsCompany,
            int courseId, LtiParamDTO extraData = null)
        {
            if (lmsCompany == null)
                throw new ArgumentNullException(nameof(lmsCompany));

            string error;
            var users = GetUsersOldStyle(lmsCompany, courseId, out error, extraData);
            return users.ToSuccessResult();
        }

        public override List<LmsUserDTO> GetUsersOldStyle(ILmsLicense lmsCompany,
            int courseId, out string error, LtiParamDTO param = null)
        {
            if (lmsCompany == null)
                throw new ArgumentNullException(nameof(lmsCompany));

            List<LmsUserDTO> users = this.dlapApi.GetUsersForCourse(
                lmsCompany,
                courseId,
                out error,
                param);
            return GroupUsers(users);
        }

    }

}