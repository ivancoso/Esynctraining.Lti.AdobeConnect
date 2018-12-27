using Esynctraining.Lti.Zoom.Common.Dto;
using Esynctraining.Lti.Zoom.Common.Services;
using Esynctraining.Lti.Zoom.Domain;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace Esynctraining.Lti.Zoom.Common
{
    public class LicenseAccessor : ILmsLicenseAccessor
    {
        private readonly IHttpContextAccessor _httpAccessor;
        private readonly ILtiTokenAccessor _tokenAccessor;
        private readonly UserSessionService _userService;
        private readonly ILmsLicenseService _lmsLicenseService;

        private LmsLicenseDto _license;

        public LicenseAccessor(IHttpContextAccessor httpAccessor, ILtiTokenAccessor tokenAccessor,
            ILmsLicenseService lmsLicenseService, UserSessionService userService)
        {
            _httpAccessor = httpAccessor;
            _tokenAccessor = tokenAccessor;
            _lmsLicenseService = lmsLicenseService;
            _userService = userService;
        }

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

            var lmsLicense = await _lmsLicenseService.GetLicense(session.LicenseKey);

            return lmsLicense;
        }
    }
}
