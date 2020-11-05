using System;
using Esynctraining.AC.Provider.Constants;

namespace Esynctraining.AC.Provider.DataObjects
{
    /// <summary>
    /// Connection Details for Adobe Connect API
    /// </summary>
    public class ConnectionDetails
    {
        public Uri AdobeConnectRoot { get; }

        public int HttpRequestTimeout { get; }

        public int HttpContentRequestTimeout { get; }
        

        public ConnectionDetails(Uri adobeConnectRoot,
            int requestTimeout = AdobeConnectProviderConstants.DefaultHttpRequestTimeout,
            int contentTimout = AdobeConnectProviderConstants.DefaultHttpContentRequestTimeout)
        {
            if (adobeConnectRoot == null)
                throw new ArgumentNullException(nameof(adobeConnectRoot));
            if (!adobeConnectRoot.IsAbsoluteUri)
                throw new ArgumentException("Absolute Uri expected", nameof(adobeConnectRoot));
            if (!adobeConnectRoot.Scheme.Equals("HTTPS", StringComparison.OrdinalIgnoreCase) && !adobeConnectRoot.Scheme.Equals("HTTP", StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException($"HTTP and HTTPS only", nameof(adobeConnectRoot));


            AdobeConnectRoot = adobeConnectRoot;
            HttpRequestTimeout = requestTimeout;
            HttpContentRequestTimeout = contentTimout;
        }

    }

}
