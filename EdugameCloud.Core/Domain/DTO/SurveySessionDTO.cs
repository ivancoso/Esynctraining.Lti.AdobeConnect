namespace EdugameCloud.Core.Domain.DTO
{
    using System.Runtime.Serialization;
    using Esynctraining.Core.Extensions;

    /// <summary>
    /// The survey session DTO.
    /// </summary>
    [DataContract]
    public sealed class SurveySessionDTO : AdobeConnectSessionDtoBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SurveySessionDTO"/> class.
        /// </summary>
        /// <param name="dto">
        /// The DTO.
        /// </param>
        public SurveySessionDTO(SurveySessionFromStoredProcedureDTO dto)
        {
            this.acSessionId = dto.acSessionId;
            this.acUserModeId = dto.acUserModeId;
            this.activeParticipants = dto.activeParticipants;
            this.categoryName = dto.categoryName;
            this.dateCreated = dto.dateCreated.With(x => x.ConvertToUnixTimestamp());
            this.language = dto.language;
            this.subModuleItemId = dto.subModuleItemId;
            this.surveyName = dto.surveyName;
            this.totalParticipants = dto.totalParticipants;
        }

        #region Public Properties
        
        /// <summary>
        /// Gets or sets the survey name.
        /// </summary>
        [DataMember]
        public string surveyName { get; set; }
        
        #endregion
    }
}