using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Esynctraining.Core.Domain;
using Esynctraining.Core.Json;
using Esynctraining.Core.Logging;
using Esynctraining.Core.Providers;
using Esynctraining.Lti.Zoom.Api;
using Esynctraining.Lti.Zoom.Common.Dto;

namespace Esynctraining.Lti.Zoom.Common.Services
{
    //public class LmsLicenseDbService : ILmsLicenseService
    //{
    //    private readonly ZoomDbContext _dbContext;

    //    public LmsLicenseDbService(ZoomDbContext dbContext)
    //    {
    //        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    //    }

    //    public async Task <LmsLicenseDto> GetLicense(int licenseId)
    //    {
    //        var dbLicense = await _dbContext.LmsLicenses
    //            .Include(x => x.Settings)
    //            .AsNoTracking()
    //            .FirstOrDefaultAsync(x => x.Id == licenseId);
    //        return Convert(dbLicense);
    //    }

    //    public async Task<LmsLicenseDto> GetLicense(Guid consumerKey)
    //    {
    //        var dbLicense = await _dbContext.LmsLicenses
    //            .Include(x => x.Settings)
    //            .AsNoTracking()
    //            .FirstOrDefaultAsync(x => x.ConsumerKey == consumerKey.ToString());
    //        return Convert(dbLicense);
    //    }


    //    private static LmsLicenseDto Convert(LmsLicense dbLicense)
    //    {
    //        return new LmsLicenseDto
    //        {
    //            Id = dbLicense.Id,
    //            Domain = dbLicense.Domain,
    //            ConsumerKey = Guid.Parse(dbLicense.ConsumerKey),
    //            //LmsProviderId = dbLicense.LmsProviderId,
    //            SharedSecret = Guid.Parse(dbLicense.SharedSecret),
    //            Settings = dbLicense.Settings.ToDictionary(x => x.Name, x => x.Value)
    //        };
    //    }

    //}

    public class LmsLicenseInternalApiService : ILmsLicenseService
    {
        private readonly IJsonDeserializer _jsonDeserializer;
        private readonly dynamic _settings;
        private readonly ILogger _logger;

        public LmsLicenseInternalApiService(IJsonDeserializer jsonDeserializer, ApplicationSettingsProvider settings, ILogger logger)
        {
            _jsonDeserializer = jsonDeserializer;
            _settings = settings;
            _logger = logger;
        }

        public async Task<LmsLicenseDto> GetLicense(int licenseId)
        {
            throw new NotImplementedException();
        }

        public async Task<LmsLicenseDto> GetLicense(Guid consumerKey)
        {
            HttpResponseMessage httpResponseMessage = null;
            try
            {
                using (var httpClient = new System.Net.Http.HttpClient())
                {
                    httpResponseMessage = await httpClient.GetAsync(
                        $"{_settings.LicenseServiceUrl}/{consumerKey}");
                }
                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    var resp = await httpResponseMessage.Content.ReadAsStringAsync();
                    OperationResultWithData<LmsLicenseDto> licenseDtoResult =
                        _jsonDeserializer.JsonDeserialize<OperationResultWithData<LmsLicenseDto>>(resp);
                    if (!licenseDtoResult.IsSuccess)
                    {
                        _logger.Error($"[GetLicense:{consumerKey}] {licenseDtoResult.Message}");
                        throw new ZoomLicenseException(consumerKey, licenseDtoResult.Message);
                    }

                    return licenseDtoResult.Data;
                }
                else if (httpResponseMessage.StatusCode == HttpStatusCode.NotFound)
                {
                    _logger.Error($"[GetLicense:{consumerKey}] Not found");
                    throw new ZoomLicenseException(consumerKey, "License not found");
                }
            }
            catch (HttpRequestException e)
            {
                _logger.Error($"[GetLicense:{consumerKey}] Unexpected exception", e);
                throw new ZoomLicenseException(consumerKey, "Unexpected error happened when retrieving license data. Please contact support.");
            }

            _logger.Error($"[GetLicense:{consumerKey}] Response: {httpResponseMessage.ToString()}"); //todo: check that httpResponseMessage.ToString() returns enough information
            throw new ZoomLicenseException(consumerKey, "License data could not be retrieved. Please contact support.");
        }
    }

}