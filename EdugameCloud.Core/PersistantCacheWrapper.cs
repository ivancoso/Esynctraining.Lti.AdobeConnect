using System;
using System.Runtime.Caching;
using Esynctraining.Core.Caching;

namespace EdugameCloud.Core
{
    public class PersistantCacheWrapper : ICache
    {
        private readonly ObjectCache _cache = MemoryCache.Default;

        public void Add(string key, object value)
        {
            _cache.Add(key, value, DateTimeOffset.MaxValue);
        }

        public void Add(string key, object value, string dependencyPath)
        {
            throw new NotImplementedException();
        }

        public object Get(string key)
        {
            return _cache.Get(key);
        }
    }
}