using System.Linq;
using Esynctraining.Lti.Zoom.Domain;
using Microsoft.EntityFrameworkCore;

namespace Esynctraining.Lti.Zoom.Api.Services
{
    public class LmsLicenseService : ILmsLicenseService
    {
        private ZoomDbContext _dbContext;

        public LmsLicenseService(ZoomDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        public LmsLicense GetLicense(int licenseId)
        {
            return _dbContext.LmsLicenses.Include(x => x.Settings).FirstOrDefault(x => x.Id == licenseId);
        }

        public LmsLicense GetLicense(string cunsumerKey)
        {
            return _dbContext.LmsLicenses.Include(x => x.Settings).FirstOrDefault(x => x.ConsumerKey == cunsumerKey);
        }
    }
}