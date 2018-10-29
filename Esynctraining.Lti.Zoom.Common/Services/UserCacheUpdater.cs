using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Esynctraining.Core.Json;
using Esynctraining.Core.Logging;
using Esynctraining.Lti.Lms.Common.Constants;
using Esynctraining.Zoom.ApiWrapper;
using Esynctraining.Zoom.ApiWrapper.Model;
using Microsoft.Extensions.Caching.Distributed;
using Esynctraining.Lti.Zoom.Common.Dto;

namespace Esynctraining.Lti.Zoom.Common.Services
{
    public class UserCacheUpdater
    {
        private readonly ILmsLicenseService _lmsLicenseService;
        private readonly IJsonSerializer _jsonSerializer;
        private readonly IDistributedCache _cache;
        private readonly ILogger _logger;

        public UserCacheUpdater(ILogger logger, ILmsLicenseService lmsLicenseService,
            IJsonSerializer jsonSerializer, IDistributedCache cache)
        {
            _lmsLicenseService = lmsLicenseService;
            _jsonSerializer = jsonSerializer;
            _cache = cache;
            _logger = logger;
        }

        public async Task UpdateUsers(Guid licenseKey)
        {
            var sw = Stopwatch.StartNew();
            var license = await _lmsLicenseService.GetLicense(licenseKey);
            var cacheKey = "Zoom.Users." + license.GetSetting<string>(LmsLicenseSettingNames.ZoomApiKey);
            var users = GetUsersFromApi(UserStatus.Active, license);
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


        public List<User> GetUsersFromApi(UserStatus status, LmsLicenseDto license)
        {
            var users = new List<User>();
            var pageNumber = 1;
            var pageSize = 300;
            var totalRecords = 0;
            do
            {
                var _zoomApi = new ZoomApiWrapper(new ZoomApiOptions{ZoomApiKey = license.GetSetting<string>(LmsLicenseSettingNames.ZoomApiKey), ZoomApiSecret = license.GetSetting<string>(LmsLicenseSettingNames.ZoomApiSecret) });
                var page = _zoomApi.GetUsers(status, pageSize: pageSize, pageNumber: pageNumber);
                users.AddRange(page.Users);
                totalRecords = page.TotalRecords;
                pageNumber++;

            } while (pageSize * (pageNumber - 1) < totalRecords);

            return users;
        }
    }
}