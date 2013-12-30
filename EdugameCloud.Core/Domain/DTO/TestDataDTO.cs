namespace EdugameCloud.Core.Domain.DTO
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    /// The test data DTO.
    /// </summary>
    [DataContract]
    public class TestDataDTO
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
        /// Gets or sets the test vo.
        /// </summary>
        [DataMember]
        public TestDTO testVO { get; set; }

        #endregion
    }
}