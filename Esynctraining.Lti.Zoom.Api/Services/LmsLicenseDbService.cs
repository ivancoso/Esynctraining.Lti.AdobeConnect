using System;
using System.Linq;
using System.Threading.Tasks;
using Esynctraining.Lti.Zoom.Api.Dto;
using Esynctraining.Lti.Zoom.Domain;
using Microsoft.EntityFrameworkCore;

namespace Esynctraining.Lti.Zoom.Api.Services
{
    public class LmsLicenseDbService : ILmsLicenseService
    {
        private readonly ZoomDbContext _dbContext;


        public LmsLicenseDbService(ZoomDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }


        public async Task <LmsLicenseDto> GetLicense(int licenseId)
        {
            var dbLicense = await _dbContext.LmsLicenses
                .Include(x => x.Settings)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == licenseId);
            return Convert(dbLicense);
        }

        public async Task<LmsLicenseDto> GetLicense(string consumerKey)
        {
            var dbLicense = await _dbContext.LmsLicenses
                .Include(x => x.Settings)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.ConsumerKey == consumerKey);
            return Convert(dbLicense);
        }


        private static LmsLicenseDto Convert(LmsLicense dbLicense)
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