namespace PDFAnnotation.Core.Domain.Entities
{
    using System;
    using System.Collections.Generic;
    using Esynctraining.Core.Domain.Entities;
    using Esynctraining.Core.Extensions;
    using Esynctraining.Core.FullText;

    /// <summary>
    ///     The file.
    /// </summary>
    [Serializable]
    [FullTextEnabled]
    public class File : EntityGuid
    {
        #region Fields

        /// <summary>
        ///     The status.
        /// </summary>
        private FileStatus? status = FileStatus.Created;

        /// <summary>
        ///     The marks.
        /// </summary>
        private ISet<ATMark> marks = new HashSet<ATMark>();

        /// <summary>
        ///     The children files.
        /// </summary>
        private ISet<File> childrenFiles = new HashSet<File>();

        /// <summary>
        /// The date created.
        /// </summary>
        private DateTime dateCreated;

        private DateTime? dateModified;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="File"/> class.
        /// </summary>
        public File()
        {
            this.dateCreated = DateTime.Now.AddSeconds(-30);
            this.status = FileStatus.Created;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the sessions.
        /// </summary>
        public virtual ISet<ATMark> Marks
        {
            get
            {
                return this.marks;
            }

            set
            {
                this.marks = value;
            }
        }

        /// <summary>
        ///     Gets or sets the date.
        /// </summary>
        public virtual DateTime DateCreated
        {
            get
            {
                return this.dateCreated;
            }

            set
            {
                this.dateCreated = value.AdaptToSQL();
            }
        }

        /// <summary>
        ///     Gets or sets the date.
        /// </summary>
        public virtual DateTime? DateModified
        {
            get
            {
                return this.dateModified;
            }
            set
            {
                this.dateModified = value.AdaptToSQL();
            }
        }

        /// <summary>
        ///     Gets or sets the name.
        /// </summary>
        public virtual string AcMeetingUrl { get; set; }

        /// <summary>
        ///     Gets or sets the number of pages.
        /// </summary>
        public virtual int? NumberOfPages { get; set; }

        /// <summary>
        ///     Gets or sets the name.
        /// </summary>
        [FullTextIndexed(0)]
        public virtual string FileName { get; set; }

        /// <summary>
        ///     Gets or sets the file size.
        /// </summary>
        public virtual string FileSize { get; set; }

        /// <summary>
        /// Gets or sets the Topic.
        /// </summary>
        [FullTextIndexed(1)]
        public virtual string TopicName { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        public virtual string Description { get; set; }

        /// <summary>
        ///     Gets or sets the file status.
        /// </summary>
        public virtual FileStatus? Status
        {
            get
            {
                return this.status;
            }

            set
            {
                this.status = value;
            }
        }

        /// <summary>
        ///     Gets or sets the upload file status.
        /// </summary>
        public virtual UploadFileStatus UploadFileStatus { get; set; }

        /// <summary>
        /// Gets or sets the topic.
        /// </summary>
        public virtual Topic Topic { get; set; }

        /// <summary>
        /// Gets or sets the category.
        /// </summary>
        public virtual Category Category { get; set; }

        /// <summary>
        /// Gets or sets the parent file
        /// </summary>
        public virtual File ParentFile { get; set; }

        /// <summary>
        /// Gets or sets the parent file
        /// </summary>
        public virtual ISet<File> ChildrenFiles
        {
            get
            {
                return this.childrenFiles;
            }

            set
            {
                this.childrenFiles = value;
            }
        }

        /// <summary>
        /// Gets or sets the breakout room id
        /// </summary>
        public virtual string BreakoutRoomId { get; set;}

        /// <summary>
        ///     Gets or sets the user.
        /// </summary>
        public virtual int? UserId { get; set; }

        /// <summary>
        ///     Gets or sets the display name.
        /// </summary>
        [FullTextIndexed(2)]
        public virtual string DisplayName { get; set; }

        /// <summary>
        ///     Gets or sets the file number.
        /// </summary>
        [FullTextIndexed(3)]
        public virtual int? FileNumber { get; set; }

        /// <summary>
        /// Gets or sets the is shared.
        /// </summary>
        public virtual bool? IsShared { get; set; }

        /// <summary>
        /// Gets or sets the is original.
        /// </summary>
        public virtual bool? IsOriginal { get; set; }

        #endregion
    }
}