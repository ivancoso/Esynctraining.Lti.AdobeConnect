using Esynctraining.Lti.Zoom.Api.Dto;

namespace Esynctraining.Lti.Zoom.Api.Services
{
    public interface ILmsLicenseService
    {
        LmsLicenseDto GetLicense(int licenseId);
        LmsLicenseDto GetLicense(string cunsumerKey);
    }
}