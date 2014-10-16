namespace EdugameCloud.MVC.Social.OAuth
{
    /// <summary>
    /// The twitter request token response.
    /// </summary>
    public class TwitterRequestTokenResponse
    {
        /// <summary>
        /// Gets or sets the oauth_token.
        /// </summary>
        public string oauth_token { get; set; }

        /// <summary>
        /// Gets or sets the oauth_token_secret.
        /// </summary>
        public string oauth_token_secret { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether oauth_callback_confirmed.
        /// </summary>
        public bool oauth_callback_confirmed { get; set; }
    }
}
