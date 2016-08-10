using System;

namespace Esynctraining.AC.Provider.DataObjects
{
    /// <summary>
    /// Connection Details for Adobe Connect API
    /// </summary>
    public class ConnectionDetails
    {
        public string ServiceUrl { get; private set; }
        
        public ProxyCredentials Proxy { get; set; }


        public ConnectionDetails(string serviceUrl)
        {
            if (string.IsNullOrWhiteSpace(serviceUrl))
                throw new ArgumentException("Non-empty value expected", nameof(serviceUrl));

            ServiceUrl = serviceUrl;
        }

    }

}
