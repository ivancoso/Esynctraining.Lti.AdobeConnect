namespace Esynctraining.PdfProcessor.Renders
{
    using System;
    using System.Drawing;
    using System.IO;

    using Ghostscript.NET;
    using Ghostscript.NET.Rasterizer;

    /// <summary>
    ///     The GhostScript.NET PDF render.
    /// </summary>
    public class GsNetPdfRender : IPdfRender
    {
        #region Fields

        /// <summary>
        ///     The rasterizer.
        /// </summary>
        private readonly GhostscriptRasterizer rasterizer;

        /// <summary>
        ///     The resolution.
        /// </summary>
        private readonly int resolution;

        /// <summary>
        /// Track whether Dispose has been called..
        /// </summary>
        private bool disposed;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="GsNetPdfRender"/> class.
        /// </summary>
        /// <param name="pdfStream">
        /// The PDF stream.
        /// </param>
        /// <param name="resolution">
        /// The resolution.
        /// </param>
        public GsNetPdfRender(byte[] pdfStream, int resolution)
        {
            // ReSharper disable once BitwiseOperatorOnEnumWithoutFlags
            GhostscriptVersionInfo lastInstalledVersion =
                GhostscriptVersionInfo.GetLastInstalledVersion(
                    GhostscriptLicense.GPL | GhostscriptLicense.AFPL, 
                    GhostscriptLicense.GPL);
            this.rasterizer = new GhostscriptRasterizer();
            this.rasterizer.Open(new MemoryStream(pdfStream), lastInstalledVersion, false);
            this.resolution = resolution;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GsNetPdfRender"/> class.
        /// </summary>
        /// <param name="pdfFile">
        /// The PDF file.
        /// </param>
        /// <param name="resolution">
        /// The resolution.
        /// </param>
        public GsNetPdfRender(string pdfFile, int resolution)
        {
            // ReSharper disable once BitwiseOperatorOnEnumWithoutFlags
            GhostscriptVersionInfo lastInstalledVersion =
                GhostscriptVersionInfo.GetLastInstalledVersion(
                    GhostscriptLicense.GPL | GhostscriptLicense.AFPL, 
                    GhostscriptLicense.GPL);
            this.rasterizer = new GhostscriptRasterizer();
            this.rasterizer.Open(pdfFile, lastInstalledVersion, true);
            this.resolution = resolution;
        }

        /// <summary>
        ///     Finalizes an instance of the <see cref="GsNetPdfRender" /> class.
        /// </summary>
        ~GsNetPdfRender()
        {
            this.Dispose(false);
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The dispose.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// The render PDF page.
        /// </summary>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <returns>
        /// The <see cref="Image"/>.
        /// </returns>
        public virtual Image RenderPdfPage(int page)
        {
            return this.rasterizer.GetPage(this.resolution, this.resolution, page);
        }

        #endregion

        #region Methods

        /// <summary>
        /// The dispose.
        /// </summary>
        /// <param name="disposing">
        /// The disposing.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    this.rasterizer.Close();
                    this.rasterizer.Dispose();
                }

                this.disposed = true;
            }
        }

        #endregion
    }
}