using System.Data.SqlClient;
using System.Linq;
using Esynctraining.Core.Enums;
using Esynctraining.Core.Utils;
using Esynctraining.PdfProcessor;

namespace PDFAnnotation.Core.Utils
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Threading.Tasks;

    using Castle.Core.Logging;

    using Esynctraining.Core.Providers;

    using PDFAnnotation.Core.Business.Models;
    using PDFAnnotation.Core.Domain.DTO;

    /// <summary>
    /// The PDF to SWF converter.
    /// </summary>
    /// <summary>
    ///     The PDF to SWF converter.
    /// </summary>
    public class Pdf2SwfConverter
    {
        #region Fields

        /// <summary>
        ///     The logger.
        /// </summary>
        private readonly ILogger logger;

        /// <summary>
        ///     The pdf processor.
        /// </summary>
        private readonly PdfProcessorHelper pdfProcessor;

        /// <summary>
        ///     The settings.
        /// </summary>
        private readonly dynamic settings;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Pdf2SwfConverter"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="settings">
        /// The settings.
        /// </param>
        public Pdf2SwfConverter(ILogger logger, PdfProcessorHelper pdfProcessor, ApplicationSettingsProvider settings)
        {
            this.logger = logger;
            this.pdfProcessor = pdfProcessor;
            this.settings = settings;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The convert.
        /// </summary>
        /// <param name="pdfFilePath">
        /// The PDF file path.
        /// </param>
        /// <param name="swfFilePath">
        /// The SWF file path.
        /// </param>
        public void Convert(string pdfFilePath, string swfFilePath)
        {
            string args = ((string)this.settings.SWFToolsCommandPattern).Replace("{pdfFile}", pdfFilePath).Replace("{swfFile}", swfFilePath);
            bool renderAsBitmap = false;
            this.Convert(args, ref renderAsBitmap, null, null);
        }

        /// <summary>
        /// The convert if not exist.
        /// </summary>
        /// <param name="file">
        /// The file.
        /// </param>
        /// <param name="ms">
        /// The memory stream.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool ConvertIfNotExist(Domain.Entities.File file, string failedFolder, string connectionString, byte[] ms = null)
        {
            return this.ConvertIfNotExist(new FileDTO(file), failedFolder, connectionString, ms);
        }

        /// <summary>
        /// The convert if not exist.
        /// </summary>
        /// <param name="fileDTO">
        /// The file DTO.
        /// </param>
        /// <param name="ms">
        /// The memory stream.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool ConvertIfNotExist(FileDTO fileDTO, string failedFolder, string connectionString, byte[] ms = null)
        {
            try
            {
                var fileStoragePhysicalPath = (string)FileModel.FileStoragePhysicalPath(this.settings);
                var permSWFPattern = (string)this.settings.PermSWFPattern;
                var permSWFPagePattern = (string)this.settings.PermSWFPagePattern;
                this.EnsureDirectoryExist(fileStoragePhysicalPath);
                var fileId = fileDTO.fileId;
                int numberOfPages = fileDTO.numberOfPages;
                var workingDir = Path.Combine(fileStoragePhysicalPath, fileId.ToString());
                this.EnsureDirectoryExist(workingDir);
                var pdfFile = Path.Combine(workingDir, "document.pdf");
                var swfFile = Path.Combine(workingDir, "document.swf");
                var swfPagedFile = Path.Combine(workingDir, "%.swf");
                bool isDebugEnabled = this.logger.IsDebugEnabled;

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

                this.SafelyDeleteSwfFile(swfFile);

                string baseArgs = ((string)this.settings.SWFToolsCommandPattern).Replace("{pdfFile}", pdfFile);

                var logger = this.logger;

                Task.Factory.StartNew(
                    () =>
                    {
                        var s = new Stopwatch();
                        if (isDebugEnabled)
                        {
                            logger.Debug("Save SWF pages in background");
                            s.Start();
                        }

                        this.UpdateFileWithStatus(connectionString, fileId, UploadFileStatus.Rendering, logger);
                        try
                        {
                            pdfProcessor.CheckIfDocumentWasScannedAndFixIfNecessary(pdfFile);
                        }
                        catch (Exception ex)
                        {
                            this.CopyFileToFailedFolder(connectionString, failedFolder, pdfFile, fileId, logger, "_rendering");
                        }


                        if (isDebugEnabled)
                        {
                            logger.DebugFormat("Process images PDF took {0}s", s.ElapsedMilliseconds / 1000.0);
                            s.Restart();
                        }

                        this.UpdateFileWithStatus(connectionString, fileId, UploadFileStatus.Converting, logger);
                        bool renderAsBitmap = false;
                        bool bigPageSucceded = this.Convert(baseArgs.Replace("{swfFile}", swfFile), ref renderAsBitmap, () => System.IO.File.Exists(swfFile), (fail) => this.CopyFileToFailedFolder(connectionString, failedFolder, pdfFile, fileId, logger, "_converting", UploadFileStatus.ConvertingFailed, fail));
                        if (isDebugEnabled)
                        {
                            logger.DebugFormat("Convert SWF took {0}s", s.ElapsedMilliseconds / 1000.0);
                            s.Restart();
                        }

                        if (bigPageSucceded)
                        {
                            // convert page files
                            this.UpdateFileWithStatus(connectionString, fileId, UploadFileStatus.Converted, logger);

                            this.Convert(baseArgs.Replace("{swfFile}", swfPagedFile), ref renderAsBitmap, () => this.CheckIfSwfPagedFilesExist(swfPagedFile, numberOfPages), (fail) => this.CopyFileToFailedFolder(connectionString, failedFolder, pdfFile, fileId, logger, "_pages", UploadFileStatus.ConvertingPagesFailed, fail));
                            if (isDebugEnabled)
                            {
                                logger.DebugFormat("Convert SWF pages took {0}s", s.ElapsedMilliseconds / 1000.0);
                            }
                        }
                    });

                return true;
            }
            catch (Exception ex)
            {
                this.logger.Error("Converting error", ex);
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
                    this.UpdateFileWithStatus(connectionString, fileId, status.Value, log);
                }
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
            string updateStatusCommand = string.Format("update [{0}] set {1} = @status where {2} = @id", typeName, Inflector.Uncapitalize(Lambda.Property<Domain.Entities.File>(x => x.UploadFileStatus)), Inflector.Uncapitalize(typeName + Lambda.Property<Domain.Entities.File>(x => x.Id)));
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

        /// <summary>
        /// The convert.
        /// </summary>
        /// <param name="args">
        /// The args.
        /// </param>
        private bool Convert(string args, ref bool renderEverythingAsBitmap, Func<bool> checkSuccess = null, Action<string> copyToCarantineAndLog = null)
        {
            var initialArgs = args;
            if (renderEverythingAsBitmap)
            {
                args = args + " -s poly2bitmap";
            }
            var proc = new Process
            {
                StartInfo =
                                   {
                                       FileName = (string)this.settings.SWFToolsPath,
                                       Arguments = args,
                                       UseShellExecute = false,
                                       WindowStyle = ProcessWindowStyle.Hidden,
                                       CreateNoWindow = true,
                                       RedirectStandardOutput = true
                                   }
            };

            if (proc.Start())
            {
                string result = proc.StandardOutput.ReadToEnd();
                this.logger.Info("Converter message:" + result);
                proc.WaitForExit();
                proc.Close();
                if (!renderEverythingAsBitmap && result.Contains("This file is too complex to render- SWF only supports 65536 shapes at once"))
                {
                    renderEverythingAsBitmap = true;
                    this.Convert(initialArgs, ref renderEverythingAsBitmap);
                }

                if (checkSuccess != null && !checkSuccess.Invoke() && copyToCarantineAndLog != null)
                {
                    copyToCarantineAndLog.Invoke(result);
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// The ensure directory exist.
        /// </summary>
        /// <param name="workingFolder">
        /// The working folder.
        /// </param>
        private void EnsureDirectoryExist(string workingFolder)
        {
            if (!Directory.Exists(workingFolder))
            {
                Directory.CreateDirectory(workingFolder);
            }
        }

        #endregion
    }
}