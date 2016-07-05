namespace EdugameCloud.Lti.Host
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    using EdugameCloud.Lti.OAuth.Canvas;
    using Microsoft.Web.WebPages.OAuth;

    /// <summary>
    /// The authentication config.
    /// </summary>
    public static class AuthConfig
    {
        /// <summary>
        /// The register external authentications.
        /// </summary>
        /// <param name="settings">The settings</param>
        public static void RegisterAuth(dynamic settings)
        {
            //// To let users of this site log in using their accounts from other sites such as Microsoft, Facebook, and Twitter,
            //// you must update this site. For more information visit http://go.microsoft.com/fwlink/?LinkID=252166
            
            RegisterCanvasClient((string)settings.CanvasClientId, (string)settings.CanvasClientSecret);
        }
          
           
        /// <summary>
        /// The authentication web security register Canvas client.
        /// </summary>
        /// <param name="appId">
        /// The app id.
        /// </param>
        /// <param name="appSecret">
        /// The app secret.
        /// </param>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here.")]
        private static void RegisterCanvasClient(string appId, string appSecret)
        {
            string displayName = "Canvas";
            RegisterCanvasClient(appId, appSecret, displayName);
        }

        /// <summary>
        /// The register Canvas client.
        /// </summary>
        /// <param name="appId">
        /// The app id.
        /// </param>
        /// <param name="appSecret">
        /// The app secret.
        /// </param>
        /// <param name="displayName">
        /// The display name.
        /// </param>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here.")]
        private static void RegisterCanvasClient(string appId, string appSecret, string displayName)
        {
            IDictionary<string, object> extraData = new Dictionary<string, object>();
            RegisterCanvasClient(appId, appSecret, displayName, extraData);
        }
               
        /// <summary>
        /// The register Canvas client.
        /// </summary>
        /// <param name="appId">
        /// The app id.
        /// </param>
        /// <param name="appSecret">
        /// The app secret.
        /// </param>
        /// <param name="displayName">
        /// The display name.
        /// </param>
        /// <param name="extraData">
        /// The extra data.
        /// </param>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here.")]
        private static void RegisterCanvasClient(string appId, string appSecret, string displayName, IDictionary<string, object> extraData)
        {
            OAuthWebSecurity.RegisterClient(new CanvasClient(appId, appSecret), displayName, extraData);
        }

    }

}
