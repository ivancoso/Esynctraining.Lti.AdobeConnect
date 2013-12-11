namespace PDFAnnotation.Core.Domain.Entities
{
    using System;
    using Esynctraining.Core.Domain.Entities;

    using Iesi.Collections.Generic;

    using PDFAnnotation.Core.FullText;

    /// <summary>
    ///     The file.
    /// </summary>
    [Serializable]
    [FullTextEnabled]
    public class File : Entity
    {
        #region Fields

        /// <summary>
        ///     The state.
        /// </summary>
        private FileStatus? status = FileStatus.Created;

        /// <summary>
        ///     The ACSessions.
        /// </summary>
        private ISet<ATMark> marks = new HashedSet<ATMark>();

        private DateTime dateCreated;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="File"/> class.
        /// </summary>
        public File()
        {
            this.WebOrbId = Guid.NewGuid();
            this.DateCreated = DateTime.Now;
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
            get { return dateCreated; }
            set
            {
                dateCreated = value;
                this.DateModified = value;
            }
        }

        /// <summary>
        ///     Gets or sets the date.
        /// </summary>
        public virtual DateTime DateModified { get; set; }

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
        /// Gets or sets the Topic.
        /// </summary>
        public virtual string Description { get; set; }

        /// <summary>
        ///     Gets or sets the state.
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
        ///     Gets or sets the web orb id.
        /// </summary>
        public virtual Guid? WebOrbId { get; set; }

        /// <summary>
        /// Gets or sets the event.
        /// </summary>
        public virtual Topic Topic { get; set; }

        /// <summary>
        /// Gets or sets the event.
        /// </summary>
        public virtual Category Category { get; set; }

        /// <summary>
        ///     Gets or sets the event's unique name.
        /// </summary>
        [FullTextIndexed(2)]
        public virtual string DisplayName { get; set; }

        /// <summary>
        ///     Gets or sets the event's unique number.
        /// </summary>
        [FullTextIndexed(3)]
        public virtual int? FileNumber { get; set; }

        #endregion
    }
}