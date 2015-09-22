namespace EdugameCloud.Core.Domain.Entities
{
    using System;

    using Esynctraining.Core.Domain.Entities;

    /// <summary>
    ///     The score type.
    /// </summary>
    public class ScoreType : Entity
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the date created.
        /// </summary>
        public virtual DateTime DateCreated { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is active.
        /// </summary>
        public virtual bool? IsActive { get; set; }

        /// <summary>
        /// Gets or sets the score type name.
        /// </summary>
        public virtual string ScoreTypeName { get; set; }

        /// <summary>
        /// Gets or sets the prefix.
        /// </summary>
        public virtual string Prefix { get; set; }

        /// <summary>
        /// Gets or sets the default value.
        /// </summary>
        public virtual int DefaultValue { get; set; }

        #endregion
    }
}