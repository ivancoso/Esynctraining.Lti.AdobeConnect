namespace Esynctraining.PdfProcessor.Actions.Filters
{
    using System.Collections.Generic;
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

            var colorSpaces = this.TryToFindColorSpace(resources);
            if (colorSpaces == null || colorSpaces.Count == 0)
            {
                return false;
            }

            return
                colorSpaces.Any(colorSpace => colorSpace.Keys.Select(colorSpace.GetAsArray)
                    .Any(
                        csArray =>
                        csArray != null && csArray.Size > 0
                        && (PdfName.SEPARATION.Equals(csArray[0]) 
                        || PdfName.DEVICEN.Equals(csArray[0]) 
                        || PdfName.INDEXED.Equals(csArray[0]))));
        }

        /// <summary>
        /// The try to find color space.
        /// </summary>
        /// <param name="resources">
        /// The resources.
        /// </param>
        /// <returns>
        /// The <see cref="List{PdfDictionary}"/>.
        /// </returns>
        private List<PdfDictionary> TryToFindColorSpace(PdfDictionary resources)
        {
            var colorSpaces = new List<PdfDictionary>();
            PdfDictionary colorSpace;

            if (resources == null)
            {
                return colorSpaces;
            }
            
            if ((colorSpace = resources.GetAsDict(PdfName.COLORSPACE)) != null)
            {
                colorSpaces.Add(colorSpace);
            }

            var @object = resources.GetAsDict(PdfName.XOBJECT);

            if (@object == null || @object.Length == 0)
            {
                return colorSpaces;
            }

            var directObjects = @object.Keys.Select(@object.GetAsStream).ToList();

            foreach (var stream in directObjects)
            {
                var type = stream.Get(PdfName.SUBTYPE);
                if (type != null)
                {
                    if (type.Equals(PdfName.IMAGE) || type.Equals(PdfName.IMAGEB) || type.Equals(PdfName.IMAGEC)
                         || type.Equals(PdfName.IMAGEI) || type.Equals(PdfName.IMAGEMASK))
                    {
                        colorSpace = stream.GetAsDict(PdfName.COLORSPACE);
                        if (colorSpace != null)
                        {
                            colorSpaces.Add(colorSpace);
                        }
                    }
                    else if (type.Equals(PdfName.FORM))
                    {
                        var formResources = stream.GetAsDict(PdfName.RESOURCES);
                        colorSpaces.AddRange(this.TryToFindColorSpace(formResources));
                    }
                }
            }

            return colorSpaces;
        }

        #endregion
    }
}