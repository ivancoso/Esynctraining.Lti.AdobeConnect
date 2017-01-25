using System.Collections.Generic;

namespace EdugameCloud.Core.Domain.Entities
{
    using Esynctraining.Core.Domain.Entities;

    /// <summary>
    ///     The quiz question result.
    /// </summary>
    public class QuizQuestionResult : Entity
    {
        public QuizQuestionResult()
        {
            Answers = new List<QuizQuestionResultAnswer>();
        }

        #region Public Properties

        /// <summary>
        /// Gets or sets a value indicating whether is correct.
        /// </summary>
        public virtual bool IsCorrect { get; set; }

        /// <summary>
        /// Gets or sets the question.
        /// </summary>
        public virtual Question QuestionRef { get; set; }

        /// <summary>
        /// Gets or sets the question type.
        /// </summary>
        public virtual QuestionType QuestionType { get; set; }

        /// <summary>
        /// Gets or sets the question string.
        /// </summary>
        public virtual string Question { get; set; }

        /// <summary>
        /// Gets or sets the quiz result.
        /// </summary>
        public virtual QuizResult QuizResult { get; set; }

        public virtual IList<QuizQuestionResultAnswer> Answers { get; protected set; }
        #endregion
    }
}