namespace Esynctraining.PdfProcessor.Actions
{
    using System;
    using System.Drawing.Imaging;
    using Esynctraining.PdfProcessor.Actions.Filters;
    using Esynctraining.PdfProcessor.Renders;
    using iTextSharp.text.pdf;

    /// <summary>
    ///     The render page action.
    /// </summary>
    public class RenderPageAction : FilteredPdfAction, IDisposable
    {
        #region Static Fields

        /// <summary>
        ///     The EST page rendered.
        /// </summary>
        public static readonly PdfName EstPageRendered = new PdfName("EstPageRendered");

        #endregion

        #region Fields

        /// <summary>
        ///     The render.
        /// </summary>
        private readonly IPdfRender render;

        /// <summary>
        ///     Track whether Dispose has been called..
        /// </summary>
        private bool disposed;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RenderPageAction"/> class.
        /// </summary>
        /// <param name="reader">
        /// The reader.
        /// </param>
        /// <param name="stamper">
        /// The stamper.
        /// </param>
        /// <param name="render">
        /// The render.
        /// </param>
        public RenderPageAction(PdfReader reader, PdfStamper stamper, IPdfRender render)
            : base(reader, stamper)
        {
            this.render = render;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RenderPageAction"/> class.
        /// </summary>
        /// <param name="reader">
        /// The reader.
        /// </param>
        /// <param name="stamper">
        /// The stamper.
        /// </param>
        /// <param name="filter">
        /// The filter.
        /// </param>
        /// <param name="render">
        /// The render.
        /// </param>
        public RenderPageAction(PdfReader reader, PdfStamper stamper, PdfFilter filter, IPdfRender render)
            : base(reader, stamper, filter)
        {
            this.render = render;
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="RenderPageAction"/> class. 
        /// </summary>
        ~RenderPageAction()
        {
            this.Dispose(false);
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The dispose.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
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
                    this.render.Dispose();
                }

                this.disposed = true;
            }
        }

        /// <summary>
        /// The execute filtered.
        /// </summary>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        protected override bool ExecuteFiltered(int? page)
        {
            if (!page.HasValue)
            {
                return false;
            }

            var pageDict = this.Reader.GetPageNRelease(page.Value);
            var b = pageDict.GetAsBoolean(EstPageRendered);
            if (b != null && b.BooleanValue)
            {
                return false;
            }

            pageDict.Put(EstPageRendered, new PdfBoolean(true));
            pageDict.Put(PdfName.RESOURCES, new PdfDictionary());
            var canvas = this.Stamper.GetOverContent(page.Value);
            var img = this.render.RenderPdfPage(page.Value);
            var cropBox = this.Reader.GetCropBox(page.Value);
            var pdfImg = iTextSharp.text.Image.GetInstance(img, ImageFormat.Png);
            pdfImg.ScaleToFit(cropBox.Width, cropBox.Height);
            pdfImg.RotationDegrees = 360 - this.Reader.GetPageRotation(page.Value);
            pdfImg.SetAbsolutePosition(cropBox.Left, cropBox.Bottom);
            canvas.AddImage(pdfImg);

            pageDict.Put(PdfName.CONTENTS, new PdfArray());
            return true;
        }

        #endregion
    }
}