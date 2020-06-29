namespace EdugameCloud.Core.Domain.DTO
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    /// The survey data DTO.
    /// </summary>
    [DataContract]
    public class SurveyDataDTO
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the distractors.
        /// </summary>
        [DataMember]
        public DistractorFromStoredProcedureDTO[] distractors { get; set; }

        /// <summary>
        /// Gets or sets the questions.
        /// </summary>
        [DataMember]
        public QuestionFromStoredProcedureDTO[] questions { get; set; }

        /// <summary>
        /// Gets or sets the survey vo.
        /// </summary>
        [DataMember]
        public SurveyDTO surveyVO { get; set; }

        #endregion
    }
}