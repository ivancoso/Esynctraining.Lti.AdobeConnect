namespace EdugameCloud.Core.Domain.DTO
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    ///     The survey player DTO.
    /// </summary>
    [DataContract]
    public class SurveyPlayerDTO
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the answers.
        /// </summary>
        [DataMember]
        public List<SurveyQuestionResultAnswerDTO> answers { get; set; }

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
        /// Gets or sets the survey result id.
        /// </summary>
        [DataMember]
        public virtual int surveyResultId { get; set; }

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