using System;
using System.Threading.Tasks;
using Esynctraining.Lti.Lms.Common.Constants;
using Esynctraining.Lti.Zoom.Common.Services;
using Esynctraining.Zoom.ApiWrapper.OAuth;

namespace Esynctraining.Lti.Zoom.Common
{
    public class ZoomOAuthOptionsFromLicenseAccessor : IZoomOAuthOptionsAccessor
    {
        private readonly ILmsLicenseAccessor _licenseAccessor;
        private readonly ZoomOAuthConfig _zoomOAuthConfig;

        public ZoomOAuthOptionsFromLicenseAccessor(ILmsLicenseAccessor licenseAccessor, ZoomOAuthConfig zoomOAuthConfig)
        {
            _licenseAccessor = licenseAccessor ?? throw new ArgumentNullException(nameof(licenseAccessor));
        }

        public async Task<ZoomOAuthOptions> GetOptions()
        {
            var license = await _licenseAccessor.GetLicense();

            //todo: check & refresh token if necessary
            return new ZoomOAuthOptions
            {
                AccessToken = license.GetSetting<string>(LmsLicenseSettingNames.ZoomApiAccessToken),
            };
        }
    }
}