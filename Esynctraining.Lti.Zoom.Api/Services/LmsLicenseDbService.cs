using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Esynctraining.Core.Domain;
using Esynctraining.Core.Json;
using Esynctraining.Core.Providers;
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

        public async Task<LmsLicenseDto> GetLicense(Guid consumerKey)
        {
            var dbLicense = await _dbContext.LmsLicenses
                .Include(x => x.Settings)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.ConsumerKey == consumerKey.ToString());
            return Convert(dbLicense);
        }


        private static LmsLicenseDto Convert(LmsLicense dbLicense)
        {
            return new LmsLicenseDto
            {
                Id = dbLicense.Id,
                Domain = dbLicense.Domain,
                ConsumerKey = Guid.Parse(dbLicense.ConsumerKey),
                LmsProviderId = dbLicense.LmsProviderId,
                SharedSecret = Guid.Parse(dbLicense.SharedSecret),
                Settings = dbLicense.Settings.ToDictionary(x => x.Name, x => x.Value)
            };
        }

    }

    public class LmsLicenseInternalApiService : ILmsLicenseService
    {
        System.Net.Http.HttpClient _httpClient = new System.Net.Http.HttpClient();
        private readonly IJsonDeserializer _jsonDeserializer;
        private readonly dynamic _settings;

        public LmsLicenseInternalApiService(IJsonDeserializer jsonDeserializer, ApplicationSettingsProvider settings)
        {
            _jsonDeserializer = jsonDeserializer;
            _settings = settings;
        }

        public async Task<LmsLicenseDto> GetLicense(int licenseId)
        {
            throw new NotImplementedException();
        }

        public async Task<LmsLicenseDto> GetLicense(Guid consumerKey)
        {
            var httpResponseMessage = await _httpClient.GetAsync(
                $"{_settings.LicenseServiceUrl}/{consumerKey}");
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                var resp = await httpResponseMessage.Content.ReadAsStringAsync();
                OperationResultWithData<LmsLicenseDto> licenseDto = _jsonDeserializer.JsonDeserialize<OperationResultWithData<LmsLicenseDto>>(resp);
                return licenseDto.Data;
            }

            return null;
        }
    }

}