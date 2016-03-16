namespace EdugameCloud.Core.Domain.DTO
{
    using System.Runtime.Serialization;

    using Esynctraining.Core.Extensions;

    /// <summary>
    /// The SN session DTO.
    /// </summary>
    [DataContract]
    public sealed class SNSessionDTO : AdobeConnectSessionDtoBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SNSessionDTO"/> class.
        /// </summary>
        /// <param name="dto">
        /// The DTO.
        /// </param>
        public SNSessionDTO(SNSessionFromStoredProcedureDTO dto)
        {
            this.acSessionId = dto.acSessionId;
            this.acUserModeId = dto.acUserModeId;
            this.categoryName = dto.categoryName;
            this.dateCreated = dto.dateCreated.ConvertToUnixTimestamp();
            this.snGroupDiscussionId = dto.snGroupDiscussionId;
            this.groupDiscussionTitle = dto.groupDiscussionTitle;
            this.language = dto.language;
            this.activeParticipants = dto.activeParticipants;
            this.totalParticipants = dto.totalParticipants;
            this.profileName = dto.profileName;
            this.snProfileId = dto.snProfileId;
            this.subModuleItemId = dto.subModuleItemId;
        }

        #region Public Properties
      
        /// <summary>
        /// Gets or sets the group discussion id.
        /// </summary>
        [DataMember]
        public int snGroupDiscussionId { get; set; }

        /// <summary>
        /// Gets or sets the group discussion name.
        /// </summary>
        [DataMember]
        public string groupDiscussionTitle { get; set; }
        
        /// <summary>
        /// Gets or sets the SN profile name.
        /// </summary>
        [DataMember]
        public string profileName { get; set; }

        /// <summary>
        /// Gets or sets the SN profile id.
        /// </summary>
        [DataMember]
        public int snProfileId { get; set; }

        #endregion

    }

}