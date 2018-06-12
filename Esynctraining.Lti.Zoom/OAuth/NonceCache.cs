using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Esynctraining.Core.Logging;

namespace Esynctraining.Lti.Zoom.OAuth
{
    // Source based on: https://msdn.microsoft.com/en-us/library/system.threading.readerwriterlockslim.aspx
    internal sealed class NonceCache
    {
        private const int CleanPeriodMinutes = 15;

        private DateTime nextCleanTime;
        private readonly ILogger logger;
        private readonly ReaderWriterLockSlim cacheLock = new ReaderWriterLockSlim();
        private readonly Dictionary<string, NonceData> innerCache = new Dictionary<string, NonceData>();


        public NonceCache(ILogger logger)
        {
            if (logger == null)
                throw new ArgumentNullException("logger");

            nextCleanTime = DateTime.UtcNow.AddMinutes(CleanPeriodMinutes);
            this.logger = logger;
        }


        public AddOrUpdateStatus AddIfNotExist(string key, Func<NonceData> valueBuilder)
        {
            cacheLock.EnterUpgradeableReadLock();
            try
            {
                if (innerCache.ContainsKey(key))
                {
                    return AddOrUpdateStatus.Exists;
                }
                else
                {
                    cacheLock.EnterWriteLock();
                    try
                    {
                        innerCache.Add(key, valueBuilder());
                    }
                    finally
                    {
                        cacheLock.ExitWriteLock();
                    }
                    return AddOrUpdateStatus.Added;
                }
            }
            finally
            {
                cacheLock.ExitUpgradeableReadLock();
            }
        }

        public void TryDeleteOld()
        {
            DateTime utcNow = DateTime.UtcNow;
            if (utcNow <= nextCleanTime)
            {
                return;
            }

            cacheLock.EnterWriteLock();
            try
            {
                var itemsToRemove = new List<string>();
                foreach (var pair in innerCache)
                {
                    if (utcNow.Subtract(pair.Value.Timestamp).TotalMinutes > 90)
                        itemsToRemove.Add(pair.Key);
                }
                foreach (string nonce in itemsToRemove)
                {
                    innerCache.Remove(nonce);
                }
                nextCleanTime = DateTime.UtcNow.AddMinutes(CleanPeriodMinutes);
                logger.InfoFormat("BLTI nonce clean up. Cleaned {0} items. Next clean-up time is {1} (UTC time).", itemsToRemove.Count, nextCleanTime.ToString());
            }
            finally
            {
                cacheLock.ExitWriteLock();
            }
        }

        public enum AddOrUpdateStatus
        {
            Exists,
            Added,
        };


        ~NonceCache()
        {
            if (cacheLock != null)
                cacheLock.Dispose();
        }

    }
}
