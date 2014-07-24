namespace EdugameCloud.Core.Domain.Entities
{
    using System;

    using Esynctraining.Core.Domain.Entities;

    using Iesi.Collections.Generic;

    /// <summary>
    ///     The quiz result.
    /// </summary>
    public class QuizResult : Entity
    {
        #region Fields

        /// <summary>
        /// The results.
        /// </summary>
        private ISet<QuizQuestionResult> results = new HashedSet<QuizQuestionResult>();

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        public virtual string Email { get; set; }

        /// <summary>
        /// Gets or sets the is archive.
        /// </summary>
        public virtual bool? IsArchive { get; set; }

        /// <summary>
        ///     Gets or sets the ac session.
        /// </summary>
        public virtual int ACSessionId { get; set; }

        /// <summary>
        ///     Gets or sets the date created.
        /// </summary>
        public virtual DateTime DateCreated { get; set; }

        /// <summary>
        ///     Gets or sets the end time.
        /// </summary>
        public virtual DateTime EndTime { get; set; }

        /// <summary>
        ///     Gets or sets the participant name.
        /// </summary>
        public virtual string ParticipantName { get; set; }

        /// <summary>
        ///     Gets or sets the quiz.
        /// </summary>
        public virtual Quiz Quiz { get; set; }

        /// <summary>
        /// Gets or sets the results.
        /// </summary>
        public virtual ISet<QuizQuestionResult> Results
        {
            get
            {
                return this.results;
            }

            set
            {
                this.results = value;
            }
        }

        /// <summary>
        ///     Gets or sets the score.
        /// </summary>
        public virtual int Score { get; set; }

        /// <summary>
        ///     Gets or sets the start time.
        /// </summary>
        public virtual DateTime StartTime { get; set; }

        /// <summary>
        ///     Gets or sets the lms id.
        /// </summary>
        public virtual int LmsId { get; set; }

        #endregion
    }
}