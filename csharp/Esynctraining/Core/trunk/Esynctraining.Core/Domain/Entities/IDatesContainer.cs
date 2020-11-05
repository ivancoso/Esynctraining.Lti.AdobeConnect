namespace Esynctraining.Core.Domain.Entities
{
    using System;

    /// <summary>
    /// The DatesContainer interface.
    /// </summary>
    public interface IDatesContainer
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the date created.
        /// </summary>
        DateTime DateCreated { get; set; }

        /// <summary>
        /// Gets or sets the date modified.
        /// </summary>
        DateTime? DateModified { get; set; }

        #endregion
    }
}
