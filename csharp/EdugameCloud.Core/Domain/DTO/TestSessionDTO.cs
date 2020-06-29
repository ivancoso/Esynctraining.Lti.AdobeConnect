namespace EdugameCloud.Core.Domain.DTO
{
    using System.Runtime.Serialization;

    using Esynctraining.Core.Extensions;

    /// <summary>
    /// The test session DTO.
    /// </summary>
    [DataContract]
    public sealed class TestSessionDTO : AdobeConnectSessionDtoBase
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
            //this.TotalQuestion = dto.TotalQuestion;
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
            //this.userId = dto.userId;
        }

        #region Public Properties

        /// <summary>
        /// Gets or sets the passing score.
        /// </summary>
        [DataMember]
        public decimal passingScore { get; set; }
        
        /// <summary>
        /// Gets or sets the total score.
        /// </summary>
        // TRICK: doesn't not used on client-side [DataMember]
        public int TotalScore { get; set; }

        /// <summary>
        /// Gets or sets the include AC emails.
        /// </summary>
        // TRICK: doesn't not used on client-side [DataMember]
        public bool? includeAcEmails { get; set; }

        /// <summary>
        /// Gets or sets the AVG score.
        /// </summary>
        [DataMember]
        public double avgScore { get; set; }

        /// <summary>
        /// Gets or sets the test name.
        /// </summary>
        [DataMember]
        public string testName { get; set; }

        #endregion

    }

}