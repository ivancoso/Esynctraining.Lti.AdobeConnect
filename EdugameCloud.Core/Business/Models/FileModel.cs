namespace EdugameCloud.Core.Business.Models
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Imaging;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Web;
    using EdugameCloud.Core.Domain.DTO;
    using EdugameCloud.Core.Domain.Entities;
    using Esynctraining.Core.Extensions;
    using Esynctraining.Core.Logging;
    using Esynctraining.Core.Providers;
    using Esynctraining.Core.Utils;
    using Esynctraining.NHibernate;
    using Esynctraining.NHibernate.Queries;
    using NHibernate;
    using File = EdugameCloud.Core.Domain.Entities.File;

    /// <summary>
    /// The file model.
    /// </summary>
    public class FileModel : BaseModel<File, Guid>
    {
        #region Fields
        
        private readonly dynamic settings;
        private readonly ILogger _logger;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes static members of the <see cref="FileModel" /> class.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1409:RemoveUnnecessaryCode", Justification = "Reviewed. Suppression is OK here.")]
        // ReSharper disable once EmptyConstructor
        static FileModel()
        {
        }
        
        public FileModel(IRepository<File, Guid> repository, ApplicationSettingsProvider settings, ILogger logger)
            : base(repository)
        {
            this.settings = settings;
            _logger = logger;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The try parse image.
        /// </summary>
        /// <param name="bytes">
        /// The bytes.
        /// </param>
        /// <param name="img">
        /// The image.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool TryParseImage(byte[] bytes, out Image img)
        {
            img = null;
            try
            {
                img = Image.FromStream(new MemoryStream(bytes));
            }
            catch (ArgumentException)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// The delete folder.
        /// </summary>
        /// <param name="target_dir">
        /// The target directory.
        /// </param>
        public static void DeleteFolder(string target_dir)
        {
            string[] files = Directory.GetFiles(target_dir);
            string[] dirs = Directory.GetDirectories(target_dir);

            foreach (string file in files)
            {
                System.IO.File.SetAttributes(file, FileAttributes.Normal);
                System.IO.File.Delete(file);
            }

            foreach (string dir in dirs)
            {
                DeleteFolder(dir);
            }

            Directory.Delete(target_dir, false);
        }
                
        public virtual IFutureValue<File> GetOneByWebOrbId(Guid id)
        {
            var queryOver = new DefaultQueryOver<File, Guid>().GetQueryOver().Where(x => x.Id == id).Take(1);
            return this.Repository.FindOne(queryOver);
        }


        public virtual IFutureValue<File> GetOneByUniqueName(string name)
        {
            var queryOver = new DefaultQueryOver<File, Guid>().GetQueryOver().Where(x => x.FileName == name).Take(1);
            return this.Repository.FindOne(queryOver);
        }

        /// <summary>
        /// The complete file.
        /// </summary>
        /// <param name="file">
        /// The file.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool CompleteFile(File file)
        {
            string permanentFileName = this.PermanentFileName(file);
            using (FileStream fileStream = System.IO.File.OpenWrite(permanentFileName))
            {
                string webOrbFolderName = this.WebOrbFolderName(file.Id);
                string webOrbFile;
                if (Directory.Exists(webOrbFolderName) && (webOrbFile = Directory.GetFiles(webOrbFolderName).FirstOrDefault()) != null)
                {
                    byte[] content = System.IO.File.ReadAllBytes(webOrbFile);
                    this.ResizeImage(content, permanentFileName, fileStream);

                    try
                    {
                        DeleteFolder(webOrbFolderName);
                    }
                    finally
                    {
                        file.Status = ImageStatus.Completed;
                        this.RegisterSave(file);
                    }

                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// The resize image.
        /// </summary>
        /// <param name="content">Image content.</param>
        /// <param name="permanentFileName">Permanent file name.</param>
        /// <param name="fileStream">Image stream.</param>
        public void ResizeImage(byte[] content, string permanentFileName, Stream fileStream)
        {
            Image img;
            if (TryParseImage(content, out img))
            {
                try
                {
                    var resizedImage = this.ResizeImage(permanentFileName, int.Parse(this.settings.MaxImageWidth), int.Parse(this.settings.MaxImageHeight), img);
                    var iciPng = GetPngImageCodecInfo();
                    var encoderParameters = new EncoderParameters(1);
                    encoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, 100L);
                    resizedImage.Save(fileStream, iciPng, encoderParameters);
                }
                catch (Exception ex)
                {
                    IoC.Resolve<ILogger>().Error("Fail during image resize", ex);
                    fileStream.Write(content, 0, content.Length);
                }
            }
            else
            {
                fileStream.Write(content, 0, content.Length);
            }
        }

        /// <summary>
        /// The register delete.
        /// </summary>
        /// <param name="file">
        /// The file.
        /// </param>
        /// <param name="flush">
        /// The flush.
        /// </param>
        public override void RegisterDelete(File file, bool flush)
        {
            var filePath = this.PermanentFileName(file);
            if (System.IO.File.Exists(filePath))
            {
                try
                {
                    System.IO.File.Delete(filePath);
                }
                catch (Exception)
                {
                }
            }

            base.RegisterDelete(file, flush);
        }
        
        public File CreateFile(User user, string name, DateTime date, int? width, int? height, int? x, int? y)
        {
            var image = new File { CreatedBy = user, FileName = name, Height = height, Width = width, X = x, Y = y };
            if (date != default(DateTime))
            {
                image.DateCreated = date;
            }
            else
            {
                image.DateCreated = DateTime.Now;
            }

            this.RegisterSave(image, true);
            return image;
        }
        
        public byte[] GetData(File file)
        {
            var fileName = this.PermanentFileName(file);
            if (System.IO.File.Exists(fileName))
            {
                return System.IO.File.ReadAllBytes(fileName);    
            }

            _logger.WarnFormat("File not found. ID: {0}. FileName: {1}.", file.Id, fileName);
            return null;
        }
        
        public File SetData(File file, byte[] data)
        {
            var fileName = this.PermanentFileName(file);
            System.IO.File.WriteAllBytes(fileName, data);
            return file;
        }

        public string PermanentFileName(File file)
        {
            var companyId = file.CreatedBy.With(x => x.Company).With(x => x.Id.ToString(CultureInfo.InvariantCulture));
            var folder = Path.Combine(this.FileStoragePhysicalPath(), companyId);
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            return Path.Combine(folder, this.settings.PermFilePattern.Replace("{fileId}", file.Id.ToString()));
        }

        /// <summary>
        /// The save WEBORB file.
        /// </summary>
        /// <param name="file">
        /// The file.
        /// </param>
        public void SaveWeborbFile(UploadedFileDTO file)
        {
            var workingDir = this.WebOrbFolderName(file.fileId);
            if (!Directory.Exists(workingDir))
            {
                Directory.CreateDirectory(workingDir);
            }

            System.IO.File.WriteAllBytes(Path.Combine(workingDir, file.fileName), file.content);
        }

        #endregion

        #region Methods

        /// <summary>
        /// The get PNG image codec info.
        /// </summary>
        /// <returns>
        /// The <see cref="ImageCodecInfo"/>.
        /// </returns>
        private static ImageCodecInfo GetPngImageCodecInfo()
        {
            ImageCodecInfo iciPng = null;
            foreach (ImageCodecInfo ici in ImageCodecInfo.GetImageDecoders())
            {
                if (ici.FilenameExtension.ToLower().Contains("png"))
                {
                    iciPng = ici;
                    break;
                }
            }

            iciPng = iciPng ?? ImageCodecInfo.GetImageDecoders()[1];
            return iciPng;
        }

        /// <summary>
        /// The resize image.
        /// </summary>
        /// <param name="path">
        /// The path.
        /// </param>
        /// <param name="maxCanvasWidth">
        /// The max canvas width.
        /// </param>
        /// <param name="maxCanvasHeight">
        /// The max canvas height.
        /// </param>
        /// <param name="img">
        /// The image.
        /// </param>
        /// <param name="originalWidth">
        /// The original width.
        /// </param>
        /// <param name="originalHeight">
        /// The original height.
        /// </param>
        /// <returns>
        /// The <see cref="Image"/>.
        /// </returns>
        private Image ResizeImage(
            string path,
            int maxCanvasWidth,
            int maxCanvasHeight,
                                  Image img = null,
                                  int? originalWidth = null,
                                  int? originalHeight = null)
        {
            Image image = img ?? Image.FromFile(path);
            if (originalWidth == null)
            {
                originalWidth = image.Width;
            }

            if (originalHeight == null)
            {
                originalHeight = image.Height;
            }

            var canvasWidth = Math.Min(maxCanvasWidth, originalWidth.Value);
            var canvasHeight = Math.Min(maxCanvasHeight, originalHeight.Value);

            /* ------------------ new code --------------- */

            // Figure out the ratio
            // ReSharper disable once RedundantCast
            double ratioX = (double)canvasWidth / (double)originalWidth;
            // ReSharper disable once RedundantCast
            double ratioY = (double)canvasHeight / (double)originalHeight;

            // use whichever multiplier is smaller
            double ratio = ratioX < ratioY ? ratioX : ratioY;

            // now we can get the new height and width
            int newHeight = Convert.ToInt32(originalHeight * ratio);
            int newWidth = Convert.ToInt32(originalWidth * ratio);

            // Now calculate the X,Y position of the upper-left corner 
            // (one of these will always be zero)
            // ReSharper disable once UnusedVariable
            int posX = Convert.ToInt32((canvasWidth - (originalWidth * ratio)) / 2);
            // ReSharper disable once UnusedVariable
            int posY = Convert.ToInt32((canvasHeight - (originalHeight * ratio)) / 2);

            Image thumbnail = new Bitmap(newWidth, newHeight); // changed parm names

            var graphic = Graphics.FromImage(thumbnail);
            graphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphic.SmoothingMode = SmoothingMode.HighQuality;
            graphic.PixelOffsetMode = PixelOffsetMode.HighQuality;
            graphic.CompositingQuality = CompositingQuality.HighQuality;
            graphic.Clear(Color.Transparent); // white padding
            graphic.DrawImage(image, 0, 0, newWidth, newHeight);

            /* ------------- end new code ---------------- */
            return thumbnail;
        }

        /// <summary>
        /// The file storage physical path.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string FileStoragePhysicalPath()
        {
            var fileStorage = this.settings.FileStorage;
            return fileStorage.StartsWith("~") ? HttpContext.Current.Server.MapPath(fileStorage) : fileStorage;
        }

        /// <summary>
        /// The file storage physical path.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string WebOrbStoragePhysicalPath()
        {
            var fileStorage = this.settings.WebOrbStorage;
            return fileStorage.StartsWith("~") ? HttpContext.Current.Server.MapPath(fileStorage) : fileStorage;
        }

        /// <summary>
        /// The permanent file name.
        /// </summary>
        /// <param name="fileId">
        /// The fileId.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string WebOrbFolderName(Guid fileId)
        {
            return Path.Combine(this.WebOrbStoragePhysicalPath(), fileId.ToString());
        }

        #endregion
    }
}