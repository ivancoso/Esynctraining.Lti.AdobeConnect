using System;
using System.Runtime.Serialization;
using EdugameCloud.Core.Domain.Entities;
using Esynctraining.Core.Extensions;

namespace EdugameCloud.Core.Domain.DTO
{
    /// <summary>
    /// The quiz result DTO.
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
            if (result == null)
                throw new ArgumentNullException(nameof(result));

            quizResultId = result.Id;
            acSessionId = result.ACSessionId;
            dateCreated = result.DateCreated.ConvertToUnixTimestamp();
            endTime = result.EndTime.ConvertToUnixTimestamp();
            participantName = result.ParticipantName;
            score = result.Score;
            startTime = result.StartTime.ConvertToUnixTimestamp();
            isArchive = result.IsArchive ?? false;
            email = result.Email;
            lmsId = result.LmsId;
            lmsUserParametersId = result.LmsUserParametersId ?? 0;
            acEmail = result.ACEmail;
            isCompleted = result.isCompleted ?? false;
            appInFocusTime = result.AppInFocusTime;
            appMaximizedTime = result.AppMaximizedTime;
            guid = result.Guid;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        [DataMember]
        public string email { get; set; }

        /// <summary>
        /// Gets or sets the Adobe Connect email.
        /// </summary>
        [DataMember]
        public string acEmail { get; set; }

        /// <summary>
        /// Gets or sets the date created.
        /// </summary>
        [DataMember]
        public double dateCreated { get; set; }

        /// <summary>
        /// Gets or sets the end time. (Unix Timestamp value)
        /// </summary>
        [DataMember]
        public double endTime { get; set; }

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        [DataMember]
        public int quizResultId { get; set; }

        /// <summary>
        /// Gets or sets the participant name
        /// </summary>
        [DataMember]
        public string participantName { get; set; }

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
        /// Gets or sets a value indicating whether is archive.
        /// </summary>
        [DataMember]
        public bool isArchive { get; set; }

        /// <summary>
        /// Gets or sets the quiz result id.
        /// </summary>
        [DataMember]
        public int lmsId { get; set; }

        /// <summary>
        /// Gets or sets the LMS user parameters id.
        /// </summary>
        [DataMember]
        public int lmsUserParametersId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is completed.
        /// </summary>
        [DataMember]
        public bool isCompleted { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is completed.
        /// </summary>
        [DataMember]
        public int appInFocusTime { get; set; }

        [DataMember]
        public int appMaximizedTime { get; set; }

        [DataMember]
        public Guid guid { get; set; }

        [DataMember]
        public QuizQuestionResultDTO[] results { get; set; }

        public int acSessionId { get; set; }

        #endregion

    }

}