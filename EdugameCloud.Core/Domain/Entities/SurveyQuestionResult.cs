namespace EdugameCloud.Core.Domain.Entities
{
    using Esynctraining.Core.Domain.Entities;

    using Iesi.Collections.Generic;

    /// <summary>
    ///     The survey question result.
    /// </summary>
    public class SurveyQuestionResult : Entity
    {
        #region Public Properties

        /// <summary>
        /// The answers.
        /// </summary>
        private ISet<SurveyQuestionResultAnswer> answers = new HashedSet<SurveyQuestionResultAnswer>();

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
        /// Gets or sets the survey result.
        /// </summary>
        public virtual SurveyResult SurveyResult { get; set; }

        /// <summary>
        /// Gets or sets the results.
        /// </summary>
        public virtual ISet<SurveyQuestionResultAnswer> Answers
        {
            get
            {
                return this.answers;
            }

            set
            {
                this.answers = value;
            }
        }

        #endregion
    }
}