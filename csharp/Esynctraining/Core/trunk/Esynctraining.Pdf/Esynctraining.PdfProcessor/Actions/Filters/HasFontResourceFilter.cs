namespace Esynctraining.PdfProcessor.Actions.Filters
{
    using iTextSharp.text.pdf;

    /// <summary>
    /// The has font resource filter.
    /// </summary>
    public class HasFontResourceFilter : PdfFilter
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="HasFontResourceFilter"/> class.
        /// </summary>
        /// <param name="reader">
        /// The reader.
        /// </param>
        public HasFontResourceFilter(PdfReader reader)
            : base(reader)
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
            if (!page.HasValue)
            {
                return false;
            }

            var pageDict = this.Reader.GetPageNRelease(page.Value);
            var resources = pageDict.GetAsDict(PdfName.RESOURCES);
            if (resources == null)
            {
                return false;
            }

            var font = resources.GetAsDict(PdfName.FONT);
            return font != null;
        }

        #endregion
    }
}