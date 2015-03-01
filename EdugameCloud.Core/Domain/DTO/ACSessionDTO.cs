namespace EdugameCloud.Core.Domain.DTO
{
    using System.Runtime.Serialization;
    using EdugameCloud.Core.Domain.Entities;
    using EdugameCloud.Core.Extensions;

    using Esynctraining.Core.Extensions;

    /// <summary>
    ///     The applet item DTO.
    /// </summary>
    [DataContract]
    public class ACSessionDTO
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ACSessionDTO"/> class.
        /// </summary>
        public ACSessionDTO()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ACSessionDTO"/> class.
        /// </summary>
        /// <param name="result">
        /// The result.
        /// </param>
        public ACSessionDTO(ACSession result)
        {
            this.acSessionId = result.Id;
            this.subModuleItemId = result.SubModuleItem.With(x => x.Id);
            this.acUserModeId = result.ACUserMode.With(x => x.Id);
            this.userId = result.User.With(x => x.Id);
            this.languageId = result.Language.With(x => x.Id);
            this.accountId = result.AccountId;
            this.meetingURL = result.MeetingUrl;
            this.scoId = result.ScoId;
            this.dateCreated = result.DateCreated.With(x => x.ConvertToUnixTimestamp());
            this.status = (int)result.Status;
            this.includeACEmails = result.IncludeAcEmails ?? false;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets a value indicating whether include ac emails.
        /// </summary>
        [DataMember]
        public bool includeACEmails { get; set; }

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        [DataMember]
        public int status { get; set; }

        /// <summary>
        /// Gets or sets the AC user mode id.
        /// </summary>
        [DataMember]
        public int acUserModeId { get; set; }

        /// <summary>
        /// Gets or sets the account id.
        /// </summary>
        [DataMember]
        public int accountId { get; set; }

        /// <summary>
        /// Gets or sets the date created.
        /// </summary>
        [DataMember]
        public double dateCreated { get; set; }

        /// <summary>
        /// Gets or sets the language.
        /// </summary>
        [DataMember]
        public int languageId { get; set; }

        /// <summary>
        /// Gets or sets the meeting url.
        /// </summary>
        [DataMember]
        public string meetingURL { get; set; }

        /// <summary>
        /// Gets or sets the SCO id.
        /// </summary>
        [DataMember]
        public int scoId { get; set; }

        /// <summary>
        /// Gets or sets the user.
        /// </summary>
        [DataMember]
        public int userId { get; set; }

        /// <summary>
        ///     Gets or sets the sub module item.
        /// </summary>
        [DataMember]
        public int subModuleItemId { get; set; }

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        [DataMember]
        public int acSessionId { get; set; }

        #endregion
    }
}