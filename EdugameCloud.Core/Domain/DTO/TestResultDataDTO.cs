namespace EdugameCloud.Core.Domain.DTO
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    /// The test result data DTO.
    /// </summary>
    [DataContract]
    public class TestResultDataDTO
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the players.
        /// </summary>
        [DataMember]
        public List<TestPlayerDTO> players { get; set; }

        /// <summary>
        /// Gets or sets the questions.
        /// </summary>
        [DataMember]
        public List<QuestionForAdminDTO> questions { get; set; }

        #endregion
    }
}