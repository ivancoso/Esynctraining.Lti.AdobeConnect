using System.Runtime.Serialization;

namespace Esynctraining.Lti.Lms.Common.Dto.Moodle
{
    /// <summary>
    /// The moodle quiz result DTO.
    /// </summary>
    [DataContract]
    public class MoodleQuizResultDTO
    {
        /// <summary>
        /// Gets or sets the quiz id.
        /// </summary>
        [DataMember]
        public virtual int quizId { get; set; }

        /// <summary>
        /// Gets or sets the question id.
        /// </summary>
        [DataMember]
        public virtual int questionId { get; set; }

        /// <summary>
        /// Gets or sets the user id.
        /// </summary>
        [DataMember]
        public virtual string userId { get; set; }

        /// <summary>
        /// Gets or sets the start time.
        /// </summary>
        [DataMember]
        public virtual int startTime { get; set; }

        /// <summary>
        /// Gets or sets the answers.
        /// </summary>
        [DataMember]
        public virtual object[] answers { get; set; }

        /// <summary>
        /// Gets or sets the question type.
        /// </summary>
        [DataMember]
        public virtual string questionType { get; set; }
    }
}
