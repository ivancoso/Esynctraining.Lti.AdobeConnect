namespace EdugameCloud.Core.Extensions
{
    using System;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Web;

    /// <summary>
    /// Name value collection extensions
    /// </summary>
    public static class NameValueCollectionExtensions
    {
        #region Public Methods and Operators

        /// <summary>
        /// The has key.
        /// </summary>
        /// <param name="nvc">
        /// Name value collection
        /// </param>
        /// <param name="key">
        /// Key value
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool HasKey(this NameValueCollection nvc, string key)
        {
            if (nvc != null && nvc.HasKeys())
            {
                foreach (var keyVar in nvc.Keys)
                {
                    if (keyVar is string && keyVar.ToString().Equals(key, StringComparison.InvariantCultureIgnoreCase))
                    {
                        return true;
                    }
                }

                return false;
            }

            return false;
        }

        /// <summary>
        /// The has key.
        /// </summary>
        /// <param name="nvc">
        /// The name value collection.
        /// </param>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool HasKey(this HttpCookieCollection nvc, string key)
        {
            if (nvc != null && nvc.AllKeys.Length > 0)
            {
                return nvc.AllKeys.Any(keyVar => keyVar.Equals(key, StringComparison.InvariantCultureIgnoreCase));
            }

            return false;
        }

        #endregion
    }
}