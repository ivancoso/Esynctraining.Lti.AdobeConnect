using System;
using Esynctraining.AC.Provider.Constants;

namespace Esynctraining.AC.Provider.DataObjects
{
    /// <summary>
    /// Connection Details for Adobe Connect API
    /// </summary>
    public class ConnectionDetails
    {
        public string ServiceUrl { get; private set; }
        public int HttpRequestTimeout { get; private set; }
        public int HttpContentRequestTimeout { get; private set; }
        
        public ProxyCredentials Proxy { get; set; }


        public ConnectionDetails(string serviceUrl,
            int requestTimeout = AdobeConnectProviderConstants.DefaultHttpRequestTimeout,
            int contentTimout = AdobeConnectProviderConstants.DefaultHttpContentRequestTimeout)
        {
            if (string.IsNullOrWhiteSpace(serviceUrl))
                throw new ArgumentException("Non-empty value expected", nameof(serviceUrl));

            ServiceUrl = serviceUrl;
            HttpRequestTimeout = requestTimeout;
            HttpContentRequestTimeout = contentTimout;
        }

    }

}
