using Esynctraining.Lti.Zoom.Domain;

namespace Esynctraining.Lti.Zoom.Api.Services
{
    public interface ILmsLicenseService
    {
        LmsLicense GetLicense(int licenseId);
        LmsLicense GetLicense(string cunsumerKey);
        //LmsLicense SaveLicense(L)
    }
}