namespace Esynctraining.AC.Provider.Constants
{
    /// <summary>
    /// The adobe connect provider constants.
    /// </summary>
    public static class AdobeConnectProviderConstants
    {
        /// <summary>
        /// The session cookie name.
        /// </summary>
        public const string SessionCookieName = "BREEZESESSION";

        /// <summary>
        /// The default event max participants.
        /// </summary>
        public const int DefaultEventMaxParticipants = 100;

        // not longer used
        //public const string ConfigStringServiceUrl = "AdobeConnectProvider_ServiceUrl";
        //public const string ConfigStringEventMaxParticipants = "AdobeConnectProvider_EventMaxParticipants";
        //public const string ConfigStringProxyUrl = "AdobeConnectProvider_ProxyUrl";
        //public const string ConfigStringProxyDomain = "AdobeConnectProvider_ProxyDomain";
        //public const string ConfigStringProxyLogin = "AdobeConnectProvider_ProxyLogin";
        //public const string ConfigStringProxyPassword = "AdobeConnectProvider_ProxyPassword";

        /// <summary>
        /// The date format.
        /// </summary>
        public const string DateFormat = @"yyyy-MM-dd\THH:mm:sszzz";

        /// <summary>
        /// The http client timeout for http requests
        /// </summary>
        public const int DefaultHttpRequestTimeout = 2000*60; //2 min
        
		/// <summary>
        /// The http client timeout for content download/upload requests
        /// </summary>
        public const int DefaultHttpContentRequestTimeout = 10000*60; //10 min

        /// <summary>
        /// The maximum number of returned objects for api calls
        /// </summary>
        public const int MaxOperationSize = 20000;
        
        /// <summary>
        /// The maximum number of returned objects for chunked requests. Not related to api, internal usage only
        /// </summary>
        public const int ChunkOperationSize = 50;
    }

}
