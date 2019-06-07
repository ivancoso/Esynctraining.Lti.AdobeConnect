using System;
using System.Reflection;
using System.Threading.Tasks;
using Esynctraining.Pdf.Common;

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
        private const string Resources = "Esynctraining.PdfProcessor.Resources.";

        private PdfProcessorSettings _settings;

        public static (string GhostScriptDllPath, Version GhostScriptVersion) SetupGhostScriptFromEmbeddedResources(string workingDir = null)
        {
            workingDir = string.IsNullOrWhiteSpace(workingDir) ? Directory.GetCurrentDirectory() : workingDir;

            var gsLibName = Environment.Is64BitProcess ? "gsdll64.dll" : "gsdll32.dll";
            var gsVersion = "9.18"; 

            string ghostScriptNativeDllPath = $"{workingDir}\\{gsLibName}";
            ResourceHelper.FlushResourceToFile(Assembly.GetAssembly(typeof(PdfProcessorHelper)), Resources + gsLibName,
                ghostScriptNativeDllPath);

            return (
                GhostScriptDllPath : ghostScriptNativeDllPath,
                GhostScriptVersion : Version.Parse(gsVersion)
            );
        }

        public PdfProcessorHelper(PdfProcessorSettings settings)
        {
            if (!settings.SearchForGhosts)
            {
                if (string.IsNullOrWhiteSpace(settings.GhostScriptDllPath) || !File.Exists(settings.GhostScriptDllPath))
                {
                    throw new ArgumentOutOfRangeException(nameof(settings), "GhostScriptDllPath is missing or doesn't exist");
                }
            }

            _settings = settings;
        }

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
        /// <param name="path">
        /// The path.
        /// </param>
        /// <param name="resolution">
        /// The resolution.
        /// </param>
        public async Task CheckIfDocumentWasScannedAndFixIfNecessaryAsync(string path, int resolution = 150)
        {
            await this.RenderSpecialCsPagesWithGsNetAsync(path, path, resolution);
        }

        /// <summary>
        /// The check if document was scanned and fix if necessary.
        /// </summary>
        /// <param name="inPath">
        /// The input.
        /// </param>
        /// /// <param name="outPath">
        /// The output.
        /// </param>
        /// <param name="resolution">
        /// The resolution.
        /// </param>
        public async Task CheckIfDocumentWasScannedAndFixIfNecessaryAsync(string inPath, string outPath, int resolution = 150)
        {
            await this.RenderSpecialCsPagesWithGsNetAsync(inPath, outPath, resolution);
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
        public async Task<bool> RenderSpecialCsPagesWithGsNetAsync(string inPdfPath, string outPdfPath, int resolution)
        {
            if (File.Exists(inPdfPath))
            {
                byte[] output;
                var input = File.ReadAllBytes(inPdfPath);
                var pdfWasRendered = this.RenderSpecialCsPagesWithGsNet(input, out output, resolution);
                if (Path.GetFullPath(inPdfPath) == Path.GetFullPath(outPdfPath) && pdfWasRendered)
                {
                    await WriteAllBytesAsync(outPdfPath, output);
                }
                else
                {
                    await WriteAllBytesAsync(outPdfPath, pdfWasRendered ? output : input);    
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
            PdfReader.unethicalreading = true;
            using (var reader = new PdfReader(inPdfBuffer))
                {
                    // Check if document has scanned pages.
                    var andFilter =
                        new AndFilter(new List<PdfFilter>
                                {
                                    new HasSpecialCsResourceFilter(reader),
                                    // removed the comment out here to make all unit tests working as designed by the author
                                    new NotFilter(new HasFontResourceFilter(reader)),
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
                                using (var render = CreateGsNetPdfRender(inPdfBuffer, resolution))
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

        #region Private Methods

        private async Task WriteAllBytesAsync(string filePath, byte[] bytes)
        {
            using (FileStream sourceStream = new FileStream(filePath,
                FileMode.Append, FileAccess.Write, FileShare.None,
                bufferSize: 4096, useAsync: true))
            {
                await sourceStream.WriteAsync(bytes, 0, bytes.Length);
            };
        }

        #endregion

        private GsNetPdfRender CreateGsNetPdfRender(byte[] inBuffer, int resolution)
        {
            if (_settings.SearchForGhosts)
            {
                //this will search the registry for system installed GhostScript
                return new GsNetPdfRender(inBuffer, resolution);
            }

            //this way we are pointing the wrapper to the dll 
            return new GsNetPdfRender(inBuffer, 150, _settings.GhostScriptVersion, _settings.GhostScriptDllPath,_settings.GhostScriptLibPath);
        }

        private GsNetPdfRender CreateGsNetPdfRender(string inFile, int resolution)
        {
            if (_settings.SearchForGhosts)
            {
                //this will search the registry for system installed GhostScript
                return new GsNetPdfRender(inFile, resolution);
            }

            //this way we are pointing the wrapper to the dll 
            return new GsNetPdfRender(inFile, 150, _settings.GhostScriptVersion, _settings.GhostScriptDllPath,
                _settings.GhostScriptLibPath);

        }
    }
}