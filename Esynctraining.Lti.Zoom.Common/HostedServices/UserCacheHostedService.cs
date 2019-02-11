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
using Esynctraining.Zoom.ApiWrapper.OAuth;

namespace Esynctraining.Lti.Zoom.Common.HostedServices
{
    public class UserCacheHostedService : TimedHostedService
    {
        private readonly UserCacheUpdater _cacheUpdater;
        private readonly ILmsLicenseService _lmsLicenseService;
        private readonly ZoomOAuthConfig _zoomOAuthConfig;

        public UserCacheHostedService(ILogger logger, UserCacheUpdater cacheUpdater, ApplicationSettingsProvider settings, ILmsLicenseService lmsLicenseService, ZoomOAuthConfig zoomOAuthConfig) : base(logger, settings)
        {
            _cacheUpdater = cacheUpdater ?? throw new ArgumentNullException(nameof(cacheUpdater));
            _lmsLicenseService = lmsLicenseService ?? throw new ArgumentNullException(nameof(lmsLicenseService));
            _zoomOAuthConfig = zoomOAuthConfig ?? throw new ArgumentNullException(nameof(zoomOAuthConfig)); ;
        }

        protected override async void DoWork(object state)
        {
            var sw = Stopwatch.StartNew();
            var licenses = new List<LmsLicenseDto>();
            foreach (var key in new HashSet<Guid>(StaticStorage.RequestedLicenses))
            {
                licenses.Add(await _lmsLicenseService.GetLicense(key));
            }

            var groupedByZoomKey = licenses.GroupBy(x => x.ZoomUserDto.UserId);
            foreach (var licenseSet in groupedByZoomKey)
            {
                var license = licenseSet.First();
                //var optionsAccessor = new ZoomApiJwtOptionsConstructorAccessor(new ZoomApiJwtOptions
                //{
                //    ApiKey = license.GetSetting<string>(LmsLicenseSettingNames.ZoomApiKey),
                //    ApiSecret = license.GetSetting<string>(LmsLicenseSettingNames.ZoomApiSecret)
                //});

                //var authParamsAccessor = new ZoomJwtAuthParamsAccessor(optionsAccessor);
                //var zoomApi = new ZoomApiWrapper(authParamsAccessor);

                ILmsLicenseAccessor lmsLicenseAccessor = new LicenseConstructorAccessor(license);
                var optionsAccessor = new ZoomOAuthOptionsFromLicenseAccessor(lmsLicenseAccessor, _lmsLicenseService, Logger);


                //var optionsAccessor = new ZoomOAuthOptionsConstructorAccessor(new ZoomOAuthOptions
                //{
                //    AccessToken = license.GetSetting<string>(LmsLicenseSettingNames.ZoomApiAccessToken)
                //});

                var authParamsAccessor = new ZoomOAuthParamsAccessor(optionsAccessor);
                var zoomApi = new ZoomApiWrapper(authParamsAccessor);
                await _cacheUpdater.UpdateUsers(licenseSet.Key, zoomApi);
            }

            sw.Stop();
            Logger.Info($"[UpdateCache] Time: {sw.Elapsed}.");
        }
    }
}