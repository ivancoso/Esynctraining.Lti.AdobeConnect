namespace EdugameCloud.Core.Domain.DTO
{
    using System.Runtime.Serialization;

    /// <summary>
    /// The quiz and sub module items DTO.
    /// </summary>
    [DataContract]
    public class QuizesAndSubModuleItemsDTO
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the quiz.
        /// </summary>
        [DataMember]
        public QuizFromStoredProcedureDTO[] quizzes { get; set; }

        /// <summary>
        /// Gets or sets the sub module items.
        /// </summary>
        [DataMember]
        public SubModuleItemDTO[] subModuleItems { get; set; }

        #endregion
    }
}