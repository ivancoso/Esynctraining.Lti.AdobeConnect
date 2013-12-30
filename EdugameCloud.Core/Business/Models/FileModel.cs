namespace EdugameCloud.Core.Business.Models
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Imaging;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Web;

    using Castle.Core.Logging;

    using EdugameCloud.Core.Domain.Entities;

    using Esynctraining.Core.Business;
    using Esynctraining.Core.Business.Models;
    using Esynctraining.Core.Business.Queries;
    using Esynctraining.Core.Extensions;
    using Esynctraining.Core.Providers;
    using Esynctraining.Core.Utils;

    using NHibernate;
    using NHibernate.Linq;

    using File = EdugameCloud.Core.Domain.Entities.File;

    /// <summary>
    ///     The file model.
    /// </summary>
    public class FileModel : BaseModel<File, int>
    {
        #region Static Fields

        /// <summary>
        ///     The object to sync threads.
        /// </summary>
        private static readonly object SyncObj = new object();

        #endregion

        #region Fields

        /// <summary>
        ///     The settings.
        /// </summary>
        private readonly dynamic settings;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes static members of the <see cref="FileModel" /> class.
        /// </summary>
        static FileModel()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileModel"/> class.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="settings">
        /// The settings.
        /// </param>
        public FileModel(IRepository<File, int> repository, ApplicationSettingsProvider settings)
            : base(repository)
        {
            this.settings = settings;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The read stream fully.
        /// </summary>
        /// <param name="input">
        /// The input.
        /// </param>
        /// <returns>
        /// The array of byte.
        /// </returns>
        public byte[] ReadStreamFully(Stream input)
        {
            var buffer = new byte[16 * 1024];
            using (var ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }

                return ms.ToArray();
            }
        }

        /// <summary>
        /// The get one by web orb id.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="IFutureValue{File}"/>.
        /// </returns>
        public virtual IFutureValue<File> GetOneByWebOrbId(Guid id)
        {
            var queryOver = new DefaultQueryOver<File, int>().GetQueryOver().Where(x => x.WebOrbId == id).Take(1);
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
                string webOrbFolderName = this.WebOrbFolderName(file);
                string webOrbFile;
                if (Directory.Exists(webOrbFolderName) && (webOrbFile = Directory.GetFiles(webOrbFolderName).FirstOrDefault()) != null)
                {
                    byte[] content = System.IO.File.ReadAllBytes(webOrbFile);
                    ResizeImage(content, permanentFileName, fileStream);

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
        /// The try parse image.
        /// </summary>
        /// <param name="bytes">
        /// The bytes.
        /// </param>
        /// <param name="img">
        /// The img.
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
        /// The img.
        /// </param>
        /// <param name="originalWidth">
        /// The original width.
        /// </param>
        /// <param name="originalHeight">
        /// The original height.
        /// </param>
        private Image ResizeImage(string path, 
                     /* note changed names */
                     int maxCanvasWidth, int maxCanvasHeight,
                     /* new */
                     Image img = null,
                     int? originalWidth = null, int? originalHeight = null)
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
            double ratioX = (double)canvasWidth / (double)originalWidth;
            double ratioY = (double)canvasHeight / (double)originalHeight;

            // use whichever multiplier is smaller
            double ratio = ratioX < ratioY ? ratioX : ratioY;

            // now we can get the new height and width
            int newHeight = Convert.ToInt32(originalHeight * ratio);
            int newWidth = Convert.ToInt32(originalWidth * ratio);

            // Now calculate the X,Y position of the upper-left corner 
            // (one of these will always be zero)
            int posX = Convert.ToInt32((canvasWidth - (originalWidth * ratio)) / 2);
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
                    var resizedImage = this.ResizeImage(permanentFileName, int.Parse(settings.MaxImageWidth), int.Parse(settings.MaxImageHeight), img);
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
        /// The delete folder.
        /// </summary>
        /// <param name="target_dir">
        /// The target_dir.
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

        /// <summary>
        /// The register save.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <param name="flush">
        /// The flush.
        /// </param>
        public override void RegisterSave(File entity, bool flush)
        {
            if (entity.IsTransient() && entity.WebOrbId.HasValue)
            {
                while (this.GetOneByWebOrbId(entity.WebOrbId.Value).Value != null)
                {
                    entity.WebOrbId = Guid.NewGuid();
                }
            }

            base.RegisterSave(entity, flush);
        }

        /// <summary>
        /// The create file.
        /// </summary>
        /// <param name="user">
        /// The user.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="date">
        /// The date.
        /// </param>
        /// <param name="width">
        /// The width.
        /// </param>
        /// <param name="height">
        /// The height.
        /// </param>
        /// <param name="x">
        /// The x.
        /// </param>
        /// <param name="y">
        /// The y.
        /// </param>
        /// <returns>
        /// The <see cref="File"/>.
        /// </returns>
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

        /// <summary>
        /// The get all by user.
        /// </summary>
        /// <param name="user">
        /// The user.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{File}"/>.
        /// </returns>
        public virtual IEnumerable<File> GetAllByUser(User user)
        {
            return this.Repository.Session.Query<File>().Where(x => x.CreatedBy.Id == user.Id);
        }

        /// <summary>
        /// The get data.
        /// </summary>
        /// <param name="file">
        /// The file.
        /// </param>
        /// <returns>
        /// The array of bytes.
        /// </returns>
        public byte[] GetData(File file)
        {
            var fileName = this.PermanentFileName(file);
            if (System.IO.File.Exists(fileName))
            {
                return System.IO.File.ReadAllBytes(fileName);    
            }

            return null;
        }

        /// <summary>
        /// The set data.
        /// </summary>
        /// <param name="file">
        /// The file.
        /// </param>
        /// <param name="data">
        /// Thi file data.
        /// </param>
        /// <returns>
        /// The file.
        /// </returns>
        public File SetData(File file, byte[] data)
        {
            var fileName = this.PermanentFileName(file);
            System.IO.File.WriteAllBytes(fileName, data);
            return file;
        }

        /// <summary>
        /// The get one by name.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <returns>
        /// The <see cref="IFutureValue{File}"/>.
        /// </returns>
        public virtual IFutureValue<File> GetOneByName(string name)
        {
            return this.Repository.Session.Query<File>().Where(x => x.FileName == name.ToLower()).ToFutureValue();
        }

        /// <summary>
        /// The get permanent url.
        /// </summary>
        /// <param name="file">
        /// The file.
        /// </param>
        /// <returns>
        /// The <see cref="string"/> url.
        /// </returns>
        public virtual string GetPermanentUrl(File file)
        {
            if (file != null)
            {
                var companyId = file.CreatedBy.With(x => x.Company).With(x => x.Id.ToString(CultureInfo.InvariantCulture));
                return this.settings.FileStorage + "/" + companyId + "/" + this.settings.PermFilePattern.Replace("{fileId}", file.WebOrbId.ToString());
            }

            return string.Empty;
        }

        /// <summary>
        /// The get all by ids.
        /// </summary>
        /// <param name="ids">
        /// The ids.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{File}"/>.
        /// </returns>
        public IEnumerable<File> GetAllByGuids(List<Guid> ids)
        {
            var queryOver = new DefaultQueryOver<File, int>().GetQueryOver().AndRestrictionOn(x => x.WebOrbId).IsIn(ids);
            return this.Repository.FindAll(queryOver);
        }

        #endregion

        #region Methods

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
        /// <param name="file">
        /// The file.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string PermanentFileName(File file)
        {
            var companyId = file.CreatedBy.With(x => x.Company).With(x => x.Id.ToString(CultureInfo.InvariantCulture));
            var folder = Path.Combine(this.FileStoragePhysicalPath(), companyId);
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            return Path.Combine(folder, this.settings.PermFilePattern.Replace("{fileId}", file.WebOrbId.ToString()));
        }

        /// <summary>
        /// The permanent file name.
        /// </summary>
        /// <param name="file">
        /// The file.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string WebOrbFolderName(File file)
        {
            return Path.Combine(this.WebOrbStoragePhysicalPath(), file.WebOrbId.ToString());
        }

        #endregion
    }
}