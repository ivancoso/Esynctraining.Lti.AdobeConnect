namespace EdugameCloud.MVC.OAuth
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// The Instagram user counts.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here.")]
    public class InstagramUserCounts
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the followed by.
        /// </summary>
        public int followed_by { get; set; }

        /// <summary>
        /// Gets or sets the follows.
        /// </summary>
        public int follows { get; set; }

        /// <summary>
        /// Gets or sets the media.
        /// </summary>
        public int media { get; set; }

        #endregion
    }
}