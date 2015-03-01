namespace EdugameCloud.Core.Domain.DTO
{
    using System.Runtime.Serialization;
    using EdugameCloud.Core.Extensions;
    using Esynctraining.Core.Extensions;

    /// <summary>
    ///     The survey session DTO.
    /// </summary>
    [DataContract]
    public class SurveySessionDTO
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SurveySessionDTO"/> class.
        /// </summary>
        /// <param name="dto">
        /// The DTO.
        /// </param>
        public SurveySessionDTO(SurveySessionFromStoredProcedureDTO dto)
        {
            this.TotalQuestion = dto.TotalQuestion;
            this.TotalScore = dto.TotalScore;
            this.acSessionId = dto.acSessionId;
            this.acUserModeId = dto.acUserModeId;
            this.activeParticipants = dto.activeParticipants;
            this.categoryName = dto.categoryName;
            this.dateCreated = dto.dateCreated.With(x => x.ConvertToUnixTimestamp());
            this.language = dto.language;
            this.subModuleItemId = dto.subModuleItemId;
            this.surveyName = dto.surveyName;
            this.totalParticipants = dto.totalParticipants;
            this.userId = dto.userId;
        }

        #region Public Properties

        /// <summary>
        /// Gets or sets the total question.
        /// </summary>
        [DataMember]
        public int TotalQuestion { get; set; }

        /// <summary>
        /// Gets or sets the total score.
        /// </summary>
        [DataMember]
        public int TotalScore { get; set; }

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
        /// Gets or sets the category name.
        /// </summary>
        [DataMember]
        public string categoryName { get; set; }

        /// <summary>
        /// Gets or sets the language.
        /// </summary>
        [DataMember]
        public string language { get; set; }

        /// <summary>
        /// Gets or sets the participants.
        /// </summary>
        [DataMember]
        public int activeParticipants { get; set; }

        /// <summary>
        /// Gets or sets the participants.
        /// </summary>
        [DataMember]
        public int totalParticipants { get; set; }

        /// <summary>
        /// Gets or sets the sub module item id.
        /// </summary>
        [DataMember]
        public int subModuleItemId { get; set; }

        /// <summary>
        /// Gets or sets the survey name.
        /// </summary>
        [DataMember]
        public string surveyName { get; set; }

        /// <summary>
        /// Gets or sets the user id.
        /// </summary>
        [DataMember]
        public int userId { get; set; }

        /// <summary>
        /// Gets or sets the date created.
        /// </summary>
        [DataMember]
        public double dateCreated { get; set; }

        #endregion
    }
}