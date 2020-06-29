namespace EdugameCloud.Core.Domain.Entities
{
    using Esynctraining.Core.Domain.Entities;

    /// <summary>
    ///     The survey question result answer.
    /// </summary>
    public class SurveyQuestionResultAnswer : Entity
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets a value indicating whether is correct.
        /// </summary>
        public virtual string Value { get; set; }

        /// <summary>
        /// Gets or sets the question.
        /// </summary>
        public virtual SurveyQuestionResult SurveyQuestionResult { get; set; }

        /// <summary>
        /// Gets or sets the survey distractor.
        /// </summary>
        public virtual Distractor SurveyDistractor { get; set; }

        /// <summary>
        /// Gets or sets the survey distractor answer.
        /// </summary>
        public virtual Distractor SurveyDistractorAnswer { get; set; }

        #endregion
    }
}