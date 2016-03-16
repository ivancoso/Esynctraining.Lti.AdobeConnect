using Esynctraining.AC.Provider;
using Esynctraining.Core.Logging;

namespace EdugameCloud.Lti.API.AdobeConnect
{
    internal sealed class AdobeConnectProxy : Esynctraining.AdobeConnect.AdobeConnectProxy, IAdobeConnectProxy
    {
        public AdobeConnectProxy(AdobeConnectProvider provider, ILogger logger, string apiUrl, string principalId)
            : base(provider, logger, apiUrl)
        {
            PrincipalId = principalId;
        }

        public string PrincipalId { get; private set; }
        
    }

}
