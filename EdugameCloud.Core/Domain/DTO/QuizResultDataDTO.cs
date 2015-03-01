namespace EdugameCloud.Core.Domain.DTO
{
    using System.Runtime.Serialization;

    /// <summary>
    /// The quiz result data DTO.
    /// </summary>
    [DataContract]
    public class QuizResultDataDTO
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the players.
        /// </summary>
        [DataMember]
        public QuizPlayerDTO[] players { get; set; }

        /// <summary>
        /// Gets or sets the questions.
        /// </summary>
        [DataMember]
        public QuestionForAdminDTO[] questions { get; set; }

        #endregion
    }
}