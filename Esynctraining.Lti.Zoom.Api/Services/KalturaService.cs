using System;
using System.Threading.Tasks;
using Esynctraining.Core.Providers;
using Esynctraining.Lti.Lms.Common.Constants;
using Esynctraining.Lti.Zoom.Api.Dto.Kaltura;
using Kaltura;
using Kaltura.Enums;
using Kaltura.Request;
using Kaltura.Services;
using ILogger = Esynctraining.Core.Logging.ILogger;

namespace Esynctraining.Lti.Zoom.Api.Services
{
    public class KalturaService
    {
        private readonly ILogger _logger;
        private readonly ILmsLicenseAccessor _licenseAccessor;
        private readonly dynamic _settings;

        public KalturaService(ApplicationSettingsProvider settings, ILmsLicenseAccessor licenseAccessor, ILogger logger)
        {
            _settings = settings;
            _licenseAccessor = licenseAccessor;
            _logger = logger;
        }

        public async Task<KalturaSessionDto> GetKalturaSession()
        {
            var client = await GetClient();

            return new KalturaSessionDto { KS = client.KS, ServiceUrl = client.Configuration.ServiceUrl };
        }

        private async Task<Client> GetClient(string ks = null)
        {
            var licenseDto = await _licenseAccessor.GetLicense();

            var client = new Client(new Configuration()
            {
                ServiceUrl = _settings.KalturaApiUrl
            });

            var adminSecret = licenseDto.GetSetting<string>(LmsLicenseSettingNames.KalturaAdminSecret);
            var partnerId = licenseDto.GetSetting<int>(LmsLicenseSettingNames.KalturaAdminPartnerId);
            SessionService.Start(adminSecret, "", SessionType.ADMIN, partnerId, 86400, "")
                .SetCompletion(new OnCompletedHandler<string>(
                    (string response, Exception e) =>
                    {
                        if (e != null)
                            throw e;

                        client.KS = response;
                    }))
                .ExecuteAndWaitForResponse(client);

            return client;
        }
    }
}