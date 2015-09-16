namespace EdugameCloud.Core.Domain.DTO
{
    using System.Runtime.Serialization;

    using EdugameCloud.Core.Extensions;

    /// <summary>
    ///     The crossword session from stored procedure DTO.
    /// </summary>
    [DataContract]
    public class CrosswordSessionDTO
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CrosswordSessionDTO"/> class.
        /// </summary>
        public CrosswordSessionDTO()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CrosswordSessionDTO"/> class.
        /// </summary>
        /// <param name="dto">
        /// The DTO.
        /// </param>
        public CrosswordSessionDTO(CrosswordSessionFromStoredProcedureDTO dto)
        {
            this.acSessionId = dto.acSessionId;
            this.acUserModeId = dto.acUserModeId;
            this.activeParticipants = dto.activeParticipants;
            this.appletItemId = dto.appletItemId;
            this.appletName = dto.appletName;
            this.categoryName = dto.categoryName;
            this.dateCreated = dto.dateCreated.ConvertToUnixTimestamp();
            this.language = dto.language;
            this.subModuleItemId = dto.subModuleItemId;
            this.totalParticipants = dto.totalParticipants;
        }

        #region Public Properties

        /// <summary>
        /// Gets or sets the ac session id.
        /// </summary>
        [DataMember]
        public int acSessionId { get; set; }

        /// <summary>
        /// Gets or sets the ac user mode id.
        /// </summary>
        [DataMember]
        public int acUserModeId { get; set; }

        /// <summary>
        ///     Gets or sets the participants.
        /// </summary>
        [DataMember]
        public int activeParticipants { get; set; }

        /// <summary>
        /// Gets or sets the applet item id.
        /// </summary>
        [DataMember]
        public int appletItemId { get; set; }

        /// <summary>
        /// Gets or sets the applet name.
        /// </summary>
        [DataMember]
        public string appletName { get; set; }

        /// <summary>
        /// Gets or sets the category name.
        /// </summary>
        [DataMember]
        public string categoryName { get; set; }

        /// <summary>
        /// Gets or sets the date created.
        /// </summary>
        [DataMember]
        public double dateCreated { get; set; }

        /// <summary>
        /// Gets or sets the language.
        /// </summary>
        [DataMember]
        public string language { get; set; }

        /// <summary>
        /// Gets or sets the sub module item id.
        /// </summary>
        [DataMember]
        public int subModuleItemId { get; set; }

        /// <summary>
        ///     Gets or sets the participants.
        /// </summary>
        [DataMember]
        public int totalParticipants { get; set; }
        
        #endregion
    }
}