namespace EdugameCloud.MVC.OAuth
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// The Instagram user.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here.")]
    public class InstagramUser
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the bio.
        /// </summary>
        public string bio { get; set; }

        /// <summary>
        /// Gets or sets the counts.
        /// </summary>
        public InstagramUserCounts counts { get; set; }

        /// <summary>
        /// Gets or sets the full_name.
        /// </summary>
        public string full_name { get; set; }

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// Gets or sets the profile_picture.
        /// </summary>
        public string profile_picture { get; set; }

        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        public string username { get; set; }

        /// <summary>
        /// Gets or sets the website.
        /// </summary>
        public string website { get; set; }

        #endregion
    }
}