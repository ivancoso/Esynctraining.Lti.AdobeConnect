namespace EdugameCloud.Core.Domain.Entities
{
    using Esynctraining.Core.Domain.Entities;

    /// <summary>
    ///     The test question result.
    /// </summary>
    public class TestQuestionResult : Entity
    {
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
        /// Gets or sets the test result.
        /// </summary>
        public virtual TestResult TestResult { get; set; }

        #endregion
    }
}