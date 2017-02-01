using System;
using Esynctraining.AC.Provider;
using Esynctraining.Core.Logging;

namespace EdugameCloud.Lti.API.AdobeConnect
{
    internal sealed class AdobeConnectProxy : Esynctraining.AdobeConnect.AdobeConnectProxy, IAdobeConnectProxy
    {
        public AdobeConnectProxy(AdobeConnectProvider provider, ILogger logger, Uri acRootUrl, string principalId)
            : base(provider, logger, acRootUrl)
        {
            //PrincipalId = principalId;
        }

        //public string PrincipalId { get; private set; }
        
    }

}
