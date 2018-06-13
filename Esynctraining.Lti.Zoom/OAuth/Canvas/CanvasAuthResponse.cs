using System.Diagnostics.CodeAnalysis;

namespace Esynctraining.Lti.Zoom.OAuth.Canvas
{
    /// <summary>
    /// The canvas auth response.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here.")]
    public sealed class CanvasAuthResponse
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the access_token.
        /// </summary>
        public string access_token { get; set; }

        /// <summary>
        /// Gets or sets the user.
        /// </summary>
        public CanvasUser user { get; set; }

        #endregion
    }
}
