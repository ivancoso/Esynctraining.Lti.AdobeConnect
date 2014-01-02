namespace Esynctraining.PdfProcessor.Actions.Filters
{
    using System.Linq;
    using iTextSharp.text.pdf;

    /// <summary>
    /// The has special cs resource filter.
    /// </summary>
    public class HasSpecialCsResourceFilter : PdfFilter
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="HasSpecialCsResourceFilter"/> class.
        /// </summary>
        /// <param name="reader">
        /// The reader.
        /// </param>
        public HasSpecialCsResourceFilter(PdfReader reader)
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

            var colorSpace = resources.GetAsDict(PdfName.COLORSPACE);
            if (colorSpace == null)
            {
                return false;
            }

            return
                colorSpace.Keys.Select(colorSpace.GetAsArray)
                    .Any(
                        csArray =>
                        csArray != null && csArray.Size > 0
                        && (PdfName.SEPARATION.Equals(csArray[0]) 
                        || PdfName.DEVICEN.Equals(csArray[0]) 
                        || PdfName.INDEXED.Equals(csArray[0])));
        }

        #endregion
    }
}