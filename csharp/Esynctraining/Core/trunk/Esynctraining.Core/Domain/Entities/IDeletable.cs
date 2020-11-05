namespace Esynctraining.Core.Domain.Entities
{
    /// <summary>
    /// The delete able interface.
    /// </summary>
    public interface IDeletable
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets a value indicating whether is deleted.
        /// </summary>
        bool IsDeleted { get; set; }

        #endregion
    }
}