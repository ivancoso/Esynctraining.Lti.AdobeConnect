namespace EdugameCloud.Core.Domain.Entities
{
    using Esynctraining.Core.Domain.Entities;

    /// <summary>
    ///     The question type.
    /// </summary>
    public class QuestionType : Entity
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the correct text.
        /// </summary>
        public virtual string CorrectText { get; set; }

        /// <summary>
        /// Gets or sets the icon source.
        /// </summary>
        public virtual string IconSource { get; set; }

        /// <summary>
        /// Gets or sets the incorrect message.
        /// </summary>
        public virtual string IncorrectMessage { get; set; }

        /// <summary>
        /// Gets or sets the instruction.
        /// </summary>
        public virtual string Instruction { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is active.
        /// </summary>
        public virtual bool? IsActive { get; set; }

        /// <summary>
        /// Gets or sets the question type description.
        /// </summary>
        public virtual string QuestionTypeDescription { get; set; }

        /// <summary>
        /// Gets or sets the question type order.
        /// </summary>
        public virtual int? QuestionTypeOrder { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        public virtual string Type { get; set; }

        #endregion
    }
}