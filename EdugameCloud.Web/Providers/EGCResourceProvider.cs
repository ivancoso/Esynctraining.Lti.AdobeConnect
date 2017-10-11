namespace EdugameCloud.Web.Providers
{
    using System;
    using System.Collections;
    using System.Reflection;

    using Esynctraining.Core.Extensions;
    using Esynctraining.Core.Providers;

    public class EGCResourceProvider : IResourceProvider
    {
        private readonly Hashtable _cache = new Hashtable();


        public void ClearCache()
        {
            lock (_cache)
            {
                _cache.Clear();
            }
        }

        public string GetResourceString(string key, string resourceName)
        {
            string cacheKey = string.Format("{0}.{1}", resourceName, key);
            lock (_cache)
            {
                if (!_cache.ContainsKey(cacheKey))
                {
                    _cache.Add(cacheKey, GetFromType(resourceName, key));
                }
            }

            return _cache[cacheKey].ToString();
        }


        private static string GetFromType(string resourceName, string key)
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

    }

}