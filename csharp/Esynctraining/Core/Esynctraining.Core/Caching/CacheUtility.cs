using System;

namespace Esynctraining.Core.Caching
{
    public static class CacheUtility
    {
        public static T GetCachedItem<T>(ICache cache, string cacheKey, Func<T> dataFetcher) where T : class
        {
            return CacheUtility.GetCachedItem<T>(cache, cacheKey, CacheUtility.TypeLock<T>.SyncLock, dataFetcher);
        }

        public static T GetCachedItem<T>(ICache cache, string cacheKey, string dependencyPath, Func<T> dataFetcher) where T : class
        {
            return CacheUtility.GetCachedItem<T>(cache, cacheKey, dependencyPath, CacheUtility.TypeLock<T>.SyncLock, dataFetcher);
        }


        private static T GetCachedItem<T>(ICache cache, string cacheKey, object lockHandle, Func<T> dataFetcher) where T : class
        {
            object item = cache.Get(cacheKey);
            if (item == null)
            {
                lock (lockHandle)
                {
                    item = cache.Get(cacheKey);
                    if (item == null)
                    {
                        item = (object)dataFetcher();
                        if (item != null)
                            cache.Add(cacheKey, (object)(T)item);
                    }
                }
            }
            T typedItem = item as T;
            if (typedItem != null)
                return typedItem;

            throw new Exception(string.Format("Object retrieved from the cache is not of the expected {0} type. Another component might be using the same key to store in the cache.", (object)typeof(T).FullName));
        }

        private static T GetCachedItem<T>(ICache cache, string cacheKey, string dependencyPath, object lockHandle, Func<T> dataFetcher) where T : class
        {
            object item = cache.Get(cacheKey);
            if (item == null)
            {
                lock (lockHandle)
                {
                    item = cache.Get(cacheKey);
                    if (item == null)
                    {
                        item = (object)dataFetcher();
                        if (item != null)
                            cache.Add(cacheKey, (object)(T)item, dependencyPath);
                    }
                }
            }
            T typedItem = item as T;
            if (typedItem != null)
                return typedItem;

            throw new Exception(string.Format("Object retrieved from the cache is not of the expected {0} type. Another component might be using the same key to store in the cache.", (object)typeof(T).FullName));
        }

        //public static void Remove(ICache cache, string cacheKey)
        //{
        //    cache.Remove(cacheKey);
        //}

        public static class TypeLock<T>
        {
            public static readonly object SyncLock = new object();
        }

    }

}
