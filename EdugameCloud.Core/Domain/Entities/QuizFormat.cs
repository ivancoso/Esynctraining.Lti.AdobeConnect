namespace EdugameCloud.Core.Domain.Entities
{
    using System;

    using Esynctraining.Core.Domain.Entities;

    /// <summary>
    ///     The quiz format.
    /// </summary>
    public class QuizFormat : Entity
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the quiz format.
        /// </summary>
        public virtual string QuizFormatName { get; set; }

        /// <summary>
        ///     Gets or sets the date created.
        /// </summary>
        public virtual DateTime DateCreated { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether is active.
        /// </summary>
        public virtual bool? IsActive { get; set; }

        #endregion
    }
}