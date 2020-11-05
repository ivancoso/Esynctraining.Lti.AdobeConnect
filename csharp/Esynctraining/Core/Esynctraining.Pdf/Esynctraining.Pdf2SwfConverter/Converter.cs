using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Esynctraining.Pdf2SwfConverter.Extensions;
using Esynctraining.Pdf2SwfConverter.Model;
using Esynctraining.PdfProcessor;
using iTextSharp.text.pdf;
using Microsoft.Extensions.Logging;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Esynctraining.Pdf2SwfConverter
{
    public class Converter
    {
        private readonly ConverterSettings _settings;
        private readonly PdfProcessorHelper _pdfProcessor;
        private readonly ILogger _logger;


        public Converter(ConverterSettings settings, PdfProcessorHelper pdfProcessor, ILogger logger)
        {
            _settings = settings;
            _pdfProcessor = pdfProcessor;
            _logger = logger;
        }

        #region Public Methods

        public async Task<ConversionStatus> ConvertPdf2Swf(string inPdfFilePath, ConversionOptions options, Func<ConversionState, ConversionStatus> checkResultAfterConversion = null)
        {
            var status = new ConversionStatus(ConversionState.ConversionFailed);
            try
            {
                var inputPdf = new FileInfo(inPdfFilePath);
                if (!inputPdf.Exists)
                {
                    status.Message = "Input file doesn't exist. " + inPdfFilePath;
                    _logger.LogError(status.Message);
                    return status;
                }

                //setting defaults
                var workingDir = options.OutputDirectory ?? inputPdf.DirectoryName;
                _logger.LogDebug($"Starting Conversion for file {inPdfFilePath}. Working dir {workingDir}");
                var optimizedFilePath = Path.Combine(workingDir, options.OutputNamingConventions.BuildOptimizedPdfFileName(inputPdf.Name));
                await OptimizePdfForConverter(inPdfFilePath, optimizedFilePath, status);

                if (status.State == ConversionState.OptimizationFailed)
                {
                    return status;
                }

                int numberOfPages = await GetNumberOfPages(optimizedFilePath);

                if (numberOfPages == 0)
                {
                    var message = $"Critical failure. Can't detect number of pages for pdf: {optimizedFilePath}";
                    _logger.LogCritical(message);
                    return new ConversionStatus(ConversionState.ConversionFailed, message);
                }

                var swfFile = Path.Combine(workingDir, options.OutputNamingConventions.BuildSwfFileName(inputPdf.Name));
                var swfPagedFile = Path.Combine(workingDir, options.OutputNamingConventions.BuildPagedSwfFileName(inputPdf.Name));

                if (options.OverwriteExistingSwfs)
                {
                    CleanFilesBeforeConversion(options, swfFile, swfPagedFile, numberOfPages);
                }

                switch (options.HowToConvert)
                {
                    case ConversionPath.OnePdf2OneSwf:
                        status = await PerformOnePdf2OneSwfConversion(options, optimizedFilePath, swfFile, checkResultAfterConversion);
                        break;
                    case ConversionPath.OnePdf2PerPageSwf:
                        status = await PerformOnePdf2ManyPagesConversion(options, inPdfFilePath, swfPagedFile, numberOfPages, checkResultAfterConversion);
                        break;
                    case ConversionPath.Both:
                        status = await PerformOnePdf2OneSwfConversion(options, optimizedFilePath, swfFile);
                        if (status.State == ConversionState.Converted || status.State == ConversionState.OutputExists)
                        {
                            status = await PerformOnePdf2ManyPagesConversion(options, inPdfFilePath, swfPagedFile, numberOfPages, checkResultAfterConversion, status);
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                status.Message = $"Critical failure. Converting error {ex}";
                status.State = ConversionState.ConversionFailed;
                _logger.LogCritical(status.Message);
                return status;
            }

            _logger.LogDebug($"Finishing Conversion for file {inPdfFilePath}. Status: {status.State}");

            return status;
        }

        #endregion

        #region Private Methods

        private void CleanFilesBeforeConversion(ConversionOptions options, string swfFile, string swfPagedFilePattern, int numberOfPages)
        {
            if (options.HowToConvert == ConversionPath.OnePdf2OneSwf || options.HowToConvert == ConversionPath.Both)
            {
                _logger.LogDebug($"Cleaning swf file {swfFile};");
                SafelyDeleteFile(swfFile);
            }

            if (options.HowToConvert == ConversionPath.OnePdf2PerPageSwf || options.HowToConvert == ConversionPath.Both)
            {
                foreach (var swfPage in GetOutputSwfFileNames(swfPagedFilePattern, numberOfPages))
                {
                    _logger.LogDebug($"Cleaning swf page file {swfPage};");
                    SafelyDeleteFile(swfPage);
                }
            }
        }

        private IEnumerable<string> GetOutputSwfFileNames(string swfPagedFilePattern, int numberOfPages)
        {
            swfPagedFilePattern = swfPagedFilePattern.Replace(Const.FileNamePageNumberPlaceHolder, "{0}");
            if (numberOfPages > 0)
            {
                for (int i = 1; i <= numberOfPages; i++)
                {
                    yield return string.Format(swfPagedFilePattern, i);
                }
            }
        }

        private async Task<ConversionStatus> PerformOnePdf2OneSwfConversion(ConversionOptions options, string optimizedPdfPath,
            string outSwfFilePath,
            Func<ConversionState, ConversionStatus> checkResultAfterConversion = null)
        {
            string args = _settings.SwfToolsCommandPattern
                .Replace("{pdfFile}", optimizedPdfPath)
                .Replace("{swfFile}", outSwfFilePath);

            Func<ConversionState, ConversionStatus> checkResultsAndReturnStatus = null;

            if (checkResultAfterConversion != null)
            {
                checkResultsAndReturnStatus = checkResultAfterConversion;
            }
            else
            {
                checkResultsAndReturnStatus = (state) => DefaultSuccessCheck(state, outSwfFilePath);
            }

            if (!options.OverwriteExistingSwfs 
                && File.Exists(outSwfFilePath))
            {
                return checkResultsAndReturnStatus(ConversionState.OutputExists);
            }

            return await Convert(args, optimizedPdfPath, null, checkResultsAndReturnStatus);
        }

        private async Task<ConversionStatus> PerformOnePdf2ManyPagesConversion(ConversionOptions options, 
            string optimizedPdfPath, 
            string outSwfFileMultiPagePattern, 
            int numberOfPages, 
            Func<ConversionState, ConversionStatus> checkResultAfterConversion = null, ConversionStatus previousStatus = null)
        {
            var args = _settings.SwfToolsCommandPattern
                .Replace("{pdfFile}", optimizedPdfPath)
                .Replace("{swfFile}", outSwfFileMultiPagePattern);
            Func<ConversionState, ConversionStatus> GetSuccessValidator()
            {
                Func<ConversionState, ConversionStatus> result = null;

                if (checkResultAfterConversion != null)
                {
                    result = checkResultAfterConversion;
                }
                else
                {
                    result = state => DefaultMultiPageSuccessCheck(state, outSwfFileMultiPagePattern, numberOfPages);
                }

                return result;
            }

            var checkResultsAndReturnStatus = GetSuccessValidator();

            if (!options.OverwriteExistingSwfs)
            {
                var pagesExist = GetOutputSwfFileNames(outSwfFileMultiPagePattern, numberOfPages).All(File.Exists);

                if (pagesExist)
                {
                    return checkResultsAndReturnStatus(ConversionState.OutputExists);
                }

                //another try
            }

            return await Convert(args, optimizedPdfPath, previousStatus, checkResultsAndReturnStatus);
        }

        private ConversionStatus DefaultSuccessCheck(ConversionState state, string swfFilePath)
        {
            if (state == ConversionState.Converted)
            {
                if (!File.Exists(swfFilePath))
                {
                    var failed = new ConversionStatus(ConversionState.ConversionFailed, "Output file doesn't exist");
                    _logger.LogError(failed.Message);
                    return failed;
                }
            }

            return new ConversionStatus(state);
        }
        private ConversionStatus DefaultMultiPageSuccessCheck(ConversionState state, string swfPageFileNamePattern, int numberOfPages)
        {
            if (state == ConversionState.Converted)
            {
                foreach (var outputSwfFileName in GetOutputSwfFileNames(swfPageFileNamePattern, numberOfPages))
                {
                    if (!File.Exists(outputSwfFileName))
                    {
                        var status = new ConversionStatus(ConversionState.ConversionFailedPaged);
                        status.Message = $"At least one output page is missing: " + outputSwfFileName;
                        _logger.LogError(status.Message);
                        return status;
                    }
                }
            }

            return new ConversionStatus(state);
        }

        private async Task OptimizePdfForConverter(string pdfFilePath, string optimizedFilePath, ConversionStatus status)
        {
            try
            {
                _logger.LogDebug($"Starting PDF optimization for file: {pdfFilePath}.");
                await _pdfProcessor.CheckIfDocumentWasScannedAndFixIfNecessaryAsync(pdfFilePath, optimizedFilePath);
                _logger.LogDebug($"Finishing PDF optimization for file: {pdfFilePath}. Optimized file path: {optimizedFilePath}");
            }
            catch (Exception ex)
            {
                status.Message = $"PDF file optimization failed: {ex}";
                _logger.LogError(status.Message);
                status.State = ConversionState.OptimizationFailed;
            }
        }

        private async Task<int> GetNumberOfPages(string file)
        {
            try
            {
                using (var reader = new PdfReader(file))
                {
                    var res = reader.NumberOfPages;

                    return res != 0 ? res : await GetNumberOfPagesUsingStream(file);
                }
            }
            catch (Exception)
            {
                return await GetNumberOfPagesUsingStream(file);
            }
        }

        private async Task<int> GetNumberOfPagesUsingStream(string file)
        {
            using (var sr = new StreamReader(File.OpenRead(file)))
            {
                var regex = new Regex(@"/Type\s*/Page[^s]");
                var input = await sr.ReadToEndAsync();
                var matches = regex.Matches(input);
                return matches.Count;
            }
        }

        private void SafelyDeleteFile(string file)
        {
            try
            {
                System.IO.File.Delete(file);
            }
            catch (Exception ex)
            {
                //who cares
                _logger.LogWarning($"Can't delete the file:{file}. Error {ex}");
            }
        }


        private async Task<ConversionStatus> Convert(string args, string filePath,
            ConversionStatus previousStatus = null,
            Func<ConversionState, ConversionStatus> checkResultAfterConversion = null)
        {
            var status = previousStatus ?? new ConversionStatus();
            var initialArgs = args;

            if (status.RenderEverythingAsBitmap)
            {
                _logger.LogDebug($"Retrying conversion for the file {filePath} with RenderEverythingAsBitmap option");
                args = $"{args} {_settings.RenderAsBitmapArgs}";
            }

            if (status.OverwriteSource)
            {
                _logger.LogDebug($"Retrying conversion for the file {filePath} trying to overwrite file protection");
                if (!TryToOverwriteProtection(filePath))
                {
                    _logger.LogInformation($"PDF file is either password protected or encrypted:{filePath}");
                    status.Message = "PDF file is protected. Can't proceed with the conversion";
                    status.State = ConversionState.ConversionFailed;
                    return status;
                }
            }

            try
            {
                using (var proc = new Process
                {
                    StartInfo =
                    {
                        FileName = _settings.Pdf2SwfExecutableFilePath,
                        Arguments = args,
                        UseShellExecute = false,
                        WindowStyle = ProcessWindowStyle.Hidden,
                        CreateNoWindow = true,
                        RedirectStandardOutput = true,
                    }
                })
                {
                    if (proc.Start())
                    {
                        string result = proc.StandardOutput.ReadToEnd();
                        await proc.WaitForExitAsync();

                        _logger.LogInformation("Converter message:" + result);
                        proc.Close();


                        if (!status.RenderEverythingAsBitmap
                            && result.Contains("This file is too complex to render- SWF only supports 65536 shapes at once")
                        )
                        {
                            status.RenderEverythingAsBitmap = true;
                            return await Convert(initialArgs, filePath, status);
                        }

                        if (!status.OverwriteSource
                            && result.Contains("PDF disallows copying"))
                        {
                            status.OverwriteSource = true;
                            return await Convert(initialArgs, filePath, status);
                        }
                        else if (status.OverwriteSource)
                        {
                            status.OverwriteSource = false;
                        }

                        if (checkResultAfterConversion != null)
                        {
                            return checkResultAfterConversion(ConversionState.Converted);
                        }

                        status.State = ConversionState.Converted;
                    }
                    else
                    {
                        _logger.LogError($"Starting the conversion tool process failed {_settings.Pdf2SwfExecutableFilePath} {args}");
                        status.State = ConversionState.ConversionFailed;
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Conversion failed with error: {e}");
                status.State = ConversionState.ConversionFailed;
            }
            

            return status;
        }

        private bool TryToOverwriteProtection(string filePath)
        {
            try
            {
                byte[] outPdfBuffer;
                var input = System.IO.File.ReadAllBytes(filePath);
                PdfReader.unethicalreading = true;
                using (var reader = new PdfReader(input))
                {
                    using (var outPdfStream = new MemoryStream())
                    {
                        using (var stamper = new PdfStamper(reader, outPdfStream))
                        {
                            reader.RemoveUnusedObjects();
                        }
                        outPdfBuffer = outPdfStream.ToArray();
                        System.IO.File.WriteAllBytes(filePath, outPdfBuffer);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("TryToOverwriteProtection for source file:" + filePath, ex);
                return false;
            }
        }

        #endregion

    }

}
