namespace EdugameCloud.MVC.Social.OAuth.Instagram
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// The Instagram auth response.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here.")]
    public class InstagramAuthResponse
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the access_token.
        /// </summary>
        public string access_token { get; set; }

        /// <summary>
        /// Gets or sets the user.
        /// </summary>
        public InstagramUser user { get; set; }

        #endregion
    }
}