namespace EdugameCloud.Core.Domain.Entities
{
    using System;

    using Esynctraining.Core.Domain.Entities;

    /// <summary>
    ///     The applet result.
    /// </summary>
    public class AppletResult : Entity
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the AC session.
        /// </summary>
        public virtual int ACSessionId { get; set; }

        /// <summary>
        /// Gets or sets the document xml.
        /// </summary>
        public virtual AppletItem AppletItem { get; set; }

        /// <summary>
        /// Gets or sets the date created.
        /// </summary>
        public virtual DateTime DateCreated { get; set; }

        /// <summary>
        /// Gets or sets the end time.
        /// </summary>
        public virtual DateTime EndTime { get; set; }

        /// <summary>
        /// Gets or sets the participant name
        /// </summary>
        public virtual string ParticipantName { get; set; }

        /// <summary>
        /// Gets or sets the score.
        /// </summary>
        public virtual int Score { get; set; }

        /// <summary>
        /// Gets or sets the is archive.
        /// </summary>
        public virtual bool? IsArchive { get; set; }

        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        public virtual string Email { get; set; }

        /// <summary>
        /// Gets or sets the start time.
        /// </summary>
        public virtual DateTime StartTime { get; set; }

        #endregion
    }
}