namespace EdugameCloud.Core.Domain.Entities
{
    using System;
    using Esynctraining.Core.Domain.Entities;

    /// <summary>
    ///     The SN Group discussion.
    /// </summary>
    public class SNGroupDiscussion : Entity
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the date created.
        /// </summary>
        public virtual DateTime DateCreated { get; set; }

        /// <summary>
        /// Gets or sets the date modified.
        /// </summary>
        public virtual DateTime? DateModified { get; set; }

        /// <summary>
        ///     Gets or sets the title.
        /// </summary>
        public virtual string GroupDiscussionTitle { get; set; }

        /// <summary>
        ///     Gets or sets the data.
        /// </summary>
        public virtual string GroupDiscussionData { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is active.
        /// </summary>
        public virtual bool IsActive { get; set; }

        /// <summary>
        ///     Gets or sets the session.
        /// </summary>
        public virtual int ACSessionId { get; set; }

        #endregion
    }
}