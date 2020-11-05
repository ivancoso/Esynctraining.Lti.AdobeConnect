namespace Esynctraining.PdfProcessor.Actions.Filters
{
    using iTextSharp.text.pdf;

    using PdfAction = Esynctraining.PdfProcessor.Actions.PdfAction;

    /// <summary>
    ///     The PDF filter.
    /// </summary>
    public class PdfFilter : PdfAction
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfFilter"/> class.
        /// </summary>
        /// <param name="reader">
        /// The reader.
        /// </param>
        public PdfFilter(PdfReader reader)
            : base(reader, null)
        {
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
            return true;
        }

        #endregion
    }
}