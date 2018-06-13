using System.Linq;
using Esynctraining.Lti.Zoom.Api.Dto;
using Esynctraining.Lti.Zoom.Domain;
using Microsoft.EntityFrameworkCore;

namespace Esynctraining.Lti.Zoom.Api.Services
{
    public class LmsLicenseDbService : ILmsLicenseService
    {
        private ZoomDbContext _dbContext;

        public LmsLicenseDbService(ZoomDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public LmsLicenseDto GetLicense(int licenseId)
        {
            var dbLicense =  _dbContext.LmsLicenses.Include(x => x.Settings).FirstOrDefault(x => x.Id == licenseId);
            return Convert(dbLicense);
        }

        public LmsLicenseDto GetLicense(string cunsumerKey)
        {
            var dbLicense = _dbContext.LmsLicenses.Include(x => x.Settings).FirstOrDefault(x => x.ConsumerKey == cunsumerKey);
            return Convert(dbLicense);
        }

        private LmsLicenseDto Convert(LmsLicense dbLicense)
        {
            return new LmsLicenseDto
            {
                Id = dbLicense.Id,
                Domain = dbLicense.Domain,
                ConsumerKey = dbLicense.ConsumerKey,
                LmsProviderId = dbLicense.LmsProviderId,
                SharedSecret = dbLicense.SharedSecret,
                Settings = dbLicense.Settings.ToDictionary(x => x.Name, x => (object)x.Value)
            };
        }
    }
}