namespace EdugameCloud.Core.Domain.DTO
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    /// The quizes and sub module items dto.
    /// </summary>
    [DataContract]
    public class QuizesAndSubModuleItemsDTO
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the quizes.
        /// </summary>
        [DataMember]
        public List<QuizFromStoredProcedureDTO> quizzes { get; set; }

        /// <summary>
        /// Gets or sets the sub module items.
        /// </summary>
        [DataMember]
        public List<SubModuleItemDTO> subModuleItems { get; set; }

        #endregion
    }
}