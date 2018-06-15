using System;
using System.Threading.Tasks;
using Esynctraining.Lti.Lms.Common.Constants;
using Esynctraining.Lti.Zoom.Api.Dto;
using Esynctraining.Lti.Zoom.Api.Services;
using Esynctraining.Lti.Zoom.Domain;
using Esynctraining.Zoom.ApiWrapper;
using Microsoft.AspNetCore.Http;

namespace Esynctraining.Lti.Zoom.Api.Host.FIlters
{
    public class ZoomLicenseAccessor : IZoomOptionsAccessor, ILmsLicenseAccessor
    {
        private readonly IHttpContextAccessor _httpAccessor;
        private readonly ILtiTokenAccessor _tokenAccessor;
        private readonly UserSessionService _userService;
        private readonly ILmsLicenseService _lmsLicenseService;

        private LmsLicenseDto _license;

        public ZoomLicenseAccessor(IHttpContextAccessor httpAccessor, LtiTokenAccessor tokenAccessor,
            ILmsLicenseService lmsLicenseService, UserSessionService userService)
        {
            _httpAccessor = httpAccessor;
            _tokenAccessor = tokenAccessor;
            _lmsLicenseService = lmsLicenseService;
            _userService = userService;
        }

        public ZoomApiOptions Options => new ZoomApiOptions
        {
            ZoomApiKey = _license.GetSetting<string>(LmsLicenseSettingNames.ZoomApiKey),
            ZoomApiSecret = _license.GetSetting<string>(LmsLicenseSettingNames.ZoomApiSecret)
        };

        public async Task<LmsLicenseDto> GetLicense()
        {
            return _license ?? (_license = await RetrieveLicense());
        }

        private async Task<LmsLicenseDto> RetrieveLicense()
        {
            Guid token = _tokenAccessor.FetchToken(_httpAccessor.HttpContext.Request, out string mode);
           
            LmsUserSession session = await _userService.GetSession(token);
            if (session == null)
            {
                throw new InvalidOperationException("Could not retrieve user session information");
            }

            var lmsLicense = _lmsLicenseService.GetLicense(session.LicenseId);
            if (lmsLicense == null)
            {
                throw new InvalidOperationException("Could not retrieve user license information");
            }
            return lmsLicense;
        }
    }
}