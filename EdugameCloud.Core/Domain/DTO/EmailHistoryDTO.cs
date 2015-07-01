namespace EdugameCloud.Core.Domain.DTO
{
    using System;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Xml;

    using EdugameCloud.Core.Domain.Entities;
    using EdugameCloud.Core.Extensions;

    using Esynctraining.Core.Extensions;

    /// <summary>
    /// The email history DTO.
    /// </summary>
    [DataContract]
    public class EmailHistoryDTO
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="EmailHistoryDTO"/> class.
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
                this.body = string.Empty; //// TODO Either escape HTML inside XML or disable AMF String Referencing via config emailHistory.Body;
                this.date = emailHistory.Date.ConvertToUnixTimestamp();
                this.companyName = emailHistory.Return(x => x.User.Return(y => y.Company.Return(z => z.CompanyName, null), null), null);
                this.status = emailHistory.Status;
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the sent to.
        /// </summary>
        [DataMember]
        public int emailHistoryId { get; set; }

        /// <summary>
        /// Gets or sets the sent to.
        /// </summary>
        [DataMember]
        public string sentTo { get; set; }

        /// <summary>
        /// Gets or sets the sent from.
        /// </summary>
        [DataMember]
        public string sentFrom { get; set; }

        /// <summary>
        /// Gets or sets the sent to.
        /// </summary>
        [DataMember]
        public string sentToName { get; set; }

        /// <summary>
        /// Gets or sets the sent from.
        /// </summary>
        [DataMember]
        public string sentFromName { get; set; }

        /// <summary>
        /// Gets or sets the sent cc.
        /// </summary>
        [DataMember]
        public string sentCc { get; set; }

        /// <summary>
        /// Gets or sets the sent bcc.
        /// </summary>
        [DataMember]
        public string sentBcc { get; set; }

        /// <summary>
        /// Gets or sets the subject.
        /// </summary>
        [DataMember]
        public string subject { get; set; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        [DataMember]
        public string message { get; set; }

        /// <summary>
        /// Gets or sets the body.
        /// </summary>
        [DataMember]
        public string body { get; set; }

        /// <summary>
        /// Gets or sets the date.
        /// </summary>
        [DataMember]
        public double date { get; set; }

        /// <summary>
        /// Gets or sets the company name.
        /// </summary>
        [DataMember]
        public string companyName { get; set; }

        [DataMember]
        public int status { get; set; }

        #endregion

    }

}
