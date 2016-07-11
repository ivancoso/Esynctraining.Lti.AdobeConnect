namespace EdugameCloud.MVC.Social.OAuth
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    using DotNetOpenAuth.AspNet.Clients;

    /// <summary>
    /// The Twitter client.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here.")]
    public class TwitterClient2 : DotNetOpenAuth.AspNet.Clients.TwitterClient
    {
        /// <summary>
        /// The token manager.
        /// </summary>
        public static TwitterClientTokenManager TokenManager = new TwitterClientTokenManager();

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TwitterClient2"/> class.
        /// </summary>
        /// <param name="appId">
        /// The app id.
        /// </param>
        /// <param name="appSecret">
        /// The app secret.
        /// </param>
        public TwitterClient2(string appId, string appSecret)
            : base(appId, appSecret, TokenManager)
        {
        }

        #endregion
    }

    /// <summary>
    /// The twitter client token manager.
    /// </summary>
    public class TwitterClientTokenManager : IOAuthTokenManager
    {
        /// <summary>
        /// The cache.
        /// </summary>
        private IDictionary<string, string> cache = new Dictionary<string, string>();

        /// <summary>
        /// The get token secret.
        /// </summary>
        /// <param name="token">
        /// The token.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string GetTokenSecret(string token)
        {
            if (this.cache.ContainsKey(token))
            {
                return this.cache[token];
            }

            return string.Empty;
        }

        /// <summary>
        /// The store request token.
        /// </summary>
        /// <param name="requestToken">
        /// The request token.
        /// </param>
        /// <param name="requestTokenSecret">
        /// The request token secret.
        /// </param>
        public void StoreRequestToken(string requestToken, string requestTokenSecret)
        {
            if (!this.cache.ContainsKey(requestToken))
            {
                this.cache.Add(requestToken, requestTokenSecret);
            }
        }

        /// <summary>
        /// The replace request token with access token.
        /// </summary>
        /// <param name="requestToken">
        /// The request token.
        /// </param>
        /// <param name="accessToken">
        /// The access token.
        /// </param>
        /// <param name="accessTokenSecret">
        /// The access token secret.
        /// </param>
        public void ReplaceRequestTokenWithAccessToken(
            string requestToken,
            string accessToken,
            string accessTokenSecret)
        {
            if (this.cache.ContainsKey(requestToken))
            {
                this.cache.Remove(requestToken);
            }

            this.StoreRequestToken(accessToken, accessTokenSecret);
        }
    }
}