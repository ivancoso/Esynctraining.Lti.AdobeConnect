namespace EdugameCloud.MVC.Social.OAuth
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// The Instagram user data.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here.")]
    public class InstagramUserData
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        public InstagramUser data { get; set; }

        #endregion
    }
}