namespace EdugameCloud.Web
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    using EdugameCloud.MVC.Social.OAuth;

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

            //RegisterTwitterClient((string)settings.TWConsumerKey, (string)settings.TWConsumerSecret);
            RegisterTwitterClient((string)settings.TWConsumerKey, (string)settings.TWConsumerSecret);

            OAuthWebSecurity.RegisterFacebookClient((string)settings.FBAppId, (string)settings.FBAppSecret);

            RegisterInstagramClient((string)settings.InstagramClientId, (string)settings.InstagramClientSecret);
        }

        /// <summary>
        /// The authentication web security register Instagram client.
        /// </summary>
        /// <param name="appId">
        /// The app id.
        /// </param>
        /// <param name="appSecret">
        /// The app secret.
        /// </param>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here.")]
        private static void RegisterInstagramClient(string appId, string appSecret)
        {
            string displayName = "Instagram";
            RegisterInstagramClient(appId, appSecret, displayName);
        }

        /// <summary>
        /// The register Instagram client.
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
        private static void RegisterInstagramClient(string appId, string appSecret, string displayName)
        {
            IDictionary<string, object> extraData = new Dictionary<string, object>();
            RegisterInstagramClient(appId, appSecret, displayName, extraData);
        }

        /// <summary>
        /// The register Instagram client.
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
        private static void RegisterInstagramClient(string appId, string appSecret, string displayName, IDictionary<string, object> extraData)
        {
            OAuthWebSecurity.RegisterClient(new InstagramClient(appId, appSecret), displayName, extraData);
        }

        /// <summary>
        /// The authentication web security register Instagram client.
        /// </summary>
        /// <param name="appId">
        /// The app id.
        /// </param>
        /// <param name="appSecret">
        /// The app secret.
        /// </param>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here.")]
        private static void RegisterTwitterClient(string appId, string appSecret)
        {
            string displayName = "Twitter";
            RegisterTwitterClient(appId, appSecret, displayName);
        }

        /// <summary>
        /// The register Instagram client.
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
        private static void RegisterTwitterClient(string appId, string appSecret, string displayName)
        {
            IDictionary<string, object> extraData = new Dictionary<string, object>();
            RegisterTwitterClient(appId, appSecret, displayName, extraData);
        }

        /// <summary>
        /// The register Instagram client.
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
        private static void RegisterTwitterClient(string appId, string appSecret, string displayName, IDictionary<string, object> extraData)
        {
            OAuthWebSecurity.RegisterClient(new TwitterClient2(appId, appSecret), displayName, extraData);
        }
    }
}
