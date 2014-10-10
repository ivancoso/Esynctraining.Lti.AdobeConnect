namespace EdugameCloud.Core.Domain.DTO
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    ///     The quiz player DTO.
    /// </summary>
    [DataContract]
    public class QuizPlayerDTO
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the total question.
        /// </summary>
        [DataMember]
        public virtual int TotalQuestion { get; set; }

        /// <summary>
        /// Gets or sets the end time.
        /// </summary>
        [DataMember]
        public virtual DateTime endTime { get; set; }

        /// <summary>
        /// Gets or sets the AC email.
        /// </summary>
        [DataMember]
        public virtual string acEmail { get; set; }

        /// <summary>
        /// Gets or sets the participant name.
        /// </summary>
        [DataMember]
        public virtual string participantName { get; set; }

        /// <summary>
        /// Gets or sets the position.
        /// </summary>
        [DataMember]
        public virtual long position { get; set; }

        /// <summary>
        /// Gets or sets the quiz result id.
        /// </summary>
        [DataMember]
        public virtual int quizResultId { get; set; }

        /// <summary>
        /// Gets or sets the score.
        /// </summary>
        [DataMember]
        public virtual int score { get; set; }

        /// <summary>
        /// Gets or sets the start time.
        /// </summary>
        [DataMember]
        public virtual DateTime startTime { get; set; }

        #endregion
    }
}