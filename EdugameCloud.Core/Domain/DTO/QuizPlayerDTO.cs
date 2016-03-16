namespace EdugameCloud.Core.Domain.DTO
{
    using System.Runtime.Serialization;

    using Esynctraining.Core.Extensions;

    /// <summary>
    /// The quiz player DTO.
    /// </summary>
    [DataContract]
    public sealed class QuizPlayerDTO
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="QuizPlayerDTO"/> class.
        /// </summary>
        public QuizPlayerDTO()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QuizPlayerDTO"/> class.
        /// </summary>
        /// <param name="dto">
        /// The DTO.
        /// </param>
        public QuizPlayerDTO(QuizPlayerFromStoredProcedureDTO dto)
        {
            this.TotalQuestion = dto.TotalQuestion;
            this.endTime = dto.endTime.ConvertToUnixTimestamp();
            this.acEmail = dto.acEmail;
            this.participantName = dto.participantName;
            this.position = dto.position;
            this.quizResultId = dto.quizResultId;
            this.score = dto.score;
            this.startTime = dto.startTime.ConvertToUnixTimestamp();
            this.isCompleted = dto.isCompleted;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the total question.
        /// </summary>
        // NOTE: not in use on client side [DataMember]
        public int TotalQuestion { get; set; }

        /// <summary>
        /// Gets or sets the AdobeConnect email.
        /// </summary>
        [DataMember]
        public string acEmail { get; set; }

        /// <summary>
        /// Gets or sets the end time. (Unix Timestamp value)
        /// </summary>
        [DataMember]
        public double endTime { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is completed.
        /// </summary>
        [DataMember]
        public bool isCompleted { get; set; }

        /// <summary>
        /// Gets or sets the participant name.
        /// </summary>
        [DataMember]
        public string participantName { get; set; }

        /// <summary>
        /// Gets or sets the position.
        /// </summary>
        [DataMember]
        public long position { get; set; }

        /// <summary>
        /// Gets or sets the quiz result id.
        /// </summary>
        [DataMember]
        public int quizResultId { get; set; }

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

        #endregion

    }

}