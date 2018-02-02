using System;
using System.Collections.Generic;
using Esynctraining.Core.Logging;
using EdugameCloud.Lti.API.AgilixBuzz;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.DTO;
using EdugameCloud.Lti.API;
using Esynctraining.Core.Domain;
using System.Threading.Tasks;

namespace EdugameCloud.Lti.AgilixBuzz
{
    public class AgilixBuzzLmsUserService : LmsUserServiceBase
    {
        private readonly IAgilixBuzzApi _dlapApi;


        public AgilixBuzzLmsUserService(ILogger logger, IAgilixBuzzApi dlapApi) : base(logger)
        {
            _dlapApi = dlapApi ?? throw new ArgumentNullException(nameof(dlapApi));
        }


        public override async Task<OperationResultWithData<List<LmsUserDTO>>> GetUsers(ILmsLicense lmsCompany,
            int courseId, LtiParamDTO extraData = null)
        {
            if (lmsCompany == null)
                throw new ArgumentNullException(nameof(lmsCompany));
            
            var users = await GetUsersOldStyle(lmsCompany, courseId, extraData);
            return users.users.ToSuccessResult();
        }

        public override async Task<(List<LmsUserDTO> users, string error)> GetUsersOldStyle(ILmsLicense lmsCompany,
            int courseId, LtiParamDTO param = null)
        {
            if (lmsCompany == null)
                throw new ArgumentNullException(nameof(lmsCompany));

            var (users, error) = await _dlapApi.GetUsersForCourseAsync(lmsCompany, courseId, param);

            if (!string.IsNullOrWhiteSpace(error))
                Logger.Error("[AgilixBuzz.dlapApi.GetUsersForCourse] error:" + error);
            
            return (GroupUsers(users), error);
        }

    }

}