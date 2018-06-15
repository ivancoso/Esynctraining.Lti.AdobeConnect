using System.Threading.Tasks;
using Esynctraining.Lti.Zoom.Api.Dto;

namespace Esynctraining.Lti.Zoom.Api.Services
{
    public interface ILmsLicenseAccessor
    {
        Task<LmsLicenseDto> GetLicense();
    }
}