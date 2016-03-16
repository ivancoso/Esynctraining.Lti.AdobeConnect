namespace EdugameCloud.Core.Domain.DTO
{
    using System.Runtime.Serialization;

    using Esynctraining.Core.Extensions;

    /// <summary>
    ///     The test player DTO.
    /// </summary>
    [DataContract]
    public class TestPlayerDTO
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TestPlayerDTO"/> class.
        /// </summary>
        public TestPlayerDTO()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TestPlayerDTO"/> class.
        /// </summary>
        /// <param name="dto">
        /// The DTO.
        /// </param>
        public TestPlayerDTO(TestPlayerFromStoredProcedureDTO dto)
        {
            this.timePassed = dto.timePassed;
            this.scorePassed = dto.scorePassed;
            this.TotalQuestion = dto.TotalQuestion;
            this.endTime = dto.endTime.ConvertToUnixTimestamp();
            this.participantName = dto.participantName;
            this.acEmail = dto.acEmail;
            this.position = dto.position;
            this.testResultId = dto.testResultId;
            this.score = dto.score;
            this.startTime = dto.startTime.ConvertToUnixTimestamp();
            this.timeLimit = dto.timeLimit;
            this.passingScore = dto.passingScore;
            this.isCompleted = dto.isCompleted;
        }

        #region Public Properties

        /// <summary>
        /// Gets or sets a value indicating whether time passed.
        /// </summary>
        [DataMember]
        public bool timePassed { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether score passed.
        /// </summary>
        [DataMember]
        public bool scorePassed { get; set; }

        /// <summary>
        /// Gets or sets the total question.
        /// </summary>
        // NOTE: not in use on client-side [DataMember]
        public long TotalQuestion { get; set; }

        /// <summary>
        /// Gets or sets the end time. (Unix Timestamp value)
        /// </summary>
        [DataMember]
        public double endTime { get; set; }

        /// <summary>
        /// Gets or sets the participant name.
        /// </summary>
        [DataMember]
        public string participantName { get; set; }

        /// <summary>
        /// Gets or sets the Adobe Connect email.
        /// </summary>
        [DataMember]
        public string acEmail { get; set; }

        /// <summary>
        /// Gets or sets the position.
        /// </summary>
        [DataMember]
        public long position { get; set; }

        /// <summary>
        /// Gets or sets the test result id.
        /// </summary>
        [DataMember]
        public int testResultId { get; set; }

        /// <summary>
        /// Gets or sets the score.
        /// </summary>
        [DataMember]
        public int score { get; set; }

        /// <summary>
        /// Gets or sets the start time. (Unix Timestamp value)
        /// </summary>
        [DataMember]
        public double startTime { get; set; }

        /// <summary>
        /// Gets or sets the time limit.
        /// </summary>
        [DataMember]
        public int? timeLimit { get; set; }

        /// <summary>
        /// Gets or sets the passing score.
        /// </summary>
        [DataMember]
        public decimal? passingScore { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is completed.
        /// </summary>
        [DataMember]
        public bool isCompleted { get; set; }

        #endregion
    }
}