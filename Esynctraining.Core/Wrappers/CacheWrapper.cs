namespace Esynctraining.Core.Wrappers
{
    using System;
    using System.Collections;
    using System.Web.Caching;

    /// <summary>
    /// The cache wrapper.
    /// </summary>
    public class CacheWrapper : CacheBase
    {
        #region Fields

        /// <summary>
        /// The cache.
        /// </summary>
        private readonly Cache cache;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheWrapper"/> class.
        /// </summary>
        /// <param name="cache">
        /// The cache.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Argument exception when cache is empty
        /// </exception>
        public CacheWrapper(Cache cache)
        {
            if (cache == null)
            {
                throw new ArgumentNullException("cache");
            }

            this.cache = cache;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the count.
        /// </summary>
        public override int Count
        {
            get
            {
                return this.cache.Count;
            }
        }

        /// <summary>
        /// Gets the effective percentage physical memory limit.
        /// </summary>
        public override long EffectivePercentagePhysicalMemoryLimit
        {
            get
            {
                return this.cache.EffectivePercentagePhysicalMemoryLimit;
            }
        }

        /// <summary>
        /// Gets the effective private bytes limit.
        /// </summary>
        public override long EffectivePrivateBytesLimit
        {
            get
            {
                return this.cache.EffectivePrivateBytesLimit;
            }
        }

        #endregion

        #region Public Indexers

        /// <summary>
        /// The this.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public override object this[string key]
        {
            get
            {
                return this.cache[key];
            }

            set
            {
                this.cache[key] = value;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The add.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="dependencies">
        /// The dependencies.
        /// </param>
        /// <param name="absoluteExpiration">
        /// The absolute expiration.
        /// </param>
        /// <param name="slidingExpiration">
        /// The sliding expiration.
        /// </param>
        /// <param name="priority">
        /// The priority.
        /// </param>
        /// <param name="onRemoveCallback">
        /// The on remove callback.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public override object Add(
            string key, 
            object value, 
            CacheDependency dependencies, 
            DateTime absoluteExpiration, 
            TimeSpan slidingExpiration, 
            CacheItemPriority priority, 
            CacheItemRemovedCallback onRemoveCallback)
        {
            return this.cache.Add(
                key, value, dependencies, absoluteExpiration, slidingExpiration, priority, onRemoveCallback);
        }

        /// <summary>
        /// The get.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public override object Get(string key)
        {
            return this.cache.Get(key);
        }

        /// <summary>
        /// The get enumerator.
        /// </summary>
        /// <returns>
        /// The <see cref="IDictionaryEnumerator"/>.
        /// </returns>
        public override IDictionaryEnumerator GetEnumerator()
        {
            return this.cache.GetEnumerator();
        }

        /// <summary>
        /// The insert.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        public override void Insert(string key, object value)
        {
            this.cache.Insert(key, value);
        }

        /// <summary>
        /// The insert.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="dependencies">
        /// The dependencies.
        /// </param>
        public override void Insert(string key, object value, CacheDependency dependencies)
        {
            this.cache.Insert(key, value, dependencies);
        }

        /// <summary>
        /// The insert.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="dependencies">
        /// The dependencies.
        /// </param>
        /// <param name="absoluteExpiration">
        /// The absolute expiration.
        /// </param>
        /// <param name="slidingExpiration">
        /// The sliding expiration.
        /// </param>
        public override void Insert(
            string key, 
            object value, 
            CacheDependency dependencies, 
            DateTime absoluteExpiration, 
            TimeSpan slidingExpiration)
        {
            this.cache.Insert(key, value, dependencies, absoluteExpiration, slidingExpiration);
        }

        /// <summary>
        /// The insert.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="dependencies">
        /// The dependencies.
        /// </param>
        /// <param name="absoluteExpiration">
        /// The absolute expiration.
        /// </param>
        /// <param name="slidingExpiration">
        /// The sliding expiration.
        /// </param>
        /// <param name="onUpdateCallback">
        /// The on update callback.
        /// </param>
        public override void Insert(
            string key, 
            object value, 
            CacheDependency dependencies, 
            DateTime absoluteExpiration, 
            TimeSpan slidingExpiration, 
            CacheItemUpdateCallback onUpdateCallback)
        {
            this.cache.Insert(key, value, dependencies, absoluteExpiration, slidingExpiration, onUpdateCallback);
        }

        /// <summary>
        /// The insert.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="dependencies">
        /// The dependencies.
        /// </param>
        /// <param name="absoluteExpiration">
        /// The absolute expiration.
        /// </param>
        /// <param name="slidingExpiration">
        /// The sliding expiration.
        /// </param>
        /// <param name="priority">
        /// The priority.
        /// </param>
        /// <param name="onRemoveCallback">
        /// The on remove callback.
        /// </param>
        public override void Insert(
            string key, 
            object value, 
            CacheDependency dependencies, 
            DateTime absoluteExpiration, 
            TimeSpan slidingExpiration, 
            CacheItemPriority priority, 
            CacheItemRemovedCallback onRemoveCallback)
        {
            this.cache.Insert(
                key, value, dependencies, absoluteExpiration, slidingExpiration, priority, onRemoveCallback);
        }

        /// <summary>
        /// The remove.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public override object Remove(string key)
        {
            return this.cache.Remove(key);
        }

        #endregion
    }
}