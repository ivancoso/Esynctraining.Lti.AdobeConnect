namespace EdugameCloud.Core.Domain.Entities
{
    using Esynctraining.Core.Domain.Entities;

    using Iesi.Collections.Generic;

    /// <summary>
    ///     The quiz.
    /// </summary>
    public class Quiz : Entity
    {
        #region Fields

        /// <summary>
        /// The results.
        /// </summary>
        private ISet<QuizResult> results = new HashedSet<QuizResult>();

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the description.
        /// </summary>
        public virtual string Description { get; set; }

        /// <summary>
        ///     Gets or sets the quiz format.
        /// </summary>
        public virtual QuizFormat QuizFormat { get; set; }

        /// <summary>
        ///     Gets or sets the quiz name.
        /// </summary>
        public virtual string QuizName { get; set; }

        /// <summary>
        /// Gets or sets the results.
        /// </summary>
        public virtual ISet<QuizResult> Results
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
        ///     Gets or sets the score type.
        /// </summary>
        public virtual ScoreType ScoreType { get; set; }

        /// <summary>
        ///     Gets or sets the sub module item.
        /// </summary>
        public virtual SubModuleItem SubModuleItem { get; set; }

        /// <summary>
        ///     Gets or sets the moodle id.
        /// </summary>
        public virtual int MoodleId { get; set; }

        #endregion
    }
}