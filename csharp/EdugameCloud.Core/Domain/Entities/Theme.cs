namespace EdugameCloud.Core.Domain.Entities
{
    using System;

    using Esynctraining.Core.Domain.Entities;

    /// <summary>
    ///     The theme
    /// </summary>
    public class Theme : Entity
    {
        #region Public Properties

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
        /// Gets or sets a value indicating whether is active.
        /// </summary>
        public virtual bool? IsActive { get; set; }

        /// <summary>
        /// Gets or sets the theme name.
        /// </summary>
        public virtual string ThemeName { get; set; }

        /// <summary>
        /// Gets or sets the modified by.
        /// </summary>
        public virtual User ModifiedBy { get; set; }

        #endregion
    }
}