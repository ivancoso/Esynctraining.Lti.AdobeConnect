using System;

namespace EdugameCloud.Core.Domain.DTO
{
    using System.Runtime.Serialization;

    using Esynctraining.Core.Extensions;

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
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            this.TotalQuestion = dto.TotalQuestion;
            this.endTime = dto.endTime.ConvertToUnixTimestamp();
            this.acEmail = dto.acEmail;
            this.participantName = dto.participantName;
            this.position = dto.position;
            this.quizResultId = dto.quizResultId;
            this.score = dto.score;
            this.startTime = dto.startTime.ConvertToUnixTimestamp();
            this.isCompleted = dto.isCompleted;
            this.isPostQuiz = dto.isPostQuiz;
            this.quizResultGuid = dto.quizResultGuid;
            var theScore = 
                dto.passingScore == 0 
                || dto.TotalQuestion <= 0 
                || (float)dto.score/dto.TotalQuestion >= (float)dto.passingScore/ 100;

            var scoreWithPercents =
                theScore
                && (dto.appMaximizedTime.HasValue && dto.appMaximizedTime.Value >= 95) 
                && (dto.appInFocusTime.HasValue && dto.appInFocusTime.Value >= 95);

            this.isParticipated = dto.appMaximizedTime == null || dto.appInFocusTime == null || scoreWithPercents;

            if (dto.isPostQuiz)
                this.isParticipated = theScore;
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

        /// <summary>
        /// Gets or sets of a flag isParticipated. Formula - not less than 95% for each parameter of (inFocusIndex, maximizedIndex)
        /// </summary>
        [DataMember]
        public bool isParticipated { get; set; }
        [DataMember]
        public bool isPostQuiz { get; set; }

        [DataMember]
        public Guid quizResultGuid { get; set; }

        [DataMember]
        public string certDownloadUrl { get; set; }
        [DataMember]
        public string certPreviewUrl { get; set; }

        #endregion

        public string BuildDownloadUrl(string root)
        {
            return $"{root}/quiz-certificate/{quizResultGuid}/download";
        }

        public string BuildPreviewUrl(string root)
        {
            return $"{root}/quiz-certificate/{quizResultGuid}/preview";
        }

    }

}