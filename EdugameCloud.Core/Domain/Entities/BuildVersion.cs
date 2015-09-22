namespace EdugameCloud.Core.Domain.Entities
{
    using System;

    using Esynctraining.Core.Domain.Entities;

    /// <summary>
    ///     The build version.
    /// </summary>
    public class BuildVersion : Entity
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the build number.
        /// </summary>
        public virtual string BuildNumber { get; set; }

        /// <summary>
        /// Gets or sets the date created.
        /// </summary>
        public virtual DateTime DateCreated { get; set; }

        /// <summary>
        /// Gets or sets the date modified.
        /// </summary>
        public virtual DateTime DateModified { get; set; }

        /// <summary>
        /// Gets or sets the description html.
        /// </summary>
        public virtual string DescriptionHTML { get; set; }

        /// <summary>
        /// Gets or sets the description small.
        /// </summary>
        public virtual string DescriptionSmall { get; set; }

        /// <summary>
        /// Gets or sets the file path.
        /// </summary>
        public virtual File File { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is active.
        /// </summary>
        public virtual bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        public virtual BuildVersionType Type { get; set; }

        #endregion
    }
}