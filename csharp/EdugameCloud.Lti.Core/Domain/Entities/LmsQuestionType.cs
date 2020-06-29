namespace EdugameCloud.Lti.Domain.Entities
{
    using Esynctraining.Core.Domain.Entities;

    /// <summary>
    /// The LMS question type.
    /// </summary>
    public class LmsQuestionType : Entity
    {
        /// <summary>
        /// Gets or sets the LMS provider.
        /// </summary>
        public virtual LmsProvider LmsProvider { get; set; }

        /// <summary>
        /// Gets or sets the question type.
        /// </summary>
        public virtual int QuestionTypeId { get; set; }

        /// <summary>
        /// Gets or sets the LMS question type name.
        /// </summary>
        public virtual string LmsQuestionTypeName { get; set; }

        /// <summary>
        /// Gets or sets the sub module id.
        /// </summary>
        public virtual int? SubModuleId { get; set; }
    }
}
