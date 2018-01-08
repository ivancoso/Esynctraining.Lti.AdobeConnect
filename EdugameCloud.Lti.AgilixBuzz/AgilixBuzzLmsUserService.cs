using System;
using System.Collections.Generic;
using Esynctraining.Core.Logging;
using EdugameCloud.Lti.API.AgilixBuzz;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.DTO;
using EdugameCloud.Lti.API;
using Esynctraining.Core.Domain;

namespace EdugameCloud.Lti.AgilixBuzz
{
    public class AgilixBuzzLmsUserService : LmsUserServiceBase
    {
        private readonly IAgilixBuzzApi _dlapApi;


        public AgilixBuzzLmsUserService(ILogger logger, IAgilixBuzzApi dlapApi) : base(logger)
        {
            _dlapApi = dlapApi ?? throw new ArgumentNullException(nameof(dlapApi));
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

            List<LmsUserDTO> users = _dlapApi.GetUsersForCourse(
                lmsCompany,
                courseId,
                out error,
                param);

            if (!string.IsNullOrWhiteSpace(error))
                Logger.Error("[AgilixBuzz.dlapApi.GetUsersForCourse] error:" + error);
            
            return GroupUsers(users);
        }

    }

}