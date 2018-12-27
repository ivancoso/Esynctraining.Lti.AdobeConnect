using Esynctraining.Lti.Lms.Common.Constants;
using Esynctraining.Lti.Zoom.Common.Dto;
using Esynctraining.Lti.Zoom.Common.Services;
using Esynctraining.Lti.Zoom.Domain;
using Esynctraining.Zoom.ApiWrapper;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace Esynctraining.Lti.Zoom.Common
{
    public class ZoomOptionsFromLicenseAccessor : IZoomOptionsAccessor
    {
        private readonly IHttpContextAccessor _httpAccessor;
        private readonly ILtiTokenAccessor _tokenAccessor;
        private readonly UserSessionService _userService;
        private readonly ILmsLicenseService _lmsLicenseService;

        private LmsLicenseDto _license;

        public ZoomOptionsFromLicenseAccessor(IHttpContextAccessor httpAccessor, ILtiTokenAccessor tokenAccessor,
            ILmsLicenseService lmsLicenseService, UserSessionService userService)
        {
            _httpAccessor = httpAccessor;
            _tokenAccessor = tokenAccessor;
            _lmsLicenseService = lmsLicenseService;
            _userService = userService;
        }

        public async Task<ZoomApiOptions> GetOptions()
        {
            if (_license == null)
                _license = await RetrieveLicense();

            return new ZoomApiOptions
            {
                ZoomApiKey = _license.GetSetting<string>(LmsLicenseSettingNames.ZoomApiKey),
                ZoomApiSecret = _license.GetSetting<string>(LmsLicenseSettingNames.ZoomApiSecret),
            };
        }

        private async Task<LmsLicenseDto> RetrieveLicense()
        {
            Guid token = _tokenAccessor.FetchToken(_httpAccessor.HttpContext.Request, out string mode);

            LmsUserSession session = await _userService.GetSession(token);
            if (session == null)
            {
                throw new InvalidOperationException("Could not retrieve user session information");
            }

            var lmsLicense = await _lmsLicenseService.GetLicense(session.LicenseKey);

            return lmsLicense;
        }
    }
}
