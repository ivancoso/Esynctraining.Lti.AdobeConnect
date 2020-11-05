namespace PDFAnnotation.Core.Domain.DTO
{
    using System;
    using System.IO;
    using System.Runtime.Serialization;

    using Esynctraining.Core.Extensions;

    using File = PDFAnnotation.Core.Domain.Entities.File;

    /// <summary>
    ///     The image DTO.
    /// </summary>
    [DataContract]
    public class FileDTO
    {
        /// <summary>
        /// The meeting url.
        /// </summary>
        private string meetingUrl = null;

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FileDTO"/> class.
        /// </summary>
        public FileDTO()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileDTO"/> class.
        /// </summary>
        /// <param name="file">
        /// The image.
        /// </param>
        public FileDTO(File file)
        {
            this.fileId = file.Id.ToString();
            this.parentFileId = file.ParentFile.With(x => x.Id.ToString());
            this.breakoutRoomId = file.BreakoutRoomId;
            this.dateCreated = file.DateCreated.With(x => x.ConvertToUnixTimestamp()); ;
            this.fileName = file.FileName;
            this.fileSize = file.FileSize;
            this.topicName = file.TopicName;
            this.description = file.Description;
            this.categoryId = file.Category.Return(x => x.Id, (int?)null);
            this.userId = file.UserId;
            this.displayName = file.DisplayName;
            this.dateModified = file.DateModified.Return(x => x.Value.ConvertToUnixTimestamp(), (double?)null);
            this.fileNumber = file.FileNumber;
            this.categoryName = file.Category.With(c => c.CategoryName);
            this.userName = file.UserId.ToString();
            this.topicVo = new TopicDTO(file.Topic);
            this.topicId = this.topicVo.Return(x => x.topicId, (int?)null);
            this.isShared = file.IsShared.HasValue && file.IsShared.Value;
            this.isOriginal = file.IsOriginal.HasValue && file.IsOriginal.Value;
            this.acMeetingUrl = file.AcMeetingUrl;

            if (file.Status != null)
            {
                this.fileStatus = (int)file.Status;
            }

            this.numberOfPages = file.NumberOfPages ?? 0;
            this.uploadStatus = (int)file.UploadFileStatus;

        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the fileStatus.
        /// </summary>
        [DataMember]
        public int uploadStatus { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is shared.
        /// </summary>
        [DataMember]
        public bool isOriginal { get; set; }

        /// <summary>
        ///     Gets or sets the pages count.
        /// </summary>
        [DataMember]
        public int numberOfPages { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is shared.
        /// </summary>
        [DataMember]
        public bool isShared { get; set; }

        /// <summary>
        /// Gets or sets a meeting url.
        /// </summary>
        [DataMember]
        public string acMeetingUrl
        {
            get
            {
                return string.IsNullOrWhiteSpace(this.meetingUrl.With(x => x.Trim())) ? null : this.meetingUrl.Trim();
            }
            set
            {
                this.meetingUrl = string.IsNullOrWhiteSpace(value) ? null : value.Trim();
            }
        }

        /// <summary>
        ///     Gets or sets the fileStatus.
        /// </summary>
        [DataMember]
        public int fileStatus { get; set; }

        /// <summary>
        /// Gets or sets the case id.
        /// </summary>
        [DataMember]
        public int? userId { get; set; }

        /// <summary>
        /// Gets or sets the user name.
        /// </summary>
        [DataMember]
        public string userName { get; set; }

        /// <summary>
        ///     Gets or sets the Topic.
        /// </summary>
        [DataMember]
        public string topicName { get; set; }

        /// <summary>
        ///     Gets or sets the description.
        /// </summary>
        [DataMember]
        public string description { get; set; }

        /// <summary>
        ///     Gets or sets the date.
        /// </summary>
        [DataMember]
        public double dateCreated { get; set; }

        /// <summary>
        ///     Gets or sets the id.
        /// </summary>
        [DataMember]
        public string fileId { get; set; }

        /// <summary>
        ///     Gets or sets the name.
        /// </summary>
        [DataMember]
        public string fileName { get; set; }

        /// <summary>
        ///     Gets or sets the size.
        /// </summary>
        [DataMember]
        public string fileSize { get; set; }

        /// <summary>
        /// Gets or sets the created by.
        /// </summary>
        [DataMember]
        public string createdBy { get; set; }

        /// <summary>
        /// Gets or sets the case id.
        /// </summary>
        [DataMember]
        public int? categoryId { get; set; }

        /// <summary>
        /// Gets or sets the case id.
        /// </summary>
        [DataMember]
        public string breakoutRoomId { get; set; }

        /// <summary>
        /// Gets or sets the case id.
        /// </summary>
        [DataMember]
        public string parentFileId { get; set; }

        /// <summary>
        /// Gets or sets the Topic id.
        /// </summary>
        [DataMember]
        public int? topicId { get; set; }

        /// <summary>
        ///     Gets or sets the name.
        /// </summary>
        [DataMember]
        public string displayName { get; set; }

        /// <summary>
        /// Gets or sets the date modified.
        /// </summary>
        [DataMember]
        public double? dateModified { get; set; }

        /// <summary>
        ///     Gets or sets the associated case name.
        /// </summary>
        [DataMember]
        public string categoryName { get; set; }

        /// <summary>
        ///     Gets or sets the unique number.
        /// </summary>
        [DataMember]
        public int? fileNumber { get; set; }

        /// <summary>
        /// Gets or sets Topic's DTO
        /// </summary>
        [DataMember]
        public TopicDTO topicVo { get; set; }

        #endregion

        /// <summary>
        /// The has case and Topic.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool HasCategoryAndTopic()
        {
            return this.categoryId.HasValue && this.categoryId.Value != default(int) && this.HasTopic();
        }

        /// <summary>
        /// The has case and Topic.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool HasTopic()
        {
            return this.topicId.HasValue && this.topicId.Value != default(int);
        }

        /// <summary>
        /// The is PDF.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool IsPDF()
        {
            return !string.IsNullOrWhiteSpace(this.fileName)
                   && Path.GetExtension(this.fileName).ToLowerInvariant().EndsWith("pdf");
        }

        /// <summary>
        /// The has case and witness.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool HasUser()
        {
            return this.IsPDF() && !this.HasTopic() && !this.HasCategoryAndTopic() && this.userId.HasValue && this.userId.Value != default(int);
        }
    }
}