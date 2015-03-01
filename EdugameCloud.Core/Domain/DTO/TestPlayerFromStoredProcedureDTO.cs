namespace EdugameCloud.Core.Domain.DTO
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    ///     The test player DTO.
    /// </summary>
    [DataContract]
    public class TestPlayerFromStoredProcedureDTO
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets a value indicating whether time passed.
        /// </summary>
        [DataMember]
        public virtual bool timePassed { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether score passed.
        /// </summary>
        [DataMember]
        public virtual bool scorePassed { get; set; }

        /// <summary>
        /// Gets or sets the total question.
        /// </summary>
        [DataMember]
        public virtual long TotalQuestion { get; set; }

        /// <summary>
        /// Gets or sets the end time.
        /// </summary>
        [DataMember]
        public virtual DateTime endTime { get; set; }

        /// <summary>
        /// Gets or sets the participant name.
        /// </summary>
        [DataMember]
        public virtual string participantName { get; set; }

        /// <summary>
        /// Gets or sets the AC email.
        /// </summary>
        [DataMember]
        public virtual string acEmail { get; set; }

        /// <summary>
        /// Gets or sets the position.
        /// </summary>
        [DataMember]
        public virtual long position { get; set; }

        /// <summary>
        /// Gets or sets the test result id.
        /// </summary>
        [DataMember]
        public virtual int testResultId { get; set; }

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
        /// Gets or sets the start time.
        /// </summary>
        [DataMember]
        public virtual bool isCompleted { get; set; }

        #endregion
    }
}