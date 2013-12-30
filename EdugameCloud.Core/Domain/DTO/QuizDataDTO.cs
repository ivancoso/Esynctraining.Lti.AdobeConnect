namespace EdugameCloud.Core.Domain.DTO
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    /// The quiz data DTO.
    /// </summary>
    [DataContract]
    public class QuizDataDTO
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the distractors.
        /// </summary>
        [DataMember]
        public List<DistractorFromStoredProcedureDTO> distractors { get; set; }

        /// <summary>
        /// Gets or sets the questions.
        /// </summary>
        [DataMember]
        public List<QuestionFromStoredProcedureDTO> questions { get; set; }

        /// <summary>
        /// Gets or sets the quiz vo.
        /// </summary>
        [DataMember]
        public QuizDTO quizVO { get; set; }

        #endregion
    }
}