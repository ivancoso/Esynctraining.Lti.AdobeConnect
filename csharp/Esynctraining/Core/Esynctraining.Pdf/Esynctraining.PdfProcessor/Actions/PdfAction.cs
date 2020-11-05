namespace Esynctraining.PdfProcessor.Actions
{
    using iTextSharp.text.pdf;

    /// <summary>
    ///     The PDF action.
    /// </summary>
    public abstract class PdfAction : IPdfAction
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfAction"/> class.
        /// </summary>
        /// <param name="reader">
        /// The reader.
        /// </param>
        /// <param name="stamper">
        /// The stamper.
        /// </param>
        public PdfAction(PdfReader reader, PdfStamper stamper)
        {
            this.Reader = reader;
            this.Stamper = stamper;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the reader.
        /// </summary>
        protected PdfReader Reader { get; private set; }

        /// <summary>
        /// Gets the stamper.
        /// </summary>
        protected PdfStamper Stamper { get; private set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The execute.
        /// </summary>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public abstract bool Execute(int? page);

        #endregion
    }
}