using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.DTO;
using Esynctraining.AdobeConnect;

namespace EdugameCloud.Lti.API.AdobeConnect
{
    /// <summary>
    /// The UsersSetup interface.
    /// </summary>
    public interface IUsersSetup
    {
        void SetLMSUserDefaultACPermissions(
            IAdobeConnectProxy provider,
            LmsCompany lmsCompany,
            string meetingScoId,
            LmsUserDTO user,
            string principalId);

        LmsUserDTO UpdateUser(
            LmsCompany lmsCompany,
            IAdobeConnectProxy provider,
            LtiParamDTO param,
            LmsUserDTO user,
            int meetingId,
            out string error,
            bool skipReturningUsers = false);
        
    }

}

