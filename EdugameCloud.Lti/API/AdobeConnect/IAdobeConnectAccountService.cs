using EdugameCloud.Lti.Core.DTO;
using Esynctraining.AC.Provider;

namespace EdugameCloud.Lti.API.AdobeConnect
{
    public interface IAdobeConnectAccountService
    {
        ACPasswordPoliciesDTO GetPasswordPolicies(AdobeConnectProvider provider);

    }

}