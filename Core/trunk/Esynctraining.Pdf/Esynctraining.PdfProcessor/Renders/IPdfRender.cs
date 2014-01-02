namespace Esynctraining.PdfProcessor.Renders
{
    using System;
    using System.Drawing;

    /// <summary>
    /// The PDF render interface.
    /// </summary>
    public interface IPdfRender : IDisposable
    {
        #region Public Methods and Operators

        /// <summary>
        /// The render PDF page.
        /// </summary>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <returns>
        /// The <see cref="Image"/>.
        /// </returns>
        Image RenderPdfPage(int page);

        #endregion
    }
}