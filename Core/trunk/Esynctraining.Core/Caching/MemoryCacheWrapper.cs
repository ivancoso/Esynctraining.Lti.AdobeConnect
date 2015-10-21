using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Caching;

namespace Esynctraining.Core.Caching
{
    public class MemoryCacheWrapper : ICache
    {
        private readonly ObjectCache _cache = MemoryCache.Default;


        public object Get(string key)
        {
            return this._cache.Get(key, (string)null);
        }

        public void Add(string key, object value)
        {
            this._cache.Add(key, value, new CacheItemPolicy
            {
                SlidingExpiration = TimeSpan.FromHours(2.0),
            }, (string)null);
        }

        public void Add(string key, object value, string dependencyPath)
        {
            var policy = new CacheItemPolicy
            {
                SlidingExpiration = TimeSpan.FromHours(2.0),
            };

            File.WriteAllText(dependencyPath, Guid.NewGuid().ToString());
            policy.ChangeMonitors.Add(new HostFileChangeMonitor(new List<string> { dependencyPath }));

            bool added = this._cache.Add(key, value, policy, (string)null);
        }
        
        //public void Remove(string key)
        //{
        //    this._cache.Remove(key, (string)null);

        //}

    }

}
