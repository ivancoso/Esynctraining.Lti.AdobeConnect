namespace EdugameCloud.Core.Domain.Entities
{
    using Esynctraining.Core.Domain.Entities;

    /// <summary>
    ///     The quiz question result.
    /// </summary>
    public class QuizQuestionResult : Entity
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets a value indicating whether is correct.
        /// </summary>
        public virtual bool IsCorrect { get; set; }

        /// <summary>
        /// questionId (FK for QuestionRef)
        /// </summary>
        public virtual int QuestionId { get; set; }

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

        #endregion
    }
}