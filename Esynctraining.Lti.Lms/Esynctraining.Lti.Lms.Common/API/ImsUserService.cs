using System.Collections.Generic;
using System.Threading.Tasks;
using Esynctraining.Core.Domain;
using Esynctraining.Core.Logging;
using Esynctraining.Lti.Lms.Common.Dto;

namespace Esynctraining.Lti.Lms.Common.API
{
    public class ImsUserService : LmsUserServiceBase
    {
        public ImsUserService(ILogger logger) : base(logger)
        {
        }


        public override Task<OperationResultWithData<List<LmsUserDTO>>> GetUsers(Dictionary<string, object> licenseSettings, string courseId, LtiParamDTO extraData = null)
        {
            //if (lmsCompany == null)
            //    throw new ArgumentNullException(nameof(lmsCompany));

            return Task.FromResult(new List<LmsUserDTO>().ToSuccessResult());
        }

        //public override Task<(List<LmsUserDTO> users, string error)> GetUsersOldStyle(Dictionary<string, object> licenseSettings, int courseId, LtiParamDTO param = null)
        //{
        //    //if (lmsCompany == null)
        //    //    throw new ArgumentNullException(nameof(lmsCompany));

        //    return Task.FromResult<(List<LmsUserDTO> users, string error)>((new List<LmsUserDTO>(), null));
        //}

    }

}