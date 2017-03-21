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
        LmsUserDTO UpdateUser(
            ILmsLicense lmsCompany,
            IAdobeConnectProxy provider,
            LtiParamDTO param,
            LmsUserDTO user,
            int meetingId,
            out string error,
            bool skipReturningUsers = false);
        
    }

}

