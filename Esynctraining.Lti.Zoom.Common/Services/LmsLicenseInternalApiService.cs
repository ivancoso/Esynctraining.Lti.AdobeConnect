using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Esynctraining.Core.Domain;
using Esynctraining.Core.Json;
using Esynctraining.Core.Logging;
using Esynctraining.Core.Providers;
using Esynctraining.Lti.Zoom.Common.Dto;

namespace Esynctraining.Lti.Zoom.Common.Services
{
    public class LmsLicenseInternalApiService : ILmsLicenseService
    {
        private readonly IJsonDeserializer _jsonDeserializer;
        private readonly dynamic _settings;
        private readonly ILogger _logger;
        private static readonly HttpClient _httpClient = new HttpClient();

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
                httpResponseMessage = await _httpClient.GetAsync($"{_settings.LicenseServiceUrl}/{consumerKey}");
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

        public async Task<LmsLicenseDto> UpdateOAuthTokensForLicense(Guid consumerKey, string accessToken, string refreshToken)
        {
            HttpResponseMessage httpResponseMessage = null;
            try
            {
                httpResponseMessage = await _httpClient.PutAsync($"{_settings.LicenseServiceUrl}/{consumerKey}/tokens?accessToken={accessToken}&refreshToken={refreshToken}", null);
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
                    _logger.Error($"[UpdateOAuthTokensForLicense:{consumerKey}] Not found");
                    throw new ZoomLicenseException(consumerKey, "License not found");
                }
            }
            catch (HttpRequestException e)
            {
                _logger.Error($"[UpdateOAuthTokensFor License:{consumerKey}] Unexpected exception", e);
                throw new ZoomLicenseException(consumerKey, "Unexpected error happened when retrieving license data. Please contact support.");
            }

            _logger.Error($"[UpdateOAuthTokensForLicense:{consumerKey}] Response: {httpResponseMessage.ToString()}"); //todo: check that httpResponseMessage.ToString() returns enough information
            throw new ZoomLicenseException(consumerKey, "License data could not be retrieved. Please contact support.");

        }
    }

}