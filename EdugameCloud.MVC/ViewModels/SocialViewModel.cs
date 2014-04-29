namespace EdugameCloud.MVC.ViewModels
{
    /// <summary>
    /// The social view model.
    /// </summary>
    public class SocialViewModel : HomeViewModel
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the access token.
        /// </summary>
        public string AccessToken { get; set; }

        /// <summary>
        /// Gets or sets the provider.
        /// </summary>
        public string Provider { get; set; }

        /// <summary>
        /// Gets or sets the user name.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets the access token secret.
        /// </summary>
        public string AccessTokenSecret { get; set; }

        #endregion
    }
}