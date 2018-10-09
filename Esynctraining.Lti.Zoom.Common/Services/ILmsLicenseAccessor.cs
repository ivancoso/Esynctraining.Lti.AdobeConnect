using System.Threading.Tasks;
using Esynctraining.Lti.Zoom.Common.Dto;

namespace Esynctraining.Lti.Zoom.Common.Services
{
    public interface ILmsLicenseAccessor
    {
        Task<LmsLicenseDto> GetLicense();
    }
}