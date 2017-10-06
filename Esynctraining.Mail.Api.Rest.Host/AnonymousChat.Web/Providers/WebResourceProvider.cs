namespace AnonymousChat.Web.Providers
{
    using System;
    using System.Collections;
    using System.Reflection;

    using Esynctraining.Core.Extensions;
    using Esynctraining.Core.Providers;

    /// <summary>
    /// The connect extensions resource provider.
    /// </summary>
    public class WebResourceProvider : IResourceProvider
    {
        #region Fields

        /// <summary>
        /// The cache.
        /// </summary>
        private readonly Hashtable cache = new Hashtable();

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The clear cache.
        /// </summary>
        public void ClearCache()
        {
            lock (this.cache)
            {
                this.cache.Clear();
            }
        }

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
        public string GetResourceString(string key, string resourceName)
        {
            string cacheKey = string.Format("{0}.{1}", resourceName, key);
            lock (this.cache)
            {
                if (!this.cache.ContainsKey(cacheKey))
                {
                    this.cache.Add(cacheKey, this.GetFromType(resourceName, key));
                }
            }

            return this.cache[cacheKey].ToString();
        }

        #endregion

        #region Methods

        /// <summary>
        /// The get from type.
        /// </summary>
        /// <param name="resourceName">
        /// The resource name.
        /// </param>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string GetFromType(string resourceName, string key)
        {
            Type type = Type.GetType("Resources." + resourceName, false);
            if (type != null)
            {
                PropertyInfo pi = type.GetProperty(key, BindingFlags.NonPublic | BindingFlags.Static);
                if (pi != null)
                {
                    return pi.GetValue(null, null).Return(x => x.ToString(), key);
                }
            }

            return key;
        }

        #endregion
    }
}