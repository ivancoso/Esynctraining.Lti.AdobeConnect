using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Esynctraining.Core.Logging;
using Esynctraining.Core.Providers;
using Esynctraining.Lti.Lms.Common.Constants;
using Esynctraining.Lti.Zoom.Common.Dto;
using Esynctraining.Lti.Zoom.Common.Services;
using Esynctraining.Zoom.ApiWrapper;
using Esynctraining.Zoom.ApiWrapper.JWT;

namespace Esynctraining.Lti.Zoom.Common.HostedServices
{
    public class UserCacheHostedService : TimedHostedService
    {
        private readonly UserCacheUpdater _cacheUpdater;
        private readonly ILmsLicenseService _lmsLicenseService;

        public UserCacheHostedService(ILogger logger, UserCacheUpdater cacheUpdater, ApplicationSettingsProvider settings, ILmsLicenseService lmsLicenseService) : base(logger, settings)
        {
            _cacheUpdater = cacheUpdater;
            _lmsLicenseService = lmsLicenseService;
        }

        protected override async void DoWork(object state)
        {
            var sw = Stopwatch.StartNew();
            var licenses = new List<LmsLicenseDto>();
            foreach (var key in new HashSet<Guid>(StaticStorage.RequestedLicenses))
            {
                licenses.Add(await _lmsLicenseService.GetLicense(key));
            }

            var groupedByZoomKey = licenses.GroupBy(x => x.GetSetting<string>(LmsLicenseSettingNames.ZoomApiKey));
            foreach (var licenseSet in groupedByZoomKey)
            {
                var license = licenseSet.First();
                var optionsAccessor = new ZoomApiJwtOptionsConstructorAccessor(new ZoomApiJwtOptions
                {
                    ApiKey = license.GetSetting<string>(LmsLicenseSettingNames.ZoomApiKey),
                    ApiSecret = license.GetSetting<string>(LmsLicenseSettingNames.ZoomApiSecret)
                });

                var authParamsAccessor = new ZoomJwtAuthParamsAccessor(optionsAccessor);
                var zoomApi = new ZoomApiWrapper(authParamsAccessor);
                await _cacheUpdater.UpdateUsers(licenseSet.Key, zoomApi);
            }

            sw.Stop();
            Logger.Info($"[UpdateCache] Time: {sw.Elapsed}.");
        }
    }
}