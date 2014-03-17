namespace Esynctraining.PdfProcessor
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;

    using Esynctraining.PdfProcessor.Actions;
    using Esynctraining.PdfProcessor.Actions.Filters;
    using Esynctraining.PdfProcessor.Renders;

    using iTextSharp.text.pdf;

    /// <summary>
    /// The PDF processor helper.
    /// </summary>
    public class PdfProcessorHelper
    {
        #region Public Methods and Operators

        /// <summary>
        /// The check if document was scanned and fix if necessary.
        /// </summary>
        /// <param name="path">
        /// The path.
        /// </param>
        /// <param name="resolution">
        /// The resolution.
        /// </param>
        public void CheckIfDocumentWasScannedAndFixIfNecessary(string path, int resolution = 150)
        {
            this.RenderSpecialCsPagesWithGsNet(path, path, resolution);
        }

        /// <summary>
        /// The check if document was scanned and fix if necessary.
        /// </summary>
        /// <param name="inputStream">
        /// The input stream.
        /// </param>
        /// <param name="resolution">
        /// The resolution.
        /// </param>
        /// <returns>
        /// The <see cref="MemoryStream"/>.
        /// </returns>
        public byte[] CheckIfDocumentWasScannedAndFixIfNecessary(byte[] inputStream, int resolution = 150)
        {
            byte[] output;
            if (this.RenderSpecialCsPagesWithGsNet(inputStream, out output, resolution))
            {
                return output;
            }

            return inputStream;
        }

        /// <summary>
        /// The check if document was scanned and fix if necessary.
        /// </summary>
        /// <param name="inputStream">
        /// The input stream.
        /// </param>
        /// <param name="resolution">
        /// The resolution.
        /// </param>
        /// <returns>
        /// The <see cref="MemoryStream"/>.
        /// </returns>
        public MemoryStream CheckIfDocumentWasScannedAndFixIfNecessary(MemoryStream inputStream, int resolution = 150)
        {
            var originalPosition = inputStream.Position;
            byte[] output;
            if (this.RenderSpecialCsPagesWithGsNet(inputStream.ToArray(), out output, resolution))
            {
                return new MemoryStream(output);
            }

            inputStream.Position = originalPosition;
            return inputStream;
        }

        /// <summary>
        /// Checks if the page is scanned and renders it to prevent FlexPaper "black rectangles" artifacts.
        /// </summary>
        /// <param name="inPdfPath">
        /// Input PDF.
        /// </param>
        /// <param name="outPdfPath">
        /// Output PDF in a case if file has been changed.
        /// </param>
        /// <param name="resolution">
        /// The resolution
        /// </param>
        /// <returns>
        /// True is the document has been checked, false otherwise.
        ///     If the document has been changed it is witten to out pdf stream.
        /// </returns>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here.")]
        public bool RenderSpecialCsPagesWithGsNet(string inPdfPath, string outPdfPath, int resolution)
        {
            if (File.Exists(inPdfPath))
            {
                byte[] output;
                var input = File.ReadAllBytes(inPdfPath);
                var pdfWasRendered = this.RenderSpecialCsPagesWithGsNet(input, out output, resolution);
                if (Path.GetFullPath(inPdfPath) == Path.GetFullPath(outPdfPath) && pdfWasRendered)
                {
                    File.WriteAllBytes(outPdfPath, output);
                }
                else
                {
                    File.WriteAllBytes(outPdfPath, pdfWasRendered ? output : input);    
                }
                
                return pdfWasRendered;
            }

            return false;
        }

        /// <summary>
        /// The render scanned pages with GhostScript.Net.
        /// </summary>
        /// <param name="inPdfBuffer">
        /// The in PDF  memory stream.
        /// </param>
        /// <param name="outPdfBuffer">
        /// The out PDF stream (any).
        /// </param>
        /// <param name="resolution">
        /// The resolution.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool RenderSpecialCsPagesWithGsNet(byte[] inPdfBuffer, out byte[] outPdfBuffer, int resolution)
        {
            bool result = false;
            using (var reader = new PdfReader(inPdfBuffer))
                {
                    // Check if document has scanned pages.
                    var andFilter =
                        new AndFilter(new List<PdfFilter>
                                {
                                    new HasSpecialCsResourceFilter(reader),
////                                new NotFilter(new HasFontResourceFilter(reader)),
                                    new NotFilter(new PageHasSpecificKeyFilter(reader, RenderPageAction.EstPageRendered, new PdfBoolean(true)))
                                });
                    var processor = new PdfProcessor(new List<IPdfAction> { andFilter });
                    bool containsSpecialCsPages = false;
                    for (var i = 1; i <= reader.NumberOfPages; i++)
                    {
                        if (processor.Execute(i))
                        {
                            containsSpecialCsPages = true;
                            break;
                        }
                    }

                    // Render the document if it has scanned pages.
                    if (containsSpecialCsPages)
                    {
                        using (var outPdfStream = new MemoryStream())
                        {
                            using (var stamper = new PdfStamper(reader, outPdfStream))
                            {
                                using (var render = new GsNetPdfRender(inPdfBuffer, resolution))
                                {
                                    var action = new RenderPageAction(reader, stamper, andFilter, render);
                                    processor = new PdfProcessor(new List<IPdfAction> { action });
                                    for (var i = 1; i <= reader.NumberOfPages; i++)
                                    {
                                        result |= processor.Execute(i);
                                    }
                                }
                                reader.RemoveUnusedObjects();
                            }
                            outPdfBuffer = outPdfStream.ToArray();
                        }
                    }
                    else
                    {
                        outPdfBuffer = inPdfBuffer;
                    }

                    reader.Close();
                    return result;
                }
        }

        #endregion
    }
}