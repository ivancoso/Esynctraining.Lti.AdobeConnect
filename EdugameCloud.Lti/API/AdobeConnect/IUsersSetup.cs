using System;
using System.Threading.Tasks;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.DTO;
using Esynctraining.AdobeConnect;

namespace EdugameCloud.Lti.API.AdobeConnect
{
    public interface IUsersSetup
    {
        Task<(LmsUserDTO user, string error)> UpdateUser(
            ILmsLicense lmsCompany,
            IAdobeConnectProxy provider,
            LtiParamDTO param,
            LmsUserDTO user,
            int meetingId,
            bool skipReturningUsers = false);
        
    }

}

