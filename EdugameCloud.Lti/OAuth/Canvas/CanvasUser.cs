namespace EdugameCloud.Lti.OAuth.Canvas
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// The Instagram user.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here.")]
    public sealed class CanvasUser
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        public string name { get; set; }

        #endregion
    }
}