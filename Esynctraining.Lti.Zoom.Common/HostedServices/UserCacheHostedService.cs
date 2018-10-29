using System.Diagnostics;
using System.Linq;
using Esynctraining.Core.Logging;
using Esynctraining.Core.Providers;
using Esynctraining.Lti.Zoom.Common.Services;

namespace Esynctraining.Lti.Zoom.Common.HostedServices
{
    public class UserCacheHostedService : TimedHostedService
    {
        private readonly UserCacheUpdater _cacheUpdater;

        public UserCacheHostedService(ILogger logger, UserCacheUpdater cacheUpdater, ApplicationSettingsProvider settings) : base(logger, settings)
        {
            _cacheUpdater = cacheUpdater;
        }

        protected override async void DoWork(object state)
        {
            var sw = Stopwatch.StartNew();
            foreach (var licenseKey in StaticStorage.RequestedLicenses.ToList())
            {
                await _cacheUpdater.UpdateUsers(licenseKey);
            }
            sw.Stop();
            Logger.Info($"[UpdateCache] Time: {sw.Elapsed}.");
        }
    }
}