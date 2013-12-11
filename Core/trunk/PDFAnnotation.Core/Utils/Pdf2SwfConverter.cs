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
    public class Pdf2SwfConverter
    {
        #region Fields

        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger logger;

        /// <summary>
        /// The settings.
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
        public Pdf2SwfConverter(ILogger logger, ApplicationSettingsProvider settings)
        {
            this.logger = logger;
            this.settings = settings;
        }

        #endregion

        #region Public Methods and Operators

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
        public bool ConvertIfNotExist(Domain.Entities.File file, byte[] ms = null)
        {
            return this.ConvertIfNotExist(new FileDTO(file), ms);
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
        public bool ConvertIfNotExist(FileDTO fileDTO, byte[] ms = null)
        {
            if (fileDTO.webOrbId.HasValue)
            {
                try
                {
                    var workingDir = FileModel.FileStoragePhysicalPath(this.settings);
                    this.EnsureDirectoryExist(workingDir);
                    workingDir = Path.Combine(workingDir, fileDTO.webOrbId.Value.ToString());
                    this.EnsureDirectoryExist(workingDir);
                    var pdfFile = Path.Combine(workingDir, "document.pdf");
                    var swfFile = Path.Combine(workingDir, "document.swf");
                    var swfPagedFile = Path.Combine(workingDir, "%.swf");

                    var sw = new Stopwatch();
                    sw.Start();

                    if (!File.Exists(pdfFile) && ms != null)
                    {
                        File.WriteAllBytes(pdfFile, ms);
                    }

                    this.logger.DebugFormat("Save PDF took {0}s", sw.ElapsedMilliseconds / 1000.0);

                    if (!File.Exists(pdfFile))
                    {
                        return false;
                    }

                    if (File.Exists(swfFile)
                        && new FileInfo(pdfFile).LastWriteTime < new FileInfo(swfFile).LastWriteTime)
                    {
                        return true;
                    }

                    var baseArgs = ((string)this.settings.SWFToolsCommandPattern).Replace("{pdfFile}", pdfFile);

                    sw.Restart();

                    // convert complete file
                    this.Convert(baseArgs.Replace("{swfFile}", swfFile));

                    this.logger.DebugFormat("Save SWF took {0}s", sw.ElapsedMilliseconds / 1000.0);
                    sw.Stop();

                    Task.Factory.StartNew(
                        () =>
                            {
                                this.logger.Debug("Save SWF pages in background");
                                
                                var s = new Stopwatch();
                                // convert page files
                                this.Convert(baseArgs.Replace("{swfFile}", swfPagedFile));

                                this.logger.DebugFormat("Save SWF pages took {0}s", s.ElapsedMilliseconds / 1000.0);
                            });
                    
                    return true;
                }
                catch (Exception ex)
                {
                    this.logger.Error("Converting error", ex);
                    return false;
                }
            }

            return false;
        }

        public void Convert(string pdfFilePath, string swfFilePath)
        {
            var args = ((string)this.settings.SWFToolsCommandPattern).Replace("{pdfFile}", pdfFilePath).Replace("{swfFile}", swfFilePath);

            this.Convert(args);
        }

        #endregion

        #region Methods

        private void Convert(string args)
        {
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
            }
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