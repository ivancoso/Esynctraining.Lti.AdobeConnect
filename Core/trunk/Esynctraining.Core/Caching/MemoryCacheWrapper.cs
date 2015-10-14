using System;
using System.Runtime.Caching;

namespace Esynctraining.Core.Caching
{
    public class MemoryCacheWrapper : ICache
    {
        private readonly ObjectCache _cache = (ObjectCache)MemoryCache.Default;


        public void Add(string key, object value)
        {
            this._cache.Add(key, value, new CacheItemPolicy
            {
                SlidingExpiration = TimeSpan.FromHours(2.0),
            }, (string)null);
        }

        public object Get(string key)
        {
            return this._cache.Get(key, (string)null);
        }

        //public void Remove(string key)
        //{
        //    this._cache.Remove(key, (string)null);

        //}

    }

}
