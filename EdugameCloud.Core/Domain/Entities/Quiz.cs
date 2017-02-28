namespace EdugameCloud.Core.Domain.Entities
{
    using System.Collections.Generic;
    using Esynctraining.Core.Domain.Entities;

    /// <summary>
    ///     The quiz.
    /// </summary>
    public class Quiz : Entity
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        public virtual string Description { get; set; }

        /// <summary>
        /// Gets or sets the quiz format.
        /// </summary>
        public virtual QuizFormat QuizFormat { get; set; }

        /// <summary>
        /// Gets or sets the quiz name.
        /// </summary>
        public virtual string QuizName { get; set; }

        /// <summary>
        /// Gets or sets the results.
        /// </summary>
        public virtual IList<QuizResult> Results { get; protected set; }

        /// <summary>
        /// Gets or sets the score type.
        /// </summary>
        public virtual ScoreType ScoreType { get; set; }

        /// <summary>
        /// Gets or sets the sub module item.
        /// </summary>
        public virtual SubModuleItem SubModuleItem { get; set; }

        /// <summary>
        /// Gets or sets the lms id.
        /// </summary>
        public virtual int? LmsQuizId { get; set; }

        public virtual bool IsPostQuiz { get; set; }

        public virtual int PassingScore { get; set; }

        #endregion

        public Quiz()
        { 
            Results = new List<QuizResult>();
        }
    }
}