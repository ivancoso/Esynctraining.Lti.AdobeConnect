using Esynctraining.AC.Provider;
using Esynctraining.Core.Logging;

namespace Esynctraining.AdobeConnect.Tests
{
    public class AdobeConnectProxy : AdobeConnect.AdobeConnectProxy, IAdobeConnectProxy
    {
        public string PrincipalId { get; private set; }

        public AdobeConnectProxy(AdobeConnectProvider provider, ILogger logger, string apiUrl, string principalId)
            : base(provider, logger, apiUrl)
        {
            PrincipalId = principalId;
        }
    }
}