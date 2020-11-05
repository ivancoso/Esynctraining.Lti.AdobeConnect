namespace Esynctraining.Core.Utils
{
    /// <summary>
    /// The LocalData interface.
    /// </summary>
    public interface ILocalData
    {
        /// <summary>
        /// The object indexer.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        object this[object key]
        {
            get;
            set;
        }

        /// <summary>
        /// The clear.
        /// </summary>
        void Clear();
    }
}
