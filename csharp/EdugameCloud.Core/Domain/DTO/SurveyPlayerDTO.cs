namespace EdugameCloud.Core.Domain.DTO
{
    using System.Runtime.Serialization;

    using Esynctraining.Core.Extensions;

    /// <summary>
    /// The survey player DTO.
    /// </summary>
    [DataContract]
    public class SurveyPlayerDTO
    {
        public SurveyPlayerDTO()
        {
        }

        #region Public Properties

        public SurveyPlayerDTO(SurveyPlayerFromStoredProcedureDTO dto)
        {
            this.answers = dto.answers;
            this.acEmail = dto.acEmail;
            this.TotalQuestion = dto.TotalQuestion;
            this.endTime = dto.endTime.ConvertToUnixTimestamp();
            this.participantName = dto.participantName;
            this.position = dto.position;
            this.surveyResultId = dto.surveyResultId;
            this.score = dto.score;
            this.startTime = dto.startTime.ConvertToUnixTimestamp();
        }

        /// <summary>
        /// Gets or sets the answers.
        /// </summary>
        [DataMember]
        public SurveyQuestionResultAnswerDTO[] answers { get; set; }

        /// <summary>
        /// Gets or sets the total question.
        /// </summary>
        [DataMember]
        public int TotalQuestion { get; set; }

        /// <summary>
        /// Gets or sets the end time. (Unix Timestamp value)
        /// </summary>
        [DataMember]
        public double endTime { get; set; }

        /// <summary>
        /// Gets or sets the participant name.
        /// </summary>
        [DataMember]
        public string participantName { get; set; }

        /// <summary>
        /// Gets or sets the position.
        /// </summary>
        [DataMember]
        public long position { get; set; }

        /// <summary>
        /// Gets or sets the survey result id.
        /// </summary>
        [DataMember]
        public int surveyResultId { get; set; }

        /// <summary>
        /// Gets or sets the score.
        /// </summary>
        [DataMember]
        public int score { get; set; }

        /// <summary>
        /// Gets or sets the start time. (Unix Timestamp value)
        /// </summary>
        [DataMember]
        public double startTime { get; set; }

        /// <summary>
        /// Gets or sets the AdobeConnect email.
        /// </summary>
        [DataMember]
        public string acEmail { get; set; }

        #endregion

    }

}