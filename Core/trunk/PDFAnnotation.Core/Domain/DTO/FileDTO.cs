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
            this.fileId = file.Id;
            this.dateCreated = file.DateCreated;
            this.fileName = file.FileName;
            this.fileSize = file.FileSize;
            this.topicName = file.TopicName;
            this.description = file.Description;
            this.webOrbId = file.WebOrbId;
            this.categoryId = file.Category.Return(x => x.Id, (int?)null);
            this.displayName = file.DisplayName;
            this.dateModified = file.DateModified;
            this.fileNumber = file.FileNumber;
            this.categoryName = file.Category.With(c => c.CategoryName);
            this.topicVo = new TopicDTO(file.Topic);
            this.topicId = this.topicVo.Return(x => x.topicId, (int?) null);
        }

        #endregion

        #region Public Properties

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
        public DateTime dateCreated { get; set; }

        /// <summary>
        ///     Gets or sets the id.
        /// </summary>
        [DataMember]
        public int fileId { get; set; }

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
        ///     Gets or sets the name.
        /// </summary>
        [DataMember]
        public Guid? webOrbId { get; set; }

        /// <summary>
        /// Gets or sets the case id.
        /// </summary>
        [DataMember]
        public int? categoryId { get; set; }

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
        public DateTime dateModified { get; set; }

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
    }
}