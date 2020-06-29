using Esynctraining.Lti.Zoom.Common.Dto;
using Esynctraining.Lti.Zoom.Common.Services;
using System.Threading.Tasks;

namespace Esynctraining.Lti.Zoom.Common
{
    public class LicenseConstructorAccessor : ILmsLicenseAccessor
    {
        private LmsLicenseDto _license;

        public LicenseConstructorAccessor(LmsLicenseDto license)
        {
            _license = license;
        }

        public async Task<LmsLicenseDto> GetLicense()
        {
            return _license;
        }
    }
}
