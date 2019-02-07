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

        public override Task<OperationResultWithData<List<LmsUserDTO>>> GetUsers(Dictionary<string, object> licenseSettings, string courseId, ILtiUserListParam extraData = null)
        {
            //if (lmsCompany == null)
            //    throw new ArgumentNullException(nameof(lmsCompany));

            return Task.FromResult(new List<LmsUserDTO>().ToSuccessResult());
        }
    }
}