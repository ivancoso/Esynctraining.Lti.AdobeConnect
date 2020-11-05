namespace Esynctraining.Core.Domain.Entities
{
    /// <summary>
    /// The Entity interface.
    /// </summary>
    /// <typeparam name="T">
    /// </typeparam>
    public interface IEntity<T>
        where T : struct
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        T Id { get; set; }

        #endregion
    }
}