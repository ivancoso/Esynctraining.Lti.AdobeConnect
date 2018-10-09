using System;
using System.Threading.Tasks;
using Esynctraining.Lti.Zoom.Common.Dto;

namespace Esynctraining.Lti.Zoom.Common.Services
{
    public interface ILmsLicenseService
    {
        Task<LmsLicenseDto> GetLicense(int licenseId);

        Task<LmsLicenseDto> GetLicense(Guid consumerKey);

    }

}