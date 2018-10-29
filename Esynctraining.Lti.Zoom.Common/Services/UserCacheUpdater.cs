using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Esynctraining.Core.Json;
using Esynctraining.Core.Logging;
using Esynctraining.Lti.Lms.Common.Constants;
using Esynctraining.Zoom.ApiWrapper.Model;
using Microsoft.Extensions.Caching.Distributed;

namespace Esynctraining.Lti.Zoom.Common.Services
{
    public class UserCacheUpdater
    {
        private readonly ILmsLicenseService _lmsLicenseService;
        private readonly ZoomUserService _zoomUserService;
        private readonly IJsonSerializer _jsonSerializer;
        private readonly IDistributedCache _cache;
        private readonly ILogger _logger;

        public UserCacheUpdater(ILogger logger, ILmsLicenseService lmsLicenseService, ZoomUserService zoomUserService,
            IJsonSerializer jsonSerializer, IDistributedCache cache)
        {
            _lmsLicenseService = lmsLicenseService;
            _zoomUserService = zoomUserService;
            _jsonSerializer = jsonSerializer;
            _cache = cache;
            _logger = logger;
        }

        public async Task UpdateUsers(Guid licenseKey)
        {
            var sw = Stopwatch.StartNew();
            var license = await _lmsLicenseService.GetLicense(licenseKey);
            var cacheKey = "Zoom.Users." + license.GetSetting<string>(LmsLicenseSettingNames.ZoomApiKey);
            var users = _zoomUserService.GetUsersFromApi(UserStatus.Active);
            var json = _jsonSerializer.JsonSerialize(users);
            var cacheData = Encoding.UTF8.GetBytes(json);
            var cacheEntryOption = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(60) // todo: better approach
            };

            await _cache.SetAsync(cacheKey, cacheData, cacheEntryOption);
            sw.Stop();
            _logger.Info($"[UpdateCache:{licenseKey}] Time: {sw.Elapsed}, UsersCount:{users.Count}");
        }
    }
}