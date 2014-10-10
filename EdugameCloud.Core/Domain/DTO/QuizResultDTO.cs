namespace EdugameCloud.Core.Domain.DTO
{
    using System;
    using System.Runtime.Serialization;

    using EdugameCloud.Core.Domain.Entities;

    using Esynctraining.Core.Extensions;

    /// <summary>
    ///     The quiz result DTO.
    /// </summary>
    [DataContract]
    public class QuizResultDTO
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="QuizResultDTO"/> class.
        /// </summary>
        public QuizResultDTO()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QuizResultDTO"/> class.
        /// </summary>
        /// <param name="result">
        /// The result.
        /// </param>
        public QuizResultDTO(QuizResult result)
        {
            this.quizResultId = result.Id;
            this.acSessionId = result.ACSessionId;
            this.quizId = result.Quiz.With(x => x.Id);
            this.dateCreated = result.DateCreated;
            this.endTime = result.EndTime;
            this.participantName = result.ParticipantName;
            this.score = result.Score;
            this.startTime = result.StartTime;
            this.isArchive = result.IsArchive ?? false;
            this.email = result.Email;
            this.lmsId = result.LmsId;
            this.acEmail = result.ACEmail;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        [DataMember]
        public string email { get; set; }

        /// <summary>
        /// Gets or sets the AC email.
        /// </summary>
        [DataMember]
        public string acEmail { get; set; }

        /// <summary>
        ///     Gets or sets the applet name.
        /// </summary>
        [DataMember(IsRequired = true)]
        public int acSessionId { get; set; }

        /// <summary>
        ///     Gets or sets the quiz id.
        /// </summary>
        [DataMember(IsRequired = true)]
        public int quizId { get; set; }

        /// <summary>
        ///     Gets or sets the company id.
        /// </summary>
        [DataMember(IsRequired = true)]
        public int companyId { get; set; }

        /// <summary>
        ///     Gets or sets the date created.
        /// </summary>
        [DataMember]
        public DateTime dateCreated { get; set; }

        /// <summary>
        ///     Gets or sets the end time.
        /// </summary>
        [DataMember]
        public DateTime endTime { get; set; }

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        [DataMember]
        public int quizResultId { get; set; }

        /// <summary>
        ///     Gets or sets the participant name
        /// </summary>
        [DataMember]
        public string participantName { get; set; }

        /// <summary>
        ///     Gets or sets the score.
        /// </summary>
        [DataMember]
        public int score { get; set; }

        /// <summary>
        ///     Gets or sets the start time.
        /// </summary>
        [DataMember]
        public DateTime startTime { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is archive.
        /// </summary>
        [DataMember]
        public bool isArchive { get; set; }

        /// <summary>
        /// Gets or sets the quiz result id.
        /// </summary>
        [DataMember]
        public int lmsId { get; set; }

        #endregion
    }
}