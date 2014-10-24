namespace EdugameCloud.Core.Domain.Entities
{
    using Esynctraining.Core.Domain.Entities;

    /// <summary>
    /// The lms question type.
    /// </summary>
    public class LmsQuestionType : Entity
    {
        /// <summary>
        /// Gets or sets the lms provider.
        /// </summary>
        public virtual LmsProvider LmsProvider { get; set; }

        /// <summary>
        /// Gets or sets the question type.
        /// </summary>
        public virtual QuestionType QuestionType { get; set; }

        /// <summary>
        /// Gets or sets the lms question type name.
        /// </summary>
        public virtual string LmsQuestionTypeName { get; set; }
    }
}
