namespace EdugameCloud.Core.Domain.DTO
{
    using System;
    using System.Runtime.Serialization;

    using EdugameCloud.Core.Domain.Entities;

    using Esynctraining.Core.Extensions;

    /// <summary>
    /// The email history DTO.
    /// </summary>
    [DataContract]
    public class EmailHistoryDTO
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="UserDTO"/> class.
        /// </summary>
        public EmailHistoryDTO()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EmailHistoryDTO"/> class.
        /// </summary>
        /// <param name="emailHistory">
        /// The email history.
        /// </param>
        public EmailHistoryDTO(EmailHistory emailHistory)
        {
            if (emailHistory != null)
            {
                this.emailHistoryId = emailHistory.Id;
                this.sentTo = emailHistory.SentTo;
                this.sentFrom = emailHistory.SentFrom;
                this.sentToName = emailHistory.SentToName;
                this.sentFromName = emailHistory.SentFromName;
                this.sentCc = emailHistory.SentCc;
                this.sentBcc = emailHistory.SentBcc;
                this.subject = emailHistory.Subject;
                this.message = emailHistory.Message;
                this.body = emailHistory.Body;
                this.date = emailHistory.Date;
                this.companyName = emailHistory.Return(x => x.User.Return(y => y.Company.Return(z => z.CompanyName, null), null), null);
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the sent to.
        /// </summary>
        [DataMember]
        public virtual int emailHistoryId { get; set; }

        /// <summary>
        /// Gets or sets the sent to.
        /// </summary>
        [DataMember]
        public virtual string sentTo { get; set; }
        /// <summary>
        /// Gets or sets the sent from.
        /// </summary>
        [DataMember]
        public virtual string sentFrom { get; set; }

        /// <summary>
        /// Gets or sets the sent to.
        /// </summary>
        [DataMember]
        public virtual string sentToName { get; set; }
        /// <summary>
        /// Gets or sets the sent from.
        /// </summary>
        [DataMember]
        public virtual string sentFromName { get; set; }

        /// <summary>
        /// Gets or sets the sent cc.
        /// </summary>
        [DataMember]
        public virtual string sentCc { get; set; }

        /// <summary>
        /// Gets or sets the sent bcc.
        /// </summary>
        [DataMember]
        public virtual string sentBcc { get; set; }

        /// <summary>
        /// Gets or sets the subject.
        /// </summary>
        [DataMember]
        public virtual string subject { get; set; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        [DataMember]
        public virtual string message { get; set; }

        /// <summary>
        /// Gets or sets the body.
        /// </summary>
        [DataMember]
        public virtual string body { get; set; }

        /// <summary>
        /// Gets or sets the date.
        /// </summary>
        [DataMember]
        public virtual DateTime date { get; set; }
        /// <summary>
        /// Gets or sets the company name.
        /// </summary>
        [DataMember]
        public virtual string companyName { get; set; }

        #endregion
    }
}
