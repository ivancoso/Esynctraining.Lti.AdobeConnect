namespace EdugameCloud.Core.Domain.Entities
{
    using System;
    using System.Collections.Generic;
    using Esynctraining.Core.Domain.Entities;
    using Iesi.Collections.Generic;

    /// <summary>
    ///     The survey result.
    /// </summary>
    public class SurveyResult : Entity
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the ac session.
        /// </summary>
        public virtual int ACSessionId { get; set; }

        /// <summary>
        /// Gets or sets the date created.
        /// </summary>
        public virtual DateTime DateCreated { get; set; }

        /// <summary>
        /// Gets or sets the end time.
        /// </summary>
        public virtual DateTime EndTime { get; set; }

        /// <summary>
        /// Gets or sets the participant name.
        /// </summary>
        public virtual string ParticipantName { get; set; }

        /// <summary>
        /// Gets or sets the results.
        /// </summary>
        public virtual IList<SurveyQuestionResult> Results { get; protected set; }

        /// <summary>
        /// Gets or sets the score.
        /// </summary>
        public virtual int Score { get; set; }

        /// <summary>
        /// Gets or sets the start time.
        /// </summary>
        public virtual DateTime StartTime { get; set; }

        /// <summary>
        /// Gets or sets the survey.
        /// </summary>
        public virtual Survey Survey { get; set; }

        /// <summary>
        /// Gets or sets the is archive.
        /// </summary>
        public virtual bool? IsArchive { get; set; }

        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        public virtual string Email { get; set; }

        /// <summary>
        /// Gets or sets the LMS user parameters.
        /// </summary>
        public virtual int? LmsUserParametersId { get; set; }

        #endregion

        public SurveyResult()
        {
            Results = new List<SurveyQuestionResult>();
        }

    }

}