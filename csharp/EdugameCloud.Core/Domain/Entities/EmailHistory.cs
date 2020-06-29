namespace EdugameCloud.Core.Domain.Entities
{
    using System;
    using System.Runtime.Serialization;

    using Esynctraining.Core.Domain.Entities;

    /// <summary>
    /// Email history
    /// </summary>
    [DataContract]
    [Serializable]
    public class EmailHistory : Entity
    {
        public const int StatusSent = 1;
        public const int StatusFailed = 2;

        #region Public Properties

        /// <summary>
        /// Gets or sets the sent to.
        /// </summary>
        public virtual string SentTo { get; set; }
        /// <summary>
        /// Gets or sets the sent from.
        /// </summary>
        public virtual string SentFrom { get; set; }
        /// <summary>
        /// Gets or sets the sent to name.
        /// </summary>
        public virtual string SentToName { get; set; }
        /// <summary>
        /// Gets or sets the sent from name.
        /// </summary>
        public virtual string SentFromName { get; set; }

        /// <summary>
        /// Gets or sets the sent cc.
        /// </summary>
        public virtual string SentCc { get; set; }

        /// <summary>
        /// Gets or sets the sent bcc.
        /// </summary>
        public virtual string SentBcc { get; set; }

        /// <summary>
        /// Gets or sets the subject.
        /// </summary>
        public virtual string Subject { get; set; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        public virtual string Message { get; set; }        

        /// <summary>
        /// Gets or sets the body.
        /// </summary>
        public virtual string Body { get; set; }

        /// <summary>
        /// Gets or sets the body.
        /// </summary>
        public virtual DateTime Date { get; set; }

        /// <summary>
        /// Gets or sets the body.
        /// </summary>
        public virtual User User { get; set; }

        public virtual int Status { get; set; }

        #endregion
    }
}
