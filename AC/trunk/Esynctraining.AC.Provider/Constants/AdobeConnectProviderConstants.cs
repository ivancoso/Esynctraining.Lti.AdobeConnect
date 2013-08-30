// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AdobeConnectProviderConstants.cs" company="eSyncTraining">
//   eSyncTraining
// </copyright>
// <summary>
//   The adobe connect provider constants.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

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
        /// The config string service url.
        /// </summary>
        public const string ConfigStringServiceUrl = "AdobeConnectProvider_ServiceUrl";

        /// <summary>
        /// The config string event max participants.
        /// </summary>
        public const string ConfigStringEventMaxParticipants = "AdobeConnectProvider_EventMaxParticipants";

        /// <summary>
        /// The config string proxy url.
        /// </summary>
        public const string ConfigStringProxyUrl = "AdobeConnectProvider_ProxyUrl";

        /// <summary>
        /// The config string proxy domain.
        /// </summary>
        public const string ConfigStringProxyDomain = "AdobeConnectProvider_ProxyDomain";

        /// <summary>
        /// The config string proxy login.
        /// </summary>
        public const string ConfigStringProxyLogin = "AdobeConnectProvider_ProxyLogin";

        /// <summary>
        /// The config string proxy password.
        /// </summary>
        public const string ConfigStringProxyPassword = "AdobeConnectProvider_ProxyPassword";

        /// <summary>
        /// The date format.
        /// </summary>
        public const string DateFormat = @"yyyy-MM-dd\THH:mm:sszzz";
    }
}
