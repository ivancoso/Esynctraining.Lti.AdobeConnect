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

        /// <summary>
        /// The date format.
        /// </summary>
        public const string DateFormat = @"yyyy-MM-dd\THH:mm:sszzz";

        /// <summary>
        /// The http client timeout for http requests
        /// </summary>
        public const int DefaultHttpRequestTimeout = 2000 * 60; //2 min

        /// <summary>
        /// The http client timeout for content download/upload requests
        /// </summary>
        public const int DefaultHttpContentRequestTimeout = 10000 * 60; //10 min


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
