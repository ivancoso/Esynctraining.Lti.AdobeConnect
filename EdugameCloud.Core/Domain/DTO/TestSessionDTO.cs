namespace EdugameCloud.Core.Domain.DTO
{
    using System.Runtime.Serialization;

    using EdugameCloud.Core.Extensions;

    /// <summary>
    ///     The test session DTO.
    /// </summary>
    [DataContract]
    public class TestSessionDTO
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TestSessionDTO"/> class.
        /// </summary>
        public TestSessionDTO()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TestSessionDTO"/> class.
        /// </summary>
        /// <param name="dto">
        /// The DTO.
        /// </param>
        public TestSessionDTO(TestSessionFromStoredProcedureDTO dto)
        {
            this.passingScore = dto.passingScore;
            this.TotalQuestion = dto.TotalQuestion;
            this.TotalScore = dto.TotalScore;
            this.includeAcEmails = dto.includeAcEmails;
            this.avgScore = dto.avgScore;
            this.acSessionId = dto.acSessionId;
            this.acUserModeId = dto.acUserModeId;
            this.categoryName = dto.categoryName;
            this.dateCreated = dto.dateCreated.ConvertToUnixTimestamp();
            this.language = dto.language;
            this.activeParticipants = dto.activeParticipants;
            this.totalParticipants = dto.totalParticipants;
            this.testName = dto.testName;
            this.subModuleItemId = dto.subModuleItemId;
            this.userId = dto.userId;
        }

        #region Public Properties

        /// <summary>
        /// Gets or sets the passing score.
        /// </summary>
        [DataMember]
        public decimal passingScore { get; set; }

        /// <summary>
        /// Gets or sets the total question.
        /// </summary>
        [DataMember]
        public int TotalQuestion { get; set; }

        /// <summary>
        /// Gets or sets the total score.
        /// </summary>
        [DataMember]
        public int TotalScore { get; set; }

        /// <summary>
        /// Gets or sets the include AC emails.
        /// </summary>
        [DataMember]
        public bool? includeAcEmails { get; set; }

        /// <summary>
        /// Gets or sets the AVG score.
        /// </summary>
        [DataMember]
        public double avgScore { get; set; }

        /// <summary>
        /// Gets or sets the AC session id.
        /// </summary>
        [DataMember]
        public int acSessionId { get; set; }

        /// <summary>
        /// Gets or sets the AC user mode id.
        /// </summary>
        [DataMember]
        public int acUserModeId { get; set; }

        /// <summary>
        /// Gets or sets the category name.
        /// </summary>
        [DataMember]
        public string categoryName { get; set; }

        /// <summary>
        /// Gets or sets the date created.
        /// </summary>
        [DataMember]
        public double dateCreated { get; set; }

        /// <summary>
        /// Gets or sets the language.
        /// </summary>
        [DataMember]
        public string language { get; set; }

        /// <summary>
        /// Gets or sets the participants.
        /// </summary>
        [DataMember]
        public int activeParticipants { get; set; }

        /// <summary>
        /// Gets or sets the participants.
        /// </summary>
        [DataMember]
        public int totalParticipants { get; set; }

        /// <summary>
        /// Gets or sets the quiz name.
        /// </summary>
        [DataMember]
        public string testName { get; set; }

        /// <summary>
        /// Gets or sets the sub module item id.
        /// </summary>
        [DataMember]
        public int subModuleItemId { get; set; }

        /// <summary>
        /// Gets or sets the user id.
        /// </summary>
        [DataMember]
        public int userId { get; set; }

        #endregion
    }
}