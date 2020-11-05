using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Esynctraining.Core.Logging;
using Esynctraining.Core.Providers;
using Esynctraining.Core.Utils;
using Esynctraining.PdfProcessor;
using iTextSharp.text.pdf;
using PDFAnnotation.Core.Business.Models;
using PDFAnnotation.Core.Domain.DTO;
using PDFAnnotation.Core.Domain.Entities;

namespace PDFAnnotation.Core.Utils
{
    /// <summary>
    /// The PDF to SWF converter.
    /// </summary>
    public class Pdf2SwfConverter
    {
        #region Fields

        private readonly ILogger _logger;
        private readonly PdfProcessorHelper _pdfProcessor;
        private readonly dynamic _settings;

        #endregion

        #region Constructors and Destructors

        public Pdf2SwfConverter(ILogger logger, PdfProcessorHelper pdfProcessor, ApplicationSettingsProvider settings)
        {
            _logger = logger;
            _pdfProcessor = pdfProcessor;
            _settings = settings;
        }

        #endregion

        #region Public Methods and Operators

        public void Convert(string pdfFilePath, string swfFilePath)
        {
            string args = ((string)this._settings.SWFToolsCommandPattern).Replace("{pdfFile}", pdfFilePath).Replace("{swfFile}", swfFilePath);
            bool renderAsBitmap = false;
            bool overwriteSource = false;
            Convert(args, pdfFilePath, ref renderAsBitmap, ref overwriteSource, null, null);
        }

        public bool ConvertIfNotExist(Domain.Entities.File file, string failedFolder, string connectionString, byte[] ms = null)
        {
            return ConvertIfNotExist(new FileDTO(file), failedFolder, connectionString, ms);
        }

        public bool ConvertIfNotExist(FileDTO fileDTO, string failedFolder, string connectionString, byte[] ms = null)
        {
            try
            {
                var fileStoragePhysicalPath = (string)FileModel.FileStoragePhysicalPath(this._settings);
                EnsureDirectoryExist(fileStoragePhysicalPath);
                var fileId = fileDTO.fileId;
                int numberOfPages = fileDTO.numberOfPages;
                var workingDir = Path.Combine(fileStoragePhysicalPath, fileId.ToString());
                EnsureDirectoryExist(workingDir);
                var pdfFile = Path.Combine(workingDir, "document.pdf");
                var swfFile = Path.Combine(workingDir, "document.swf");
                var swfPagedFile = Path.Combine(workingDir, "%.swf");

                if (!System.IO.File.Exists(pdfFile) && ms != null)
                {
                    System.IO.File.WriteAllBytes(pdfFile, ms);
                }

                if (!System.IO.File.Exists(pdfFile))
                {
                    return false;
                }

                if (System.IO.File.Exists(swfFile) && new FileInfo(pdfFile).LastWriteTime < new FileInfo(swfFile).LastWriteTime)
                {
                    return true;
                }

                SafelyDeleteSwfFile(swfFile);

                string baseArgs = ((string)_settings.SWFToolsCommandPattern).Replace("{pdfFile}", pdfFile);

                var logger = _logger;

                Task.Factory.StartNew(
                    () =>
                    {
                        UpdateFileWithStatus(connectionString, new Guid(fileId), UploadFileStatus.Rendering, logger);
                        try
                        {
                            _pdfProcessor.CheckIfDocumentWasScannedAndFixIfNecessary(pdfFile);
                        }
                        catch (Exception ex)
                        {
                            CopyFileToFailedFolder(connectionString, failedFolder, pdfFile, new Guid(fileId), logger, "_rendering");
                        }

                        UpdateFileWithStatus(connectionString, new Guid(fileId), UploadFileStatus.Converting, logger);
                        bool renderAsBitmap = false;
                        bool overWriteSource = false;
                        bool bigPageSucceded = this.Convert(baseArgs.Replace("{swfFile}", swfFile), pdfFile, ref renderAsBitmap, ref overWriteSource, () => System.IO.File.Exists(swfFile), (fail) => this.CopyFileToFailedFolder(connectionString, failedFolder, pdfFile, new Guid(fileId), logger, "_converting", UploadFileStatus.ConvertingFailed, fail));

                        if (bigPageSucceded)
                        {
                            // convert page files
                            UpdateFileWithStatus(connectionString, new Guid(fileId), UploadFileStatus.Converted, logger);

                            Convert(baseArgs.Replace("{swfFile}", swfPagedFile), pdfFile, ref renderAsBitmap, ref overWriteSource, () => this.CheckIfSwfPagedFilesExist(swfPagedFile, numberOfPages), (fail) => this.CopyFileToFailedFolder(connectionString, failedFolder, pdfFile, new Guid(fileId), logger, "_pages", UploadFileStatus.ConvertingPagesFailed, fail));
                        }
                    });

                return true;
            }
            catch (Exception ex)
            {
                _logger.Error("Converting error", ex);
                return false;
            }
        }

        private void SafelyDeleteSwfFile(string swfFile)
        {
            try
            {
                System.IO.File.Delete(swfFile);
            }
            catch
            {
            }
        }

        private bool CheckIfSwfPagedFilesExist(string swfPagedFile, int numberOfPages)
        {
            swfPagedFile = swfPagedFile.Replace("%", "{0}");
            if (numberOfPages > 0)
            {
                for (int i = 1; i <= numberOfPages; i++)
                {
                    if (!System.IO.File.Exists(string.Format(swfPagedFile, i)))
                    {
                        return false;
                    }
                }


                return true;
            }

            return false;
        }

        private void CopyFileToFailedFolder(string connectionString, string failedFolder, string pdfFile, Guid fileId, ILogger log, string posfix = null, UploadFileStatus? status = null, string failedMessage = null)
        {
            try
            {
                if (status.HasValue)
                {
                    UpdateFileWithStatus(connectionString, fileId, status.Value, log);
                }

                EnsureDirectoryExist(failedFolder);


                System.IO.File.Copy(pdfFile, Path.Combine(failedFolder, fileId + (string.IsNullOrWhiteSpace(posfix) ? string.Empty : posfix) + ".pdf"));

                
                if (!string.IsNullOrWhiteSpace(failedMessage))
                {
                    System.IO.File.WriteAllText(
                        Path.Combine(
                            failedFolder,
                            fileId + (string.IsNullOrWhiteSpace(posfix) ? string.Empty : posfix) + ".log"),
                        failedMessage);
                }
            }
            catch (Exception ex)
            {
                try
                {
                    log.Error("Converter. Error during copy to failed folder: ", ex);
                }
                // ReSharper disable once EmptyGeneralCatchClause
                catch
                {
                }
            }
        }

        #endregion

        #region Methods

        private bool UpdateFileWithStatus(string connectionString, Guid id, UploadFileStatus status, ILogger log)
        {
            var typeName = typeof(Domain.Entities.File).Name;
            string updateStatusCommand = string.Format("update [{0}] set {1} = @status where {2} = @id", typeName, 
                Inflector.Uncapitalize(Lambda.Property<Domain.Entities.File>(x => x.UploadFileStatus)), 
                Inflector.Uncapitalize(typeName + Lambda.Property<Domain.Entities.File>(x => x.Id)));
            using (var con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    using (var cmd = new SqlCommand(updateStatusCommand, con))
                    {
                        cmd.Parameters.AddWithValue("@status", (int)status);
                        cmd.Parameters.AddWithValue("@id", id);
                        int effected = cmd.ExecuteNonQuery();
                        return effected == 1;
                    }
                }
                catch (Exception ex)
                {
                    try
                    {
                        log.Error("Converter. Failed to update status of file: id=" + id + ", status=" + (int)status, ex);
                    }
                    // ReSharper disable once EmptyGeneralCatchClause
                    catch
                    {
                    }
                }
            }

            return false;
        }

        private bool Convert(string args, string filePath, ref bool renderEverythingAsBitmap, ref bool overwriteSource, Func<bool> checkSuccess = null, Action<string> copyToCarantineAndLog = null)
        {
            var initialArgs = args;
            if (renderEverythingAsBitmap)
            {
                args = args + " -s poly2bitmap";
            }
            if (overwriteSource)
            {
                if (!TryToOverwriteProtection(filePath))
                {
                    return false;
                }
            }
            var proc = new Process
            {
                StartInfo =
                {
                    FileName = (string)_settings.SWFToolsPath,
                    Arguments = args,
                    UseShellExecute = false,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                }
            };

            if (proc.Start())
            {
                string result = proc.StandardOutput.ReadToEnd();
                _logger.Info("Converter message:" + result);
                proc.WaitForExit();
                proc.Close();
                if (!renderEverythingAsBitmap && result.Contains("This file is too complex to render- SWF only supports 65536 shapes at once"))
                {
                    renderEverythingAsBitmap = true;
                    Convert(initialArgs, filePath, ref renderEverythingAsBitmap, ref overwriteSource);
                }

                if (!overwriteSource && result.Contains("PDF disallows copying"))
                {
                    overwriteSource = true;
                    Convert(initialArgs, filePath, ref renderEverythingAsBitmap, ref overwriteSource);
                }
                else if (overwriteSource)
                {
                    overwriteSource = false;
                }

                if (checkSuccess != null && !checkSuccess.Invoke() && copyToCarantineAndLog != null)
                {
                    copyToCarantineAndLog.Invoke(result);
                    return false;
                }
            }

            return true;
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
                _logger.Info("TryToOverwriteProtection for source file:" + filePath, ex);
                return false;
            }
        }

        private void EnsureDirectoryExist(string workingFolder)
        {
            if (Directory.Exists(workingFolder))
                return;
            Directory.CreateDirectory(workingFolder);
        }

        #endregion

    }

}