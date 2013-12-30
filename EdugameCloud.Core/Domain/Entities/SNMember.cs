namespace EdugameCloud.Core.Domain.Entities
{
    using System;

    using Esynctraining.Core.Domain.Entities;

    /// <summary>
    ///     The SN member.
    /// </summary>
    public class SNMember : Entity
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the session.
        /// </summary>
        public virtual int ACSessionId { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public virtual string Participant { get; set; }

        /// <summary>
        /// Gets or sets the participant profile.
        /// </summary>
        public virtual string ParticipantProfile { get; set; }

        /// <summary>
        /// Gets or sets the date created.
        /// </summary>
        public virtual DateTime? DateCreated { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is blocked.
        /// </summary>
        public virtual bool IsBlocked { get; set; }

        #endregion
    }
}