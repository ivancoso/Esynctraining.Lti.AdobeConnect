namespace EdugameCloud.Core.Domain.DTO
{
    using System.Runtime.Serialization;

    using EdugameCloud.Core.Extensions;

    /// <summary>
    /// The crossword session from stored procedure DTO.
    /// </summary>
    [DataContract]
    public sealed class CrosswordSessionDTO : AdobeConnectSessionDtoBase
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
        /// Gets or sets the applet item id.
        /// </summary>
        [DataMember]
        public int appletItemId { get; set; }

        /// <summary>
        /// Gets or sets the applet name.
        /// </summary>
        [DataMember]
        public string appletName { get; set; }

        #endregion

    }

}