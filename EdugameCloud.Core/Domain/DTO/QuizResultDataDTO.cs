namespace EdugameCloud.Core.Domain.DTO
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    /// The quiz result data dto.
    /// </summary>
    [DataContract]
    public class QuizResultDataDTO
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the players.
        /// </summary>
        [DataMember]
        public List<QuizPlayerDTO> players { get; set; }

        /// <summary>
        /// Gets or sets the questions.
        /// </summary>
        [DataMember]
        public List<QuestionForAdminDTO> questions { get; set; }

        #endregion
    }
}