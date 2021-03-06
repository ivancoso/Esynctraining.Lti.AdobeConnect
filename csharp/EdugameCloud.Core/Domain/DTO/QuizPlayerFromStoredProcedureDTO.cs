﻿namespace EdugameCloud.Core.Domain.DTO
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    ///     The quiz player DTO.
    /// </summary>
    [DataContract]
    public class QuizPlayerFromStoredProcedureDTO
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

        [DataMember]
        public virtual bool isCompleted { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is completed.
        /// </summary>
        [DataMember]
        public virtual int? appMaximizedTime { get; set; }

        [DataMember]
        public virtual int? appInFocusTime { get; set; }

        [DataMember]
        public virtual int passingScore { get; set; }

        [DataMember]
        public bool isPostQuiz { get; set; }

        [DataMember]
        public Guid quizResultGuid { get; set; }

        #endregion
    }
}