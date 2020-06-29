namespace EdugameCloud.Core.Domain.DTO
{
    using System.Runtime.Serialization;

    /// <summary>
    /// The quiz result data DTO.
    /// </summary>
    [KnownType(typeof(SurveyPlayerDTO))]
    [KnownType(typeof(QuestionForAdminDTO))]
    [DataContract]
    public class SurveyResultDataDTO
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the players.
        /// </summary>
        [DataMember]
        public SurveyPlayerDTO[] players { get; set; }

        /// <summary>
        /// Gets or sets the questions.
        /// </summary>
        [DataMember]
        public QuestionForAdminDTO[] questions { get; set; }

        #endregion
    }
}