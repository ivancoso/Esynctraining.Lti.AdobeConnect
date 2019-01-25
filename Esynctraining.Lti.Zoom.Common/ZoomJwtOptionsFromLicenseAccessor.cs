using Esynctraining.Lti.Lms.Common.Constants;
using Esynctraining.Lti.Zoom.Common.Services;
using System;
using System.Threading.Tasks;
using Esynctraining.Zoom.ApiWrapper.JWT;

namespace Esynctraining.Lti.Zoom.Common
{
    public class ZoomJwtOptionsFromLicenseAccessor : IZoomApiJwtOptionsAccessor
    {
        private readonly ILmsLicenseAccessor _licenseAccessor;

        public ZoomJwtOptionsFromLicenseAccessor(ILmsLicenseAccessor licenseAccessor)
        {
            _licenseAccessor = licenseAccessor ?? throw new ArgumentNullException(nameof(licenseAccessor));
        }

        public async Task<ZoomApiJwtOptions> GetOptions()
        {
            var license = await _licenseAccessor.GetLicense();

            return new ZoomApiJwtOptions
            {
                ApiKey = license.GetSetting<string>(LmsLicenseSettingNames.ZoomApiKey),
                ApiSecret = license.GetSetting<string>(LmsLicenseSettingNames.ZoomApiSecret),
            };
        }
    }
}
