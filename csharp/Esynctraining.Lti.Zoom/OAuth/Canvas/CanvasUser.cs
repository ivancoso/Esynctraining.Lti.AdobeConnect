using System.Diagnostics.CodeAnalysis;

namespace Esynctraining.Lti.Zoom.OAuth.Canvas
{
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here.")]
    public sealed class CanvasUser
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        public string name { get; set; }

    }
}
