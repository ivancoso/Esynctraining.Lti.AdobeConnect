using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Esynctraining.Lti.Zoom.Common
{
    public class NamedLocker
    {
        private readonly ConcurrentDictionary<Guid, SemaphoreSlim> _lockDict = new ConcurrentDictionary<Guid, SemaphoreSlim>();

        public async Task WaitAsync(Guid key)
        {
            await _lockDict.GetOrAdd(key, new SemaphoreSlim(1, 1)).WaitAsync();
        }

        public void Release(Guid key)
        {
            SemaphoreSlim semaphore;
            if (_lockDict.TryGetValue(key, out semaphore))
            {
                semaphore.Release();
            }
        }
    }
}
