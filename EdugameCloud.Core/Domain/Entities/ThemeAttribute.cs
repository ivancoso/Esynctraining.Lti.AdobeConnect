namespace EdugameCloud.Core.Domain.Entities
{
    using System;

    using Esynctraining.Core.Domain.Entities;

    /// <summary>
    ///     The theme attribute
    /// </summary>
    public class ThemeAttribute : Entity
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the bg color.
        /// </summary>
        public virtual string BgColor { get; set; }

        /// <summary>
        /// Gets or sets the category color.
        /// </summary>
        public virtual string CategoryColor { get; set; }

        /// <summary>
        /// Gets or sets the created by.
        /// </summary>
        public virtual User CreatedBy { get; set; }

        /// <summary>
        /// Gets or sets the date created.
        /// </summary>
        public virtual DateTime DateCreated { get; set; }

        /// <summary>
        /// Gets or sets the date modified.
        /// </summary>
        public virtual DateTime DateModified { get; set; }

        /// <summary>
        /// Gets or sets the distractor text color.
        /// </summary>
        public virtual string DistractorTextColor { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is active.
        /// </summary>
        public virtual bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets the modified by.
        /// </summary>
        public virtual User ModifiedBy { get; set; }

        /// <summary>
        /// Gets or sets the question hint color.
        /// </summary>
        public virtual string QuestionHintColor { get; set; }

        /// <summary>
        /// Gets or sets the question instruction color.
        /// </summary>
        public virtual string QuestionInstructionColor { get; set; }

        /// <summary>
        /// Gets or sets the question text color.
        /// </summary>
        public virtual string QuestionTextColor { get; set; }

        /// <summary>
        /// Gets or sets the response correct color.
        /// </summary>
        public virtual string ResponseCorrectColor { get; set; }

        /// <summary>
        /// Gets or sets the response incorrect color.
        /// </summary>
        public virtual string ResponseIncorrectColor { get; set; }

        /// <summary>
        /// Gets or sets the selection color.
        /// </summary>
        public virtual string SelectionColor { get; set; }

        /// <summary>
        /// Gets or sets the theme .
        /// </summary>
        public virtual Theme Theme { get; set; }

        /// <summary>
        /// Gets or sets the theme order.
        /// </summary>
        public virtual int ThemeOrder { get; set; }

        /// <summary>
        /// Gets or sets the title color.
        /// </summary>
        public virtual string TitleColor { get; set; }

        #endregion
    }
}