using System.Threading.Tasks;
using EdugameCloud.Lti.Domain.Entities;
using Esynctraining.AdobeConnect;
using Esynctraining.Core.Domain;
using Esynctraining.Lti.Lms.Common.Dto;

namespace EdugameCloud.Lti.API.AdobeConnect
{
    public interface IUsersSetup
    {
        Task<OperationResultWithData<LmsUserDTO>> UpdateUser(
            ILmsLicense lmsLicense,
            IAdobeConnectProxy provider,
            LtiParamDTO param,
            LmsUserDTO user,
            int meetingId,
            bool skipReturningUsers = false);
        
    }

}

