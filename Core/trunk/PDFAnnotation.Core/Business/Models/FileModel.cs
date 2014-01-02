namespace PDFAnnotation.Core.Business.Models
{
    using System;
    using System.Collections.Generic;
    using System.Data.SqlTypes;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Imaging;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Web;

    using Castle.Core.Logging;

    using Esynctraining.Core.Business;
    using Esynctraining.Core.Business.Models;
    using Esynctraining.Core.Business.Queries;
    using Esynctraining.Core.Extensions;
    using Esynctraining.Core.Providers;
    using Esynctraining.Core.Utils;

    using NHibernate;
    using NHibernate.Criterion;
    using NHibernate.Linq;
    using NHibernate.Transform;

    using PDFAnnotation.Core.Business.Models.Annotation;
    using PDFAnnotation.Core.Domain.DTO;
    using PDFAnnotation.Core.Domain.Entities;
    using PDFAnnotation.Core.Utils;

    using File = PDFAnnotation.Core.Domain.Entities.File;

    /// <summary>
    ///     The file model.
    /// </summary>
    public class FileModel : BaseModel<File, int>
    {
        #region Fields

        /// <summary>
        ///     The drawing model.
        /// </summary>
        private readonly DrawingModel drawingModel;

        /// <summary>
        ///     The highlight strike out model.
        /// </summary>
        private readonly HighlightStrikeOutModel highlightStrikeOutModel;

        /// <summary>
        ///     The mark model.
        /// </summary>
        private readonly MarkModel markModel;

        /// <summary>
        ///     The drawing model.
        /// </summary>
        private readonly PdfModel pdfModel;

        /// <summary>
        ///     The rotation model.
        /// </summary>
        private readonly RotationModel rotationModel;

        /// <summary>
        /// The full text model.
        /// </summary>
        private readonly FullTextModel fullTextModel;

        /// <summary>
        ///     The settings.
        /// </summary>
        private readonly dynamic settings;

        /// <summary>
        ///     The shape model.
        /// </summary>
        private readonly ShapeModel shapeModel;

        /// <summary>
        ///     The text item model.
        /// </summary>
        private readonly TextItemModel textItemModel;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FileModel"/> class.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="settings">
        /// The settings.
        /// </param>
        /// <param name="drawingModel">
        /// The drawing Model.
        /// </param>
        /// <param name="pdfModel">
        /// The PDF model
        /// </param>
        /// <param name="highlightStrikeOutModel">
        /// The highlight strikeout model
        /// </param>
        /// <param name="shapeModel">
        /// The shape model
        /// </param>
        /// <param name="textItemModel">
        /// The text item model
        /// </param>
        /// <param name="rotationModel">
        /// The rotation model
        /// </param>
        /// <param name="fullTextModel">
        /// The full Text Model.
        /// </param>
        /// <param name="markModel">
        /// Marks model
        /// </param>
        public FileModel(
            IRepository<File, int> repository, 
            ApplicationSettingsProvider settings, 
            DrawingModel drawingModel, 
            PdfModel pdfModel, 
            HighlightStrikeOutModel highlightStrikeOutModel, 
            ShapeModel shapeModel, 
            TextItemModel textItemModel, 
            RotationModel rotationModel, 
            FullTextModel fullTextModel,
            MarkModel markModel)
            : base(repository)
        {
            this.settings = settings;
            this.pdfModel = pdfModel;
            this.drawingModel = drawingModel;
            this.highlightStrikeOutModel = highlightStrikeOutModel;
            this.shapeModel = shapeModel;
            this.textItemModel = textItemModel;
            this.rotationModel = rotationModel;
            this.fullTextModel = fullTextModel;
            this.markModel = markModel;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Checks supposed exhibit number, returns this or first available
        /// </summary>
        /// <param name="categoryId">
        /// The category Id.
        /// </param>
        /// <param name="topicId">
        /// The topic Id.
        /// </param>
        /// <param name="fileNumber">
        /// The exhibit number.
        /// </param>
        /// <returns>
        /// The <see cref="Nullable{Int32}"/>.
        /// </returns>
        public int CheckFileNumber(int categoryId, int topicId, int? fileNumber)
        {
            var query =
                new DefaultQueryOver<File, int>().GetQueryOver()
                    .Where(x => x.Category.Id == categoryId && x.Topic.Id == topicId)
                    .AndRestrictionOn(x => x.FileNumber)
                    .IsNotNull.TransformUsing(Transformers.DistinctRootEntity).Select(x => x.FileNumber);
            var numbers = this.Repository.FindAll<int>(query).ToList();

            int next = 1;
            if (fileNumber.HasValue && fileNumber.Value != 0)
            {
                next = Math.Max(fileNumber.Value, 1);
            }

            while (numbers.Contains(next))
            {
                next++;
            }

            return next;
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
            string permanentFileName = new FileInfo(file.FileName).Extension.ToLowerInvariant() == ".pdf"
                                           ? this.PermanentPdfName(file)
                                           : this.PermanentFileName(file);
            using (FileStream fileStream = System.IO.File.OpenWrite(permanentFileName))
            {
                string webOrbFolderName = this.WebOrbFolderName(file);
                string webOrbFile;
                if (Directory.Exists(webOrbFolderName)
                    && (webOrbFile = Directory.GetFiles(webOrbFolderName).FirstOrDefault()) != null)
                {
                    byte[] content = System.IO.File.ReadAllBytes(webOrbFile);
                    fileStream.Write(content, 0, content.Length);
                    try
                    {
                        new DirectoryInfo(webOrbFolderName).GetFiles().ToList().ForEach(f => f.Delete());
                        Directory.Delete(webOrbFolderName);
                    }
                        
                        // ReSharper disable once EmptyGeneralCatchClause
                    catch
                    {
                    }

                    if (string.IsNullOrWhiteSpace(file.FileSize))
                    {
                        file.FileSize = content.Length.ToString(CultureInfo.CurrentCulture);
                    }

                    file.Status = FileStatus.Completed;
                    this.RegisterSave(file);
                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// The resize image.
        /// </summary>
        /// <param name="content">
        /// Image content.
        /// </param>
        /// <param name="maxImageWidth">
        /// The max Image Width.
        /// </param>
        /// <param name="maxImageHeight">
        /// The max Image Height.
        /// </param>
        /// <returns>
        /// The <see cref="byte[]"/>.
        /// </returns>
        public byte[] ResizeImage(byte[] content, int maxImageWidth, int maxImageHeight)
        {
            Image img;
            if (TryParseImage(content, out img))
            {
                try
                {
                    var resizedImage = this.ResizeImage(string.Empty, maxImageWidth, maxImageHeight, img);
                    var iciPng = GetPngImageCodecInfo();
                    var encoderParameters = new EncoderParameters(1);
                    encoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, 100L);
                    var ms = new MemoryStream();
                    resizedImage.Save(ms, iciPng, encoderParameters);
                    return ms.ToArray();
                }
                catch (Exception ex)
                {
                    IoC.Resolve<ILogger>().Error("Fail during image resize", ex);
                }
            }

            return content;
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
        /// The create file.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="size">
        /// The size.
        /// </param>
        /// <param name="deponent">
        /// The deponent.
        /// </param>
        /// <param name="date">
        /// The date.
        /// </param>
        /// <param name="category">
        /// The category.
        /// </param>
        /// <param name="topic">
        /// The Topic.
        /// </param>
        /// <param name="content">
        /// file content
        /// </param>
        /// <returns>
        /// The <see cref="File"/>.
        /// </returns>
        public File CreateFile(
            string name, 
            string size, 
            string deponent, 
            DateTime date, 
            Category category = null, 
            Topic topic = null, 
            byte[] content = null)
        {
            var file = new File { FileName = name.Replace(" ", "_"), FileSize = size, TopicName = deponent };

            if (string.IsNullOrWhiteSpace(file.TopicName))
            {
                if (null != topic)
                {
                    file.TopicName = topic.FullName;
                }
            }

            if (date != default(DateTime) && date > SqlDateTime.MinValue && date < SqlDateTime.MaxValue)
            {
                file.DateCreated = date;
            }
            else
            {
                file.DateCreated = DateTime.Now;
            }

            file.Category = category;
            file.Topic = topic;

            this.RegisterSave(file, true);

            if (null != content)
            {
                string webOrbFolderName = this.WebOrbFolderName(file);
                if (!Directory.Exists(webOrbFolderName))
                {
                    Directory.CreateDirectory(webOrbFolderName);
                }

                string webOrbPath = Path.Combine(webOrbFolderName, file.FileName);
                using (FileStream fileStream = System.IO.File.OpenWrite(webOrbPath))
                {
                    fileStream.Write(content, 0, content.Length);
                }
            }

            return file;
        }

        /// <summary>
        /// The get all by events.
        /// </summary>
        /// <param name="searchPattern">
        /// The search Pattern.
        /// </param>
        /// <param name="start">
        /// The start.
        /// </param>
        /// <param name="end">
        /// The end.
        /// </param>
        /// <param name="casesId">
        /// The cases Id.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{File}"/>.
        /// </returns>
        public IEnumerable<File> GetAllByCategoriesSearched(string searchPattern, DateTime? start, DateTime? end, List<int> casesId)
        {
            var searchIds = new List<int>();
            
            var queryOver = new DefaultQueryOver<File, int>().GetQueryOver().WhereRestrictionOn(x => x.Category.Id).IsIn(casesId);

            if (start.HasValue && end.HasValue)
            {
                var startDate = start.Value;
                var endDate = end.Value;
                if (startDate > endDate)
                {
                    endDate = startDate;
                }

                startDate = startDate.AddDays(-1);
                endDate = endDate.AddDays(1);
                queryOver = queryOver.AndRestrictionOn(x => x.DateCreated).IsBetween(startDate).And(endDate);
            }

            queryOver = queryOver.AndRestrictionOn(x => x.FileNumber).IsNotNull
                    .Fetch(x => x.Category).Eager
                    .Fetch(x => x.Topic).Eager;

            if (!string.IsNullOrWhiteSpace(searchPattern))
            {
                searchIds = this.fullTextModel.Search(searchPattern, typeof(File), int.MaxValue).ToList();
                queryOver = queryOver.AndRestrictionOn(x => x.Id).IsIn(searchIds);
            }

            return searchIds.Any() ? this.Repository.FindAll(queryOver).ToList().OrderBy(x => searchIds.IndexOf(x.Id)) : this.Repository.FindAll(queryOver);
        }

        /// <summary>
        /// The get all by events.
        /// </summary>
        /// <param name="eventIds">
        /// The event ids.
        /// </param>
        /// <param name="caseId">
        /// The category Id.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{File}"/>.
        /// </returns>
        public IEnumerable<File> GetAllByEventsAndCase(List<int> eventIds, int caseId)
        {
            QueryOver<File, File> defaultQuery =
                new DefaultQueryOver<File, int>().GetQueryOver()
                    .WhereRestrictionOn(x => x.Category.Id).IsLike(caseId)
                    .Fetch(x => x.Category).Eager
                    .Fetch(x => x.Topic).Eager;
            return this.Repository.FindAll(defaultQuery);
        }

        /// <summary>
        /// The get all by contact paged.
        /// </summary>
        /// <param name="topicsIds">
        /// The topicName Ids.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{Event}"/>.
        /// </returns>
        public IEnumerable<FileTopicDTO> GetAllIdsByTopicsIds(List<int> topicsIds)
        {
            FileTopicDTO dto = null;
            var queryOver =
                new DefaultQueryOver<File, int>().GetQueryOver()
                    .WhereRestrictionOn(x => x.Topic.Id)
                    .IsIn(topicsIds)
                    .SelectList(
                        list =>
                        list.Select(x => x.Topic.Id)
                            .WithAlias(() => dto.topicId)
                            .Select(x => x.Id)
                            .WithAlias(() => dto.fileId))
                    .TransformUsing(Transformers.AliasToBean<FileTopicDTO>());
            return this.Repository.FindAll<FileTopicDTO>(queryOver);
        }

        /// <summary>
        /// The get all by ids.
        /// </summary>
        /// <param name="ids">
        /// The ids.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{Image}"/>.
        /// </returns>
        public IEnumerable<File> GetAllByGuids(List<Guid> ids)
        {
            QueryOver<File, File> queryOver =
                new DefaultQueryOver<File, int>().GetQueryOver().AndRestrictionOn(x => x.WebOrbId).IsIn(ids);
            return this.Repository.FindAll(queryOver);
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
            string fileName = this.PermanentFileName(file);
            if (System.IO.File.Exists(fileName))
            {
                return System.IO.File.ReadAllBytes(fileName);
            }

            return null;
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
        /// The get one by web orb id.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="IFutureValue{Image}"/>.
        /// </returns>
        public virtual IFutureValue<File> GetOneByWebOrbId(Guid id)
        {
            QueryOver<File> queryOver =
                new DefaultQueryOver<File, int>().GetQueryOver().Where(x => x.WebOrbId == id).Take(1);
            return this.Repository.FindOne(queryOver);
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
        public byte[] GetPDFData(File file)
        {
            if (file != null)
            {
                string fileName = this.PermanentPdfName(file);
                if (System.IO.File.Exists(fileName))
                {
                    return System.IO.File.ReadAllBytes(fileName);
                }
            }

            return null;
        }

        /// <summary>
        /// The get permanent url.
        /// </summary>
        /// <param name="file">
        /// The file.
        /// </param>
        /// <param name="pageIndex">
        /// page index if requested
        /// </param>
        /// <returns>
        /// The <see cref="string"/> url.
        /// </returns>
        public virtual string GetPermanentSwfUrl(File file, int? pageIndex = null)
        {
            if (file != null)
            {
                dynamic filePattern = !pageIndex.HasValue
                                          ? this.settings.PermSWFPattern
                                          : this.settings.PermSWFPagePattern;
                dynamic filePath = filePattern.Replace("{fileId}", file.WebOrbId.ToString())
                    .Replace("{pageIndex}", pageIndex.ToString());
                return this.settings.FileStorage + "/" + filePath;
            }

            return string.Empty;
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
                return this.settings.FileStorage + "/"
                       + this.settings.PermFilePattern.Replace("{fileId}", file.WebOrbId.ToString());
            }

            return string.Empty;
        }

        /// <summary>
        /// The get data.
        /// </summary>
        /// <param name="file">
        /// The file.
        /// </param>
        /// <param name="pageIndex">
        /// page index if requested
        /// </param>
        /// <returns>
        /// The array of bytes.
        /// </returns>
        public byte[] GetSWFData(File file, int? pageIndex = null)
        {
            string fileName = this.PermanentSWFName(file, pageIndex);
            if (System.IO.File.Exists(fileName))
            {
                return System.IO.File.ReadAllBytes(fileName);
            }

            return null;
        }

        /// <summary>
        /// Get file data as image.
        /// </summary>
        /// <param name="id">
        /// The file id.
        /// </param>
        /// <returns>
        /// The <see cref="Stream"/>.
        /// </returns>
        public byte[] GetUpdatedPDFData(string id)
        {
            int fileId = int.Parse(id);
            File file = this.GetOneById(fileId).Value;
            return this.GetUpdatedPDFData(file);
        }

        /// <summary>
        /// Get file data as image.
        /// </summary>
        /// <param name="file">
        /// The file.
        /// </param>
        /// <returns>
        /// The <see cref="Stream"/>.
        /// </returns>
        public byte[] GetUpdatedPDFData(File file)
        {
            byte[] buffer = this.GetPDFData(file);
            if (buffer != null)
            {
                IEnumerable<ATDrawing> drawings = this.drawingModel.GetAllForFile(file.Id);
                IEnumerable<ATHighlightStrikeOut> highlights = this.highlightStrikeOutModel.GetAllForFile(file.Id);
                IEnumerable<ATShape> shapes = this.shapeModel.GetAllForFile(file.Id);
                IEnumerable<ATTextItem> textItems = this.textItemModel.GetAllForFile(file.Id);
                IEnumerable<ATRotation> rotations = this.rotationModel.GetAllForFile(file.Id);
                byte[] resultBuffer = this.pdfModel.DrawOnPDF(
                    drawings,
                    highlights,
                    shapes,
                    textItems,
                    rotations,
                    buffer);

                return resultBuffer;
            }

            return null;
        }

        /// <summary>
        /// The get updated SWF data.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="byte"/>.
        /// </returns>
        public byte[] GetUpdatedSWFData(string id)
        {
            int fileId = int.Parse(id);
            File file = this.GetOneById(fileId).Value;
            return this.GetUpdatedSWFData(file);
        }

        /// <summary>
        /// Get file data as image.
        /// </summary>
        /// <param name="file">
        /// The file.
        /// </param>
        /// <returns>
        /// The <see cref="Stream"/>.
        /// </returns>
        public byte[] GetUpdatedSWFData(File file)
        {
            byte[] buffer = this.GetPDFData(file);
            if (buffer != null)
            {
                List<ATDrawing> drawings = this.drawingModel.GetAllForFile(file.Id).ToList();
                drawings.Where(o => o.Mark.DateChanged > file.DateModified)
                    .ToList()
                    .ForEach(o => this.drawingModel.RegisterDelete(o));
                drawings = drawings.Where(o => o.Mark.DateChanged <= file.DateModified).ToList();

                List<ATHighlightStrikeOut> highlights = this.highlightStrikeOutModel.GetAllForFile(file.Id).ToList();
                highlights.Where(o => o.Mark.DateChanged > file.DateModified)
                    .ToList()
                    .ForEach(o => this.highlightStrikeOutModel.RegisterDelete(o));
                highlights = highlights.Where(o => o.Mark.DateChanged <= file.DateModified).ToList();

                List<ATShape> shapes = this.shapeModel.GetAllForFile(file.Id).ToList();
                shapes.Where(o => o.Mark.DateChanged > file.DateModified)
                    .ToList()
                    .ForEach(o => this.shapeModel.RegisterDelete(o));
                shapes = shapes.Where(o => o.Mark.DateChanged <= file.DateModified).ToList();

                List<ATTextItem> textItems = this.textItemModel.GetAllForFile(file.Id).ToList();
                textItems.Where(o => o.Mark.DateChanged > file.DateModified)
                    .ToList()
                    .ForEach(o => this.textItemModel.RegisterDelete(o));
                textItems = textItems.Where(o => o.Mark.DateChanged <= file.DateModified).ToList();

                List<ATRotation> rotations = this.rotationModel.GetAllForFile(file.Id).ToList();
                rotations.Where(o => o.Mark.DateChanged > file.DateModified)
                    .ToList()
                    .ForEach(o => this.rotationModel.RegisterDelete(o));
                rotations = rotations.Where(o => o.Mark.DateChanged <= file.DateModified).ToList();

                byte[] pdfBuffer = this.pdfModel.DrawOnPDF(drawings, highlights, shapes, textItems, rotations, buffer);
                string tmpName = Guid.NewGuid().ToString();
                string tmpPDF = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), tmpName + ".pdf");
                string tmpSWF = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), tmpName + ".swf");

                using (var fs = new FileStream(tmpPDF, FileMode.CreateNew, FileAccess.Write))
                {
                    fs.Write(pdfBuffer, 0, pdfBuffer.Length);
                }

                IoC.Resolve<Pdf2SwfConverter>().Convert(tmpPDF, tmpSWF);
                if (System.IO.File.Exists(tmpSWF))
                {
                    return System.IO.File.ReadAllBytes(tmpSWF);
                }
            }

            return null;
        }

        /// <summary>
        /// Get file data as image.
        /// </summary>
        /// <param name="originId">
        /// The origin Id.
        /// </param>
        /// <param name="newId">
        /// The new Id.
        /// </param>
        public void MoveMarksToNewFile(int originId, int newId)
        {
            File newFile = this.GetOneById(newId).Value;
            File originFile = this.GetOneById(originId).Value;

            foreach (ATDrawing o in this.drawingModel.GetAllForFile(originId))
            {
                o.Mark.File = null;
                ATMark m = o.Mark.DeepClone();
                o.Mark.File = originFile;

                m.Id = Guid.NewGuid();
                m.File = newFile;

                ATMark originMark = o.Mark;
                o.Mark = null;
                ATDrawing @new = o.DeepClone();
                o.Mark = originMark;

                @new.Id = default(int);
                @new.Mark = m;

                this.markModel.RegisterSave(m);
                this.drawingModel.RegisterSave(@new);
            }

            foreach (ATRotation o in this.rotationModel.GetAllForFile(originId))
            {
                o.Mark.File = null;
                ATMark m = o.Mark.DeepClone();
                o.Mark.File = originFile;

                m.Id = Guid.NewGuid();
                m.File = newFile;

                ATMark originMark = o.Mark;
                o.Mark = null;
                ATRotation @new = o.DeepClone();
                o.Mark = originMark;

                @new.Id = default(int);
                @new.Mark = m;

                this.markModel.RegisterSave(m);
                this.rotationModel.RegisterSave(@new);
            }

            foreach (ATHighlightStrikeOut o in this.highlightStrikeOutModel.GetAllForFile(originId))
            {
                o.Mark.File = null;
                ATMark m = o.Mark.DeepClone();
                o.Mark.File = originFile;

                m.Id = Guid.NewGuid();
                m.File = newFile;

                ATMark originMark = o.Mark;
                o.Mark = null;
                ATHighlightStrikeOut @new = o.DeepClone();
                o.Mark = originMark;

                @new.Id = default(int);
                @new.Mark = m;

                this.markModel.RegisterSave(m);
                this.highlightStrikeOutModel.RegisterSave(@new);
            }

            foreach (ATShape o in this.shapeModel.GetAllForFile(originId))
            {
                o.Mark.File = null;
                ATMark m = o.Mark.DeepClone();
                o.Mark.File = originFile;

                m.Id = Guid.NewGuid();
                m.File = newFile;

                ATMark originMark = o.Mark;
                o.Mark = null;
                ATShape @new = o.DeepClone();
                o.Mark = originMark;

                @new.Id = default(int);
                @new.Mark = m;

                this.markModel.RegisterSave(m);
                this.shapeModel.RegisterSave(@new);
            }

            foreach (ATTextItem o in this.textItemModel.GetAllForFile(originId))
            {
                o.Mark.File = null;
                ATMark m = o.Mark.DeepClone();
                o.Mark.File = originFile;

                m.Id = Guid.NewGuid();
                m.File = newFile;

                ATMark originMark = o.Mark;
                o.Mark = null;
                ATTextItem @new = o.DeepClone();
                o.Mark = originMark;

                @new.Id = default(int);
                @new.Mark = m;

                this.markModel.RegisterSave(m);
                this.textItemModel.RegisterSave(@new);
            }

            List<ATDrawing> drawings = this.drawingModel.GetAllForFile(originId).ToList();
            drawings.Where(o => o.Mark.DateChanged > originFile.DateModified)
                .ToList()
                .ForEach(o => this.drawingModel.RegisterDelete(o));

            List<ATHighlightStrikeOut> highlights = this.highlightStrikeOutModel.GetAllForFile(originId).ToList();
            highlights.Where(o => o.Mark.DateChanged > originFile.DateModified)
                .ToList()
                .ForEach(o => this.highlightStrikeOutModel.RegisterDelete(o));

            List<ATShape> shapes = this.shapeModel.GetAllForFile(originId).ToList();
            shapes.Where(o => o.Mark.DateChanged > originFile.DateModified)
                .ToList()
                .ForEach(o => this.shapeModel.RegisterDelete(o));

            List<ATTextItem> textItems = this.textItemModel.GetAllForFile(originId).ToList();
            textItems.Where(o => o.Mark.DateChanged > originFile.DateModified)
                .ToList()
                .ForEach(o => this.textItemModel.RegisterDelete(o));

            List<ATRotation> rotations = this.rotationModel.GetAllForFile(originId).ToList();
            rotations.Where(o => o.Mark.DateChanged > originFile.DateModified)
                .ToList()
                .ForEach(o => this.rotationModel.RegisterDelete(o));

            this.drawingModel.Flush();
            this.highlightStrikeOutModel.Flush();
            this.shapeModel.Flush();
            this.textItemModel.Flush();
            this.rotationModel.Flush();

            this.markModel.Flush();
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
        public string PermanentPdfName(File file)
        {
            string folder = this.FileStoragePhysicalPath();
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            dynamic path = Path.Combine(
                folder, 
                this.settings.PermPDFPattern.Replace("{fileId}", file.WebOrbId.ToString()));
            var di = new DirectoryInfo(Path.GetDirectoryName(path) ?? this.FileStoragePhysicalPath());
            if (!di.Exists)
            {
                di.Create();
            }

            return path;
        }

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
            string filePath = this.PermanentFileName(file);
            if (System.IO.File.Exists(filePath))
            {
                try
                {
                    System.IO.File.Delete(filePath);
                }
                    
                    // ReSharper disable once EmptyGeneralCatchClause
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

            entity.DateModified = DateTime.Now;
            base.RegisterSave(entity, flush);
        }

        /// <summary>
        /// The get one by file id.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="File"/>.
        /// </returns>
        public File GetOneByFileId(string id)
        {
            int idVal;
            Guid idGuid;
            if (Guid.TryParse(id, out idGuid))
            {
                return this.GetOneByWebOrbId(idGuid).Value;
            }

            if (int.TryParse(id, out idVal))
            {
                return this.GetOneById(idVal).Value;
            }

            return null;
        }

        /// <summary>
        /// Sets auto-incremented display file name if needed
        /// </summary>
        /// <param name="entity">
        /// The <see cref="File"/>
        /// </param>
        /// <param name="exhibitNumber">
        /// exhibit number
        /// </param>
        public void SetDisplayName(File entity, int exhibitNumber)
        {
            Topic topic = entity.Topic;
            var availiblePatternOptions = new Dictionary<string, string>();
            var file = new FileInfo(entity.FileName);
            availiblePatternOptions.Add("{fileName}", file.Name);
            availiblePatternOptions.Add("{extension}", file.Extension);
            availiblePatternOptions.Add("{number}", string.Format("{0:#0000}", exhibitNumber));
            availiblePatternOptions.Add("{topic}", topic.FullName.Trim().Replace(" ", "_"));
            string displayName = availiblePatternOptions.Keys.Aggregate((string)this.settings.DisplayNamePattern, (current, optionKey) => current.Replace(optionKey, availiblePatternOptions[optionKey]));
            entity.DisplayName = displayName;
            entity.FileNumber = exhibitNumber;
        }

        #endregion

        #region Methods

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
        /// The file storage physical path.
        /// </summary>
        /// <param name="settings">
        /// The settings.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        internal static string FileStoragePhysicalPath(dynamic settings)
        {
            dynamic fileStorage = settings.FileStorage;
            return fileStorage.StartsWith("~") ? HttpContext.Current.Server.MapPath(fileStorage) : fileStorage;
        }

        /// <summary>
        ///     The file storage physical path.
        /// </summary>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        internal string FileStoragePhysicalPath()
        {
            return FileStoragePhysicalPath(this.settings);
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
        internal string PermanentFileName(File file)
        {
            string folder = this.FileStoragePhysicalPath();
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
        internal string WebOrbFolderName(File file)
        {
            return Path.Combine(this.WebOrbStoragePhysicalPath(), file.WebOrbId.ToString());
        }

        /// <summary>
        ///     The file storage physical path.
        /// </summary>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        internal string WebOrbStoragePhysicalPath()
        {
            dynamic fileStorage = this.settings.WebOrbStorage;
            return fileStorage.StartsWith("~") ? HttpContext.Current.Server.MapPath(fileStorage) : fileStorage;
        }

        /// <summary>
        /// The permanent file name.
        /// </summary>
        /// <param name="file">
        /// The file.
        /// </param>
        /// <param name="pageIndex">
        /// page index if requested
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string PermanentSWFName(File file, int? pageIndex = null)
        {
            string folder = this.FileStoragePhysicalPath();
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            dynamic filePattern = !pageIndex.HasValue ? this.settings.PermSWFPattern : this.settings.PermSWFPagePattern;
            dynamic filePath = filePattern.Replace("{fileId}", file.WebOrbId.ToString())
                .Replace("{pageIndex}", pageIndex.ToString());

            dynamic path = Path.Combine(folder, filePath);
            var di = new DirectoryInfo(Path.GetDirectoryName(path) ?? this.FileStoragePhysicalPath());
            if (!di.Exists)
            {
                di.Create();
            }

            return path;
        }

        #endregion
    }
}