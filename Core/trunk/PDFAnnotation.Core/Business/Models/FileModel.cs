namespace PDFAnnotation.Core.Business.Models
{
    using System;
    using System.Collections.Generic;
    using System.Data.SqlTypes;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Web;

    using Esynctraining.Core.Business;
    using Esynctraining.Core.Business.Models;
    using Esynctraining.Core.Business.Queries;
    using Esynctraining.Core.Domain.Entities;
    using Esynctraining.Core.Extensions;
    using Esynctraining.Core.Providers;
    using Esynctraining.Core.Utils;

    using Iesi.Collections.Generic;

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
        /// <param name="category">
        /// </param>
        /// <param name="topic">
        /// </param>
        /// <param name="fileNumber">
        /// The exhibit number.
        /// </param>
        /// <returns>
        /// The <see cref="Nullable{Int32}"/>.
        /// </returns>
        public int CheckFileNumber(Category category, Topic topic, int? fileNumber)
        {
            List<File> files =
                category.Return(x => x.Files.Where(f => f.Topic.With(w => w.Id) == topic.Id), new HashedSet<File>())
                    .Distinct().ToList();

            int nextNumber;
            if (category.With(c => c.IsFileNumbersAutoIncremented) || !fileNumber.HasValue || fileNumber.Value == 0)
            {
                int lastNumber = files.Where(f => f.FileNumber.HasValue).Max(f => f.FileNumber) ?? 0;
                nextNumber = lastNumber + 1;
            }
            else
            {
                List<int> numbers = // ReSharper disable once PossibleInvalidOperationException
                    files.Where(f => f.FileNumber.HasValue).Select(f => f.FileNumber.Value).ToList();
                int n = Math.Max(fileNumber.Value, 1);
                while (numbers.Contains(n))
                {
                    n++;
                }

                nextNumber = n;
            }

            return nextNumber;
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
        /// <param name="event">
        /// The event
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
            var file = new File { FileName = name, FileSize = size, TopicName = deponent };

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
        public IEnumerable<File> GetAllByCasesSearched(string searchPattern, DateTime? start, DateTime? end, List<int> casesId)
        {
            var searchIds = new List<int>();
            
            var queryOver =
                new DefaultQueryOver<File, int>().GetQueryOver()
                    .Where(Restrictions.On<File>(x => x.Category.Id).IsIn(casesId));

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
                    .Where(Restrictions.On<File>(x => x.Category.Id).IsLike(caseId))
                    .Fetch(x => x.Category).Eager
                    .Fetch(x => x.Topic).Eager;
            return this.Repository.FindAll(defaultQuery);
        }

        /// <summary>
        /// The get all by contact paged.
        /// </summary>
        /// <param name="topicsIds">
        /// The witness Ids.
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
            string fileName = this.PermanentPdfName(file);
            if (System.IO.File.Exists(fileName))
            {
                return System.IO.File.ReadAllBytes(fileName);
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
        /// Get file data as image.
        /// </summary>
        /// <param name="id">
        /// The file id.
        /// </param>
        /// <returns>
        /// The <see cref="Stream"/>.
        /// </returns>
        public byte[] GetUpdatedSWFData(string id)
        {
            int fileId = int.Parse(id);
            File file = this.GetOneById(fileId).Value;

            byte[] buffer = this.GetPDFData(file);
            if (buffer != null)
            {
                List<ATDrawing> drawings = this.drawingModel.GetAllForFile(fileId).ToList();
                drawings.Where(o => o.Mark.DateChanged > file.DateModified)
                    .ToList()
                    .ForEach(o => this.drawingModel.RegisterDelete(o));
                drawings = drawings.Where(o => o.Mark.DateChanged <= file.DateModified).ToList();

                List<ATHighlightStrikeOut> highlights = this.highlightStrikeOutModel.GetAllForFile(fileId).ToList();
                highlights.Where(o => o.Mark.DateChanged > file.DateModified)
                    .ToList()
                    .ForEach(o => this.highlightStrikeOutModel.RegisterDelete(o));
                highlights = highlights.Where(o => o.Mark.DateChanged <= file.DateModified).ToList();

                List<ATShape> shapes = this.shapeModel.GetAllForFile(fileId).ToList();
                shapes.Where(o => o.Mark.DateChanged > file.DateModified)
                    .ToList()
                    .ForEach(o => this.shapeModel.RegisterDelete(o));
                shapes = shapes.Where(o => o.Mark.DateChanged <= file.DateModified).ToList();

                List<ATTextItem> textItems = this.textItemModel.GetAllForFile(fileId).ToList();
                textItems.Where(o => o.Mark.DateChanged > file.DateModified)
                    .ToList()
                    .ForEach(o => this.textItemModel.RegisterDelete(o));
                textItems = textItems.Where(o => o.Mark.DateChanged <= file.DateModified).ToList();

                List<ATRotation> rotations = this.rotationModel.GetAllForFile(fileId).ToList();
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
            string displayName = string.Format(
                "{0:#0000}_{1}{2}", 
                exhibitNumber, 
                topic.FullName.Trim().Replace(" ", "_"), 
                new FileInfo(entity.FileName).Extension);
            entity.DisplayName = displayName;
            entity.FileNumber = exhibitNumber;
        }

        #endregion

        #region Methods

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

        /// <summary>
        /// The reassign.
        /// </summary>
        /// <param name="origin">
        /// The origin.
        /// </param>
        /// <param name="mark">
        /// The mark.
        /// </param>
        /// <param name="newFile">
        /// The new file.
        /// </param>
        /// <param name="save">
        /// The save.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        private void Reassign<T>(T origin, ATMark mark, File newFile, Action<T> save) where T : Entity
        {
            mark.File = null;
            ATMark m = mark.DeepClone();
            m.Id = Guid.NewGuid();
            m.File = newFile;

            T @new = origin.DeepClone();
            @new.Id = default(int);

            save(@new);
        }

        #endregion
    }
}