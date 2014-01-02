namespace Esynctraining.PdfProcessor.Actions
{
    using Esynctraining.PdfProcessor.Actions.Filters;

    using iTextSharp.text.pdf;

    /// <summary>
    /// The filtered PDF action.
    /// </summary>
    public abstract class FilteredPdfAction : PdfAction
    {
        #region Fields

        /// <summary>
        /// The filter.
        /// </summary>
        private readonly PdfFilter filter;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FilteredPdfAction"/> class.
        /// </summary>
        /// <param name="reader">
        /// The reader.
        /// </param>
        /// <param name="stamper">
        /// The stamper.
        /// </param>
        public FilteredPdfAction(PdfReader reader, PdfStamper stamper)
            : base(reader, stamper)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FilteredPdfAction"/> class.
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
        public FilteredPdfAction(PdfReader reader, PdfStamper stamper, PdfFilter filter)
            : base(reader, stamper)
        {
            this.filter = filter;
        }

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
        public override bool Execute(int? page)
        {
            if (this.filter != null)
            {
                if (this.filter.Execute(page))
                {
                    return this.ExecuteFiltered(page);
                }

                return false;
            }

            return this.ExecuteFiltered(page);
        }

        #endregion

        #region Methods

        /// <summary>
        /// The execute filtered.
        /// </summary>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        protected abstract bool ExecuteFiltered(int? page);

        #endregion
    }
}