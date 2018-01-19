using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.DTO;
using Esynctraining.Core.Domain;
using Esynctraining.Core.Logging;

namespace EdugameCloud.Lti.API
{
    public class ImsUserService : LmsUserServiceBase
    {
        public ImsUserService(ILogger logger) : base(logger)
        {
        }


        public override Task<OperationResultWithData<List<LmsUserDTO>>> GetUsers(ILmsLicense lmsCompany, int courseId, LtiParamDTO extraData = null)
        {
            if (lmsCompany == null)
                throw new ArgumentNullException(nameof(lmsCompany));

            return Task.FromResult(new List<LmsUserDTO>().ToSuccessResult());
        }

        public override Task<(List<LmsUserDTO> users, string error)> GetUsersOldStyle(ILmsLicense lmsCompany, int courseId, LtiParamDTO param = null)
        {
            if (lmsCompany == null)
                throw new ArgumentNullException(nameof(lmsCompany));

            return Task.FromResult<(List<LmsUserDTO> users, string error)>((new List<LmsUserDTO>(), null));
        }

    }

}