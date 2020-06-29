namespace EdugameCloud.Core.Domain.Entities
{
    using Esynctraining.Core.Domain.Entities;

    /// <summary>
    ///     The sub module item theme.
    /// </summary>
    public class SubModuleItemTheme : Entity
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the background color.
        /// </summary>
        public virtual string BackgroundColor { get; set; }

        /// <summary>
        /// Gets or sets the background image.
        /// </summary>
        public virtual File BackgroundImage { get; set; }

        /// <summary>
        /// Gets or sets the correct color.
        /// </summary>
        public virtual string CorrectColor { get; set; }

        /// <summary>
        /// Gets or sets the hint color.
        /// </summary>
        public virtual string HintColor { get; set; }

        /// <summary>
        /// Gets or sets the incorrect color.
        /// </summary>
        public virtual string IncorrectColor { get; set; }

        /// <summary>
        /// Gets or sets the instruction color.
        /// </summary>
        public virtual string InstructionColor { get; set; }

        /// <summary>
        /// Gets or sets the question color.
        /// </summary>
        public virtual string QuestionColor { get; set; }

        /// <summary>
        /// Gets or sets the selection color.
        /// </summary>
        public virtual string SelectionColor { get; set; }

        /// <summary>
        /// Gets or sets the sub module item.
        /// </summary>
        public virtual SubModuleItem SubModuleItem { get; set; }

        /// <summary>
        /// Gets or sets the title color.
        /// </summary>
        public virtual string TitleColor { get; set; }

        #endregion
    }
}