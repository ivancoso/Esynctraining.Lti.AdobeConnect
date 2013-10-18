namespace Esynctraining.Core.Wrappers
{
    using System;
    using System.Collections;
    using System.Web.Caching;

    /// <summary>
    /// The cache base.
    /// </summary>
    public abstract class CacheBase : IEnumerable
    {
        #region Public Properties

        /// <summary>
        /// Gets the count.
        /// </summary>
        /// <exception cref="NotImplementedException">
        /// Property is not implemented here
        /// </exception>
        public virtual int Count
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Gets the effective percentage physical memory limit.
        /// </summary>
        /// <exception cref="NotImplementedException">
        /// Property is not implemented here
        /// </exception>
        public virtual long EffectivePercentagePhysicalMemoryLimit
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Gets the effective private bytes limit.
        /// </summary>
        /// <exception cref="NotImplementedException">
        /// Property is not implemented here
        /// </exception>
        public virtual long EffectivePrivateBytesLimit
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        #endregion

        #region Public Indexers

        /// <summary>
        /// The indexer for cached objects.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <exception cref="NotImplementedException">
        /// Property is not implemented here
        /// </exception>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public virtual object this[string key]
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
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
        /// <exception cref="NotImplementedException">
        /// Method is not implemented here
        /// </exception>
        public virtual object Add(
            string key, 
            object value, 
            CacheDependency dependencies, 
            DateTime absoluteExpiration, 
            TimeSpan slidingExpiration, 
            CacheItemPriority priority, 
            CacheItemRemovedCallback onRemoveCallback)
        {
            throw new NotImplementedException();
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
        /// <exception cref="NotImplementedException">
        /// Method is not implemented here
        /// </exception>
        public virtual object Get(string key)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The get enumerator.
        /// </summary>
        /// <returns>
        /// The <see cref="IDictionaryEnumerator"/>.
        /// </returns>
        /// <exception cref="NotImplementedException">
        /// Method is not implemented here
        /// </exception>
        public virtual IDictionaryEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
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
        /// <exception cref="NotImplementedException">
        /// Method is not implemented here
        /// </exception>
        public virtual void Insert(string key, object value)
        {
            throw new NotImplementedException();
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
        /// <exception cref="NotImplementedException">
        /// Method is not implemented here
        /// </exception>
        public virtual void Insert(string key, object value, CacheDependency dependencies)
        {
            throw new NotImplementedException();
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
        /// <exception cref="NotImplementedException">
        /// Method is not implemented here
        /// </exception>
        public virtual void Insert(
            string key, 
            object value, 
            CacheDependency dependencies, 
            DateTime absoluteExpiration, 
            TimeSpan slidingExpiration)
        {
            throw new NotImplementedException();
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
        /// <exception cref="NotImplementedException">
        /// Method is not implemented here
        /// </exception>
        public virtual void Insert(
            string key, 
            object value, 
            CacheDependency dependencies, 
            DateTime absoluteExpiration, 
            TimeSpan slidingExpiration, 
            CacheItemUpdateCallback onUpdateCallback)
        {
            throw new NotImplementedException();
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
        /// <exception cref="NotImplementedException">
        /// Method is not implemented here
        /// </exception>
        public virtual void Insert(
            string key, 
            object value, 
            CacheDependency dependencies, 
            DateTime absoluteExpiration, 
            TimeSpan slidingExpiration, 
            CacheItemPriority priority, 
            CacheItemRemovedCallback onRemoveCallback)
        {
            throw new NotImplementedException();
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
        /// <exception cref="NotImplementedException">
        /// Method is not implemented here
        /// </exception>
        public virtual object Remove(string key)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Explicit Interface Methods

        /// <summary>
        /// The get enumerator.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerator"/>.
        /// </returns>
        /// <exception cref="NotImplementedException">
        /// Method is not implemented here
        /// </exception>
        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}