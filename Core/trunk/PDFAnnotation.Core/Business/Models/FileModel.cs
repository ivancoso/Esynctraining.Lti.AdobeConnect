using System.Text.RegularExpressions;

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
    using Esynctraining.Core.Enums;
    using Esynctraining.Core.Extensions;
    using Esynctraining.Core.Providers;
    using Esynctraining.Core.Utils;
    using Esynctraining.PdfProcessor;

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
    public class FileModel : BaseModel<File, Guid>
    {
        #region Fields

        /// <summary>
        /// The dictionary locker.
        /// </summary>
        private static readonly object DictionaryLocker = new object();

        /// <summary>
        /// The object lockers.
        /// </summary>
        private static readonly Dictionary<string, object> ObjectLockers = new Dictionary<string, object>();

        /// <summary>
        ///     The drawing model.
        /// </summary>
        private readonly DrawingModel drawingModel;

        /// <summary>
        ///     The full text model.
        /// </summary>
        private readonly FullTextModel fullTextModel;

        /// <summary>
        ///     The highlight strike out model.
        /// </summary>
        private readonly HighlightStrikeOutModel highlightStrikeOutModel;

        /// <summary>
        ///     The logger.
        /// </summary>
        private readonly ILogger logger;

        /// <summary>
        ///     The mark model.
        /// </summary>
        private readonly MarkModel markModel;

        /// <summary>
        ///     The drawing model.
        /// </summary>
        private readonly PdfModel pdfModel;

        /// <summary>
        ///     The PDF processor helper.
        /// </summary>
        private readonly PdfProcessorHelper pdfProcessorHelper;

        /// <summary>
        ///     The rotation model.
        /// </summary>
        private readonly RotationModel rotationModel;

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
        /// <param name="pdfProcessorHelper">
        /// The pdf Processor Helper.
        /// </param>
        /// <param name="markModel">
        /// Marks model
        /// </param>
        /// <param name="logger">
        /// The error logger
        /// </param>
        public FileModel(
            IRepository<File, Guid> repository, 
            ApplicationSettingsProvider settings, 
            DrawingModel drawingModel, 
            PdfModel pdfModel, 
            HighlightStrikeOutModel highlightStrikeOutModel, 
            ShapeModel shapeModel, 
            TextItemModel textItemModel, 
            RotationModel rotationModel, 

            PdfProcessorHelper pdfProcessorHelper, 
			FullTextModel fullTextModel,            MarkModel markModel, 
            ILogger logger)
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
            this.pdfProcessorHelper = pdfProcessorHelper;
            this.markModel = markModel;
            this.logger = logger;
        }

        #endregion

        #region Public Methods and Operators

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
        public bool ExistAndNotEmptySWFData(File file, int? pageIndex = null)
        {
            string fileName = this.PermanentSWFName(file, pageIndex);
            return System.IO.File.Exists(fileName) && new FileInfo(fileName).Length > 0;
        }

        /// <summary>
        /// The permanent file name.
        /// </summary>
        /// <param name="file">
        /// The file.
        /// </param>
        /// <param name="settingz">
        /// The settings.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static List<FileInfo> EnumerateSWFOriginalPages(File file, dynamic settingz)
        {
            return FileModel.EnumerateSWFOriginalPages(
                file.Id,
                (string)FileModel.FileStoragePhysicalPath(settingz),
                (string)settingz.PermSWFPattern,
                (string)settingz.PermSWFPagePattern);
        }

        /// <summary>
        /// The permanent file name.
        /// </summary>
        /// <param name="fileId">
        /// The file Id.
        /// </param>
        /// <param name="fileStoragePhysicalPath">
        /// The file Storage Physical Path.
        /// </param>
        /// <param name="permSWFPattern">
        /// The perm SWF Pattern.
        /// </param>
        /// <param name="permSWFPagePattern">
        /// The perm SWF Page Pattern.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static List<FileInfo> EnumerateSWFOriginalPages(Guid fileId, string fileStoragePhysicalPath, string permSWFPattern, string permSWFPagePattern)
        {
            var regex = new Regex(@"^\d*\.swf", RegexOptions.Compiled);
            string folder = fileStoragePhysicalPath;
            if (!Directory.Exists(folder))
            {
                return new List<FileInfo>();
            }

            dynamic filePattern = permSWFPagePattern;
            dynamic filePath = filePattern.Replace("{fileId}", fileId.ToString()).Replace("{pageIndex}", "*");
            dynamic path = Path.Combine(folder, filePath);
            var di = new DirectoryInfo(Path.GetDirectoryName(path) ?? folder);
            if (!di.Exists)
            {
                return new List<FileInfo>();
            }

            string exceptFullSwfName = permSWFPattern.Replace("{fileId}", string.Empty).Trim("\\".ToCharArray());
            var pages = di.EnumerateFiles("*.swf").Where(x => regex.Match(x.Name).Success && !string.Equals(x.Name, exceptFullSwfName, StringComparison.InvariantCultureIgnoreCase)).ToList();
            return pages;
        }

        public byte[] ReadAllBytes(string filePath)
        {
            byte[] oFileBytes = null;
            using (FileStream fs = System.IO.File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                int numBytesToRead = Convert.ToInt32(fs.Length);
                oFileBytes = new byte[(numBytesToRead)];
                fs.Read(oFileBytes, 0, numBytesToRead);
            }

            return oFileBytes;
        }

        /// <summary>
        /// The update witness files.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int UpdateTopicsFiles(Topic entity)
        {
            int counter = 0;
            var query = new DefaultQueryOver<File, Guid>().GetQueryOver().Where(x => x.Topic != null && x.Topic.Id == entity.Id);
            var files = this.Repository.FindAll(query).ToList();
            foreach (var file in files)
            {
                counter++;
                file.TopicName = entity.FullName;
                this.RegisterSave(file, false, true);
            }

            return counter;
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
            QueryOver<File, File> query =
                new DefaultQueryOver<File, Guid>().GetQueryOver()
                    .Where(x => x.Category.Id == categoryId && x.Topic.Id == topicId)
                    .AndRestrictionOn(x => x.FileNumber).IsNotNull
                    .And(x => x.IsOriginal == null || x.IsOriginal == false)
                    .TransformUsing(Transformers.DistinctRootEntity)
                    .Select(x => x.FileNumber);
            List<int> numbers = this.Repository.FindAll<int>(query).ToList();

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
        /// The check exhibit number.
        /// </summary>
        /// <param name="case">
        /// The case.
        /// </param>
        /// <param name="event">
        /// The event.
        /// </param>
        /// <param name="exhibitNumber">
        /// The exhibit number.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int CheckExhibitNumber(Category @case = null, Topic @event = null, int? exhibitNumber = null)
        {
            @case = @event.Return(x => x.Category, @case);
            List<File> files = @case.Return(x => x.Files.Where(f => f.IsOriginal == null || f.IsOriginal == false), new HashSet<File>())
                    .Union(@case.Return(c => c.Topics.SelectMany(e => e.Files.Where(f => f.IsOriginal == null || f.IsOriginal == false)), new HashSet<File>()))
                    .Distinct().ToList();
            int nextNumber;
            if (@case.Return(c => c.IsFileNumbersAutoIncremented, @event == null && @case == null) || !exhibitNumber.HasValue || exhibitNumber.Value == 0)
            {
                int lastNumber = files.Where(f => f.FileNumber.HasValue).Max(f => f.FileNumber) ?? 0;
                nextNumber = lastNumber + 1;
            }
            else
            {
                List<int> numbers = // ReSharper disable once PossibleInvalidOperationException
                    files.Where(f => f.FileNumber.HasValue).Select(f => f.FileNumber.Value).ToList();
                int n = Math.Max(exhibitNumber.Value, 1);
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
            bool isPdf = new FileInfo(file.FileName).Extension.ToLowerInvariant() == ".pdf";
            string permanentFileName = isPdf ? this.PermanentPdfName(file) : this.PermanentFileName(file);
            string webOrbFolderName = this.WebOrbFolderName(file);
            string webOrbFile;
            if (Directory.Exists(webOrbFolderName) && (webOrbFile = Directory.GetFiles(webOrbFolderName).FirstOrDefault()) != null)
            {
                if (string.IsNullOrWhiteSpace(file.FileSize))
                {
                    file.FileSize = new FileInfo(webOrbFile).Length.ToString(CultureInfo.CurrentCulture);
                }

                if (isPdf)
                {
                    file.NumberOfPages = this.pdfModel.GetNumberOfPages(webOrbFile);
                    if (file.NumberOfPages == 0)
                    {
                        file.NumberOfPages = this.pdfModel.GetNumberOfPagesUsingStream(webOrbFile);
                    }
                }

                if (System.IO.File.Exists(permanentFileName))
                {
                    System.IO.File.Delete(permanentFileName);
                }

                System.IO.File.Copy(webOrbFile, permanentFileName);
                this.ClearDirectoryAndRemoveItSafely(webOrbFolderName);
                file.UploadFileStatus = UploadFileStatus.Rendering;
                this.RegisterSave(file, true);
                return true;
            }

            return false;
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
        /// <param name="user">
        /// The user.
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
            Contact user = null,
            byte[] content = null)
        {
            var file = new File { FileName = name.Replace(" ", "_"), FileSize = size, TopicName = deponent };

            if (string.IsNullOrWhiteSpace(file.TopicName))
            {
                if (null != topic)
                {
                    file.TopicName = topic.FullName;
                }

                if (null != user)
                {
                    file.TopicName = user.FullName;
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
            file.User = user;
            file.Status = FileStatus.Created;
            file.UploadFileStatus = UploadFileStatus.Uploading;

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
        public IEnumerable<File> GetAllByCategoriesSearched(
            string searchPattern, 
            DateTime? start, 
            DateTime? end, 
            List<int> casesId)
        {
            var searchIds = new List<Guid>();

            QueryOver<File, File> queryOver =
                new DefaultQueryOver<File, Guid>().GetQueryOver()
                .Where(x => (x.Status == FileStatus.UploadCompleted || x.Status == FileStatus.Saved) && (x.UploadFileStatus == UploadFileStatus.Converted || x.UploadFileStatus == UploadFileStatus.ConvertingPagesFailed))
                .AndRestrictionOn(x => x.Category.Id).IsIn(casesId);

            if (start.HasValue && end.HasValue)
            {
                DateTime startDate = start.Value;
                DateTime endDate = end.Value;
                if (startDate > endDate)
                {
                    endDate = startDate;
                }

                startDate = startDate.AddDays(-1);
                endDate = endDate.AddDays(1);
                queryOver = queryOver.AndRestrictionOn(x => x.DateCreated).IsBetween(startDate).And(endDate);
            }

            queryOver =
                queryOver.AndRestrictionOn(x => x.FileNumber)
                    .IsNotNull.Fetch(x => x.Category)
                    .Eager.Fetch(x => x.Topic)
                    .Eager;

            if (!string.IsNullOrWhiteSpace(searchPattern))
            {
                searchIds = this.fullTextModel.SearchGuids(searchPattern, typeof(File), int.MaxValue).ToList();
                queryOver = queryOver.AndRestrictionOn(x => x.Id).IsIn(searchIds);
            }

            return searchIds.Any()
                       ? this.Repository.FindAll(queryOver).ToList().OrderBy(x => searchIds.IndexOf(x.Id))
                       : this.Repository.FindAll(queryOver);
        }

        /// <summary>
        /// The get all by events.
        /// </summary>
        /// <param name="user">
        /// The user.
        /// </param>
        /// <param name="meetingUrl">
        /// The meeting url.
        /// </param>
        /// <param name="status">
        /// The status.
        /// </param>
        /// <param name="includeShared">
        /// The include Shared.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{File}"/>.
        /// </returns>
        public IEnumerable<File> GetAllByUserAndMeetingAndStatus(
            Contact user,
            string meetingUrl,
            FileStatus status,
            bool includeShared = false)
        {
            var result = new List<File>();
            int userId = user.With(x => x.Id);
            if (userId != 0)
            {
                var defaultQuery =
                    new DefaultQueryOver<File, Guid>().GetQueryOver()
                        .Where(x => x.User != null && x.User.Id == userId && x.Status == status)
                        .Fetch(x => x.User)
                        .Eager;

                if (!string.IsNullOrWhiteSpace(meetingUrl))
                {
                    defaultQuery =
                        defaultQuery.WhereRestrictionOn(x => x.AcMeetingUrl)
                            .IsNotNull.AndRestrictionOn(x => x.AcMeetingUrl)
                            .IsInsensitiveLike(meetingUrl, MatchMode.Exact);
                }
                else
                {
                    defaultQuery = defaultQuery.WhereRestrictionOn(x => x.AcMeetingUrl).IsNull;
                }

                if (string.IsNullOrWhiteSpace(meetingUrl) || !includeShared)
                {
                    return this.Repository.FindAll(defaultQuery);
                }

                result = this.Repository.FindAll(defaultQuery).ToList();
            }

            if (!string.IsNullOrWhiteSpace(meetingUrl) && includeShared)
            {
                var secondOver =
                    new DefaultQueryOver<File, Guid>().GetQueryOver()
                        .Where(x => x.User != null && x.IsShared == true && x.User.Id != userId && x.Status == status);

                secondOver =
                    secondOver.WhereRestrictionOn(x => x.AcMeetingUrl)
                        .IsNotNull.AndRestrictionOn(x => x.AcMeetingUrl)
                        .IsInsensitiveLike(meetingUrl, MatchMode.Exact);

                secondOver = secondOver.Fetch(x => x.User).Eager;

                return result.Union(this.Repository.FindAll(secondOver));
            }

            return result;
        }

//        /// <summary>
//        /// The get all by events.
//        /// </summary>
//        /// <param name="user">
//        /// The user.
//        /// </param>
//        /// <param name="status">
//        /// The status.
//        /// </param>
//        /// <param name="includeShared">
//        /// The include Shared.
//        /// </param>
//        /// <returns>
//        /// The <see cref="IEnumerable{File}"/>.
//        /// </returns>
//        public IEnumerable<File> GetAllByUserAndStatus(Contact user, FileStatus status, bool includeShared = false)
//        {
//            const int FirmContactAdminTypeId = (int)ContactTypeEnum.CompanyAdmin;
//            const int FirmContactTypeId = (int)ContactTypeEnum.Contact;
//            int userId = user.Id;
//            var defaultQuery =
//                new DefaultQueryOver<File, Guid>().GetQueryOver()
//                    .Where(x => x.User != null && x.User.Id == userId && x.Status == status)
//                    .Fetch(x => x.User)
//                    .Eager;
//            if (!includeShared || !user.CompanyContacts.Any())
//            {
//                return this.Repository.FindAll(defaultQuery);
//            }
//
//            var result = this.Repository.FindAll(defaultQuery).ToList();
//            var firmIds = user.CompanyContacts.Select(x => x.Company.Id).ToList();
//            var subQuesry = QueryOver.Of<CompanyContact>()
//                    .Where(x => x.ContactType.Id == FirmContactTypeId || x.ContactType.Id == FirmContactAdminTypeId)
//                    .AndRestrictionOn(x => x.Company.Id)
//                    .IsIn(firmIds).Select(x => x.Contact.Id);
//            var secondOver = new DefaultQueryOver<File, Guid>().GetQueryOver()
//                    .Where(x => x.User != null && x.IsShared == true && x.User.Id != userId)
//                    .WithSubquery.WhereProperty(x => x.User.Id)
//                    .In(subQuesry)
//                    .Fetch(x => x.User)
//                    .Eager;
//            result.AddRange(this.Repository.FindAll(secondOver));
//            return result;
//        }

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
                new DefaultQueryOver<File, Guid>().GetQueryOver().AndRestrictionOn(x => x.Id).IsIn(ids);
            return this.Repository.FindAll(queryOver);
        }

        /// <summary>
        /// The get all by statuses to clear before date.
        /// </summary>
        /// <param name="fileStatuses">
        /// The file statuses.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{File}"/>.
        /// </returns>
        public IEnumerable<File> GetAllByStatuses(FileStatus[] fileStatuses)
        {
            QueryOver<File> queryOver = new DefaultQueryOver<File, Guid>().GetQueryOver().WhereRestrictionOn(x => x.Status).IsIn(fileStatuses);
            return this.Repository.FindAll(queryOver);
        }

        /// <summary>
        /// The get all by statuses to clear before date.
        /// </summary>
        /// <param name="fileStatuses">
        /// The file statuses.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{File}"/>.
        /// </returns>
        public IEnumerable<File> GetAllByUploadFileStatuses(UploadFileStatus[] fileStatuses)
        {
            QueryOver<File> queryOver = new DefaultQueryOver<File, Guid>().GetQueryOver().WhereRestrictionOn(x => x.UploadFileStatus).IsIn(fileStatuses);
            return this.Repository.FindAll(queryOver);
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
            QueryOver<File, File> queryOver =
                new DefaultQueryOver<File, Guid>().GetQueryOver()
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
                return ReadAllBytes(fileName);
            }

            return null;
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
            Guid idGuid;
            if (Guid.TryParse(id, out idGuid))
            {
                return this.GetOneByWebOrbId(idGuid).Value;
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
                new DefaultQueryOver<File, Guid>().GetQueryOver().Where(x => x.Id == id).Take(1);
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
                    return ReadAllBytes(fileName);
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
                dynamic filePath = filePattern.Replace("{fileId}", file.Id.ToString()).Replace("{pageIndex}", pageIndex.ToString());
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
                return this.settings.FileStorage + "/" + this.settings.PermFilePattern.Replace("{fileId}", file.Id.ToString());
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
                return ReadAllBytes(fileName);
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
        /// The <see cref="System.IO.Stream"/>.
        /// </returns>
        public byte[] GetUpdatedPDFData(string id)
        {
            Guid fileId = Guid.Parse(id);
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
        /// The <see cref="System.IO.Stream"/>.
        /// </returns>
        public byte[] GetUpdatedPDFData(File file)
        {
            try
            {
                byte[] buffer = this.GetPDFData(file);
                if (buffer != null && file.DateModified.HasValue && file.DateModified > file.DateCreated)
{
                    string tmpName = file.DateModified.Value.ToMicroSeconds().ToString(CultureInfo.InvariantCulture); // based on date
                    var baseDir = Path.Combine(this.FileStoragePhysicalPath(), file.Id.ToString());
                    string tmpPDF = Path.Combine(baseDir, tmpName + ".pdf");
                    if (!System.IO.File.Exists(tmpPDF))
                    {
                        lock (GetDictionaryLocker(tmpPDF))
                        {
                            if (!System.IO.File.Exists(tmpPDF))
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

    using (var fs = new FileStream(tmpPDF, FileMode.Create, FileAccess.Write))
                                {
                                    fs.Write(resultBuffer, 0, resultBuffer.Length);
                                }

                    return resultBuffer;
                }
            }
 }
                    else
                    {
                        return ReadAllBytes(tmpPDF);
                    }
                }

                if (file != null && buffer != null)
                {
                    return buffer;
                }
            }
            catch (Exception ex)
            {
                this.logger.Error("Unexpected error during pdf read/write:" + ex);
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
            Guid fileId = Guid.Parse(id);
            File file = this.GetOneById(fileId).Value;
            return this.GetUpdatedSWFDataWithStatus(file).With(x => x.Item2);
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
        public byte[] GetUpdatedSWFData(File file)
        {
            return this.GetUpdatedSWFDataWithStatus(file).With(x => x.Item2);
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
        public System.Tuple<int, byte[]> GetUpdatedSWFDataWithStatus(string id)
        {
            Guid fileId = Guid.Parse(id);
            File file = this.GetOneById(fileId).Value;
            return this.GetUpdatedSWFDataWithStatus(file);
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
        public System.Tuple<int, byte[]> GetUpdatedSWFDataWithStatus(File file)
        {
            byte[] buffer = this.GetPDFData(file);
            if (buffer != null && file.DateModified.HasValue && file.DateModified > file.DateCreated)
            {
                string tmpName = file.DateModified.Value.ToMicroSeconds().ToString(CultureInfo.InvariantCulture); // based on date
                var baseDir = Path.Combine(this.FileStoragePhysicalPath(), file.Id.ToString());
                string tmpPDF = Path.Combine(baseDir, tmpName + ".pdf");
                string tmpSwf = Path.Combine(baseDir, tmpName + ".swf");
                string conversionFlag = Path.Combine(baseDir, tmpName + ".flag");
                if (!System.IO.File.Exists(tmpPDF))
                {
                    lock (GetDictionaryLocker(tmpPDF))
                    {
                        if (!System.IO.File.Exists(tmpPDF))
                        {
                            List<ATDrawing> drawings = this.drawingModel.GetAllForFile(file.Id).ToList();
                            drawings.Where(o => o.Mark.DateChanged > file.DateModified)
                                .ToList()
                                .ForEach(o => this.drawingModel.RegisterDelete(o));
                            drawings = drawings.Where(o => o.Mark.DateChanged <= file.DateModified).ToList();

                            List<ATHighlightStrikeOut> highlights =
                                this.highlightStrikeOutModel.GetAllForFile(file.Id).ToList();
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

                            byte[] pdfBuffer = this.pdfModel.DrawOnPDF(
                                drawings,
                                highlights,
                                shapes,
                                textItems,
                                rotations,
                                buffer);

                            using (var fs = new FileStream(tmpPDF, FileMode.Create, FileAccess.Write))
                            {
                                fs.Write(pdfBuffer, 0, pdfBuffer.Length);
                            }
                        }
                    }
                }

                if (!System.IO.File.Exists(conversionFlag))
                {
                    lock (GetDictionaryLocker(tmpSwf))
                    {
                        if (!System.IO.File.Exists(conversionFlag))
                        {
                            using (var st = System.IO.File.Create(conversionFlag))
                            {
                                this.RemoveFileSafely(tmpSwf);
                            }
                            var converter = IoC.Resolve<Pdf2SwfConverter>();
                            converter.Convert(tmpPDF, tmpSwf);
                            this.RemoveFileSafely(conversionFlag);
                        }
                    }
                }

                if (System.IO.File.Exists(tmpSwf))
                {
                    var fi = new FileInfo(tmpSwf);
                    if (fi.Length > 0)
                    {
                        return new Tuple<int, byte[]>(200, ReadAllBytes(tmpSwf));
                    }

                    return new Tuple<int, byte[]>(204, null);
                }

                if (System.IO.File.Exists(conversionFlag))
                {
                    return new Tuple<int, byte[]>(204, null);
                }
            }

            if (file != null)
            {
                try
                {
                    var data = this.GetSWFData(file);
                    return new Tuple<int, byte[]>(data != null ? 200 : 204, data);
                }
                catch (Exception)
                {
                    return null;
                }
            }

            return null;
        }

        /// <summary>
        /// Get file data as image.
        /// </summary>
        /// <param name="id">
        /// The file id.
        /// </param>
        /// <param name="pageIndex">
        /// The page Index.
        /// </param>
        /// <returns>
        /// The <see cref="Stream"/>.
        /// </returns>
        public byte[] GetUpdatedPagedSWFData(string id, int pageIndex)
        {
            Guid fileId = Guid.Parse(id);
            File file = this.GetOneById(fileId).Value;
            byte[] buffer = this.GetPDFData(file);
            return this.GetUpdatedPagedSWFData(file, pageIndex, buffer);
        }

        /// <summary>
        /// Get file data as image.
        /// </summary>
        /// <param name="id">
        /// The file id.
        /// </param>
        /// <param name="pageIndex">
        /// The page Index.
        /// </param>
        /// <returns>
        /// The <see cref="Stream"/>.
        /// </returns>
        public byte[] GetUpdatedPagedSWFData(File file, int pageIndex, byte[] buffer)
        {
            if (buffer != null && file.DateModified.HasValue && file.DateModified > file.DateCreated)
            {
                string tmpName = file.DateModified.Value.ToMicroSeconds().ToString(CultureInfo.InvariantCulture); // based on date
                var baseDir = Path.Combine(this.FileStoragePhysicalPath(), file.Id.ToString());
                string tmpPDF = Path.Combine(baseDir, tmpName + ".pdf");
                string tmpSwfPaged = Path.Combine(baseDir, string.Format("{0}.{1}.swf", tmpName, pageIndex));
                string tmpSwfPatternPaged = Path.Combine(baseDir, string.Format("{0}.%.swf", tmpName));

                this.CheckAndCreateUpdatedPDFWithLock(tmpPDF, file.Id, file, buffer);

                if (!System.IO.File.Exists(tmpSwfPaged))
                {
                    lock (GetDictionaryLocker(tmpSwfPatternPaged))
                    {
                        if (!System.IO.File.Exists(tmpSwfPaged))
                        {
                            var converter = IoC.Resolve<Pdf2SwfConverter>();
                            converter.Convert(tmpPDF, tmpSwfPatternPaged);
                        }
                    }
                }

                if (System.IO.File.Exists(tmpSwfPaged))
                {
                    return System.IO.File.ReadAllBytes(tmpSwfPaged);
                }
            }

            if (file != null)
            {
                try
                {
                    return this.GetSWFData(file, pageIndex);
                }
                catch (Exception)
                {
                    return null;
                }
            }

            return null;
        }

        private void CheckAndCreateUpdatedPDFWithLock(string tmpPDF, Guid fileId, File file, byte[] buffer)
        {
            if (!System.IO.File.Exists(tmpPDF))
            {
                lock (GetDictionaryLocker(tmpPDF))
                {
                    if (!System.IO.File.Exists(tmpPDF))
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

                        using (var fs = new FileStream(tmpPDF, FileMode.Create, FileAccess.Write))
                        {
                            fs.Write(pdfBuffer, 0, pdfBuffer.Length);
                        }
                    }
                }
            }
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
        public void MoveMarksToNewFile(Guid originId, Guid newId)
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
            try
            {
                var fileInfo = new FileInfo(filePath);
                // ReSharper disable once AssignNullToNotNullAttribute
                // ReSharper disable once PossibleNullReferenceException
                if (!string.IsNullOrWhiteSpace(fileInfo.DirectoryName) 
                    && Directory.Exists(fileInfo.DirectoryName) 
                    && !fileInfo.DirectoryName.Equals(this.FileStoragePhysicalPath(), StringComparison.InvariantCultureIgnoreCase))
                {
                    this.ClearDirectoryAndRemoveItSafely(fileInfo.DirectoryName);
                }
            }
            catch (Exception)
            {
                this.RemoveFileSafely(filePath);
                var pdfName = this.PermanentPdfName(file);
                this.RemoveFileSafely(pdfName);
                var swfName = this.PermanentSWFName(file);
                this.RemoveFileSafely(swfName);
                this.RemoveFileAllFileTypeSafely(swfName);
                this.RemoveFileAllFileTypeSafely(pdfName);
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
        /// <param name="updateDateModified">
        /// The update Date Modified.
        /// </param>
        public override void RegisterSave(File entity, bool flush, bool updateDateModified = true)
        {
            if (updateDateModified)
            {
                entity.DateModified = DateTime.Now.AddMinutes(1);
            }

            base.RegisterSave(entity, flush, false);
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
        /// The <see cref="byte"/>.
        /// </returns>
        public byte[] ResizeImage(byte[] content, int maxImageWidth, int maxImageHeight)
        {
            Image img;
            if (TryParseImage(content, out img))
            {
                try
                {
                    Image resizedImage = this.ResizeImage(string.Empty, maxImageWidth, maxImageHeight, img);
                    ImageCodecInfo iciPng = GetPngImageCodecInfo();
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
            if (!entity.IsOriginal.HasValue || !entity.IsOriginal.Value)
            {
                Topic topic = entity.Topic;
                var availiblePatternOptions = new Dictionary<string, string>();
                var file = new FileInfo(entity.FileName);
                availiblePatternOptions.Add("{fileName}", Path.GetFileNameWithoutExtension(file.Name));
                availiblePatternOptions.Add("{extension}", file.Extension);
                availiblePatternOptions.Add("{number}", string.Format("{0:#0000}", exhibitNumber));
                availiblePatternOptions.Add("{topic}", topic.FullName.Trim().Replace(" ", "_"));
                string displayName = availiblePatternOptions.Keys.Aggregate(
                    (string)this.settings.DisplayNamePattern,
                    (current, optionKey) => current.Replace(optionKey, availiblePatternOptions[optionKey]));
                entity.DisplayName = displayName;
            }
            else
            {
                entity.DisplayName = entity.FileName;
            }
            entity.FileNumber = exhibitNumber;
        }

        #endregion

        #region Methods


        /// <summary>
        /// The get dictionary locker.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        private static object GetDictionaryLocker(string key)
        {
            if (!ObjectLockers.ContainsKey(key))
            {
                lock (DictionaryLocker)
                {
                    if (!ObjectLockers.ContainsKey(key))
                    {
                        ObjectLockers.Add(key, new object());
                    }
                }
            }

            return ObjectLockers[key];
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

            var path = Path.Combine(folder, this.settings.PermFilePattern.Replace("{fileId}", file.Id.ToString()));
            var di = new DirectoryInfo(Path.GetDirectoryName(path) ?? this.FileStoragePhysicalPath());
            if (!di.Exists)
            {
                di.Create();
            }

            return path;
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

            var path = Path.Combine(folder, ((string)this.settings.PermPDFPattern).Replace("{fileId}", file.Id.ToString()));
            var di = new DirectoryInfo(Path.GetDirectoryName(path) ?? this.FileStoragePhysicalPath());
            if (!di.Exists)
            {
                di.Create();
            }

            return path;
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
            return Path.Combine(this.WebOrbStoragePhysicalPath(), file.Id.ToString());
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
        /// The get png image codec info.
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
        /// The clear directory and remove it safely.
        /// </summary>
        /// <param name="webOrbFolderName">
        /// The web orb folder name.
        /// </param>
        private void ClearDirectoryAndRemoveItSafely(string webOrbFolderName)
        {
            try
            {
                new DirectoryInfo(webOrbFolderName).GetFiles().ToList().ForEach(f => f.Delete());
                Directory.Delete(webOrbFolderName);
            }

                // ReSharper disable once EmptyGeneralCatchClause
            catch
            {
            }
        }

        /// <summary>
        /// The fix content for black squares.
        /// </summary>
        /// <param name="content">
        /// The content.
        /// </param>
        /// <returns>
        /// The <see cref="byte"/>.
        /// </returns>
        private byte[] FixContentForBlackSquares(byte[] content)
        {
            try
            {
                content = this.pdfProcessorHelper.CheckIfDocumentWasScannedAndFixIfNecessary(content);
            }
                
                // ReSharper disable once EmptyGeneralCatchClause
            catch (Exception)
            {
            }

            return content;
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
            dynamic filePath = filePattern.Replace("{fileId}", file.Id.ToString())
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
        /// The remove file all file type safely.
        /// </summary>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        // ReSharper disable once UnusedMember.Local
        private void RemoveFileAllFileTypeSafely(string fileName)
        {
            try
            {
                var fi = new FileInfo(fileName);

                // ReSharper disable once PossibleNullReferenceException
                if (fi.Directory != null && fi.Directory.Exists
                    && !fi.DirectoryName.Equals(
                        this.FileStoragePhysicalPath(), 
                        StringComparison.InvariantCultureIgnoreCase))
                {
                    FileInfo[] files = fi.Directory.GetFiles("*" + fi.Extension);
                    foreach (FileInfo file in files)
                    {
                        this.RemoveFileSafely(file.FullName);
                    }
                }
            }

                // ReSharper disable once EmptyGeneralCatchClause
            catch
            {
            }
        }

        /// <summary>
        /// The remove file safely.
        /// </summary>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        private void RemoveFileSafely(string fileName)
        {
            try
            {
                var fi = new FileInfo(fileName);
                if (fi.Exists)
                {
                    fi.Delete();
                }
            }

                // ReSharper disable once EmptyGeneralCatchClause
            catch
            {
                try
                {
                    //second try with removing readonly 
                    System.IO.File.SetAttributes(fileName, FileAttributes.Normal);
                    System.IO.File.Exists(fileName);
                    System.IO.File.Delete(fileName);
                }
                catch (Exception)
                {
                    
                }
            }
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
        /// <returns>
        /// The <see cref="Image"/>.
        /// </returns>
        private Image ResizeImage(
            string path, 
            /* note changed names */
            int maxCanvasWidth, 
            int maxCanvasHeight, 
            /* new */
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

            int canvasWidth = Math.Min(maxCanvasWidth, originalWidth.Value);
            int canvasHeight = Math.Min(maxCanvasHeight, originalHeight.Value);

            /* ------------------ new code --------------- */

            // Figure out the ratio
            double ratioX = canvasWidth / (double)originalWidth;
            double ratioY = canvasHeight / (double)originalHeight;

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

            Graphics graphic = Graphics.FromImage(thumbnail);
            graphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphic.SmoothingMode = SmoothingMode.HighQuality;
            graphic.PixelOffsetMode = PixelOffsetMode.HighQuality;
            graphic.CompositingQuality = CompositingQuality.HighQuality;
            graphic.Clear(Color.Transparent); // white padding
            graphic.DrawImage(image, 0, 0, newWidth, newHeight);

            /* ------------- end new code ---------------- */
            return thumbnail;
        }

        #endregion
    }
}