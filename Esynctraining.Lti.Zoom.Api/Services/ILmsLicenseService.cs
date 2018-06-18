using System.Threading.Tasks;
using Esynctraining.Lti.Zoom.Api.Dto;

namespace Esynctraining.Lti.Zoom.Api.Services
{
    public interface ILmsLicenseService
    {
        Task<LmsLicenseDto> GetLicense(int licenseId);

        Task<LmsLicenseDto> GetLicense(string consumerKey);

    }

}