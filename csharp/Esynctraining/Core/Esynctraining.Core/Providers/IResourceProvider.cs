namespace Esynctraining.Core.Providers
{
    /// <summary>
    /// The ResourceProvider interface.
    /// </summary>
    public interface IResourceProvider
    {
        /// <summary>
        /// The get resource string.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="resourceName">
        /// The resource name.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        string GetResourceString(string key, string resourceName);

        /// <summary>
        /// The clear cache.
        /// </summary>
        void ClearCache();
    }
}
