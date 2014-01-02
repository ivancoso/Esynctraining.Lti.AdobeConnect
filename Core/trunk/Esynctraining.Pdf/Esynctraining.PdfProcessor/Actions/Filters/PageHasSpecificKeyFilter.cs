namespace Esynctraining.PdfProcessor.Actions.Filters
{
    using iTextSharp.text.pdf;

    /// <summary>
    /// The page has specific key filter.
    /// </summary>
    public class PageHasSpecificKeyFilter : PdfFilter
    {
        #region Fields

        /// <summary>
        /// The key.
        /// </summary>
        private readonly PdfName key;

        /// <summary>
        /// The value.
        /// </summary>
        private readonly PdfObject value;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PageHasSpecificKeyFilter"/> class.
        /// </summary>
        /// <param name="reader">
        /// The reader.
        /// </param>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        public PageHasSpecificKeyFilter(PdfReader reader, PdfName key, PdfObject value)
            : base(reader)
        {
            this.key = key;
            this.value = value;
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
            if (pageDict == null)
            {
                return false;
            }

            var obj = pageDict.Get(this.key);
            return obj != null && this.value.ToString().Equals(PdfReader.GetPdfObject(obj).ToString());
        }

        #endregion
    }
}