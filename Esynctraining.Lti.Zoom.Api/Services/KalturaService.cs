using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Esynctraining.Core.Domain;
using Esynctraining.Core.Providers;
using Esynctraining.Lti.Lms.Common.Constants;
using Esynctraining.Lti.Zoom.Api.Dto.Kaltura;
using Kaltura;
using Kaltura.Enums;
using Kaltura.Request;
using Kaltura.Services;
using Kaltura.Types;
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

        public async Task<KalturaSessionDto> GetKalturaUserSession()
        {
            var licenseDto = await _licenseAccessor.GetLicense();

            var enabled = licenseDto.GetSetting<bool>(LmsLicenseSettingNames.EnableKaltura);
            var userSecret = licenseDto.GetSetting<string>(LmsLicenseSettingNames.KalturaUserSecret);
            var partnerId = licenseDto.GetSetting<int>(LmsLicenseSettingNames.KalturaAdminPartnerId);
            if (!enabled || string.IsNullOrEmpty(userSecret))
                throw new ZoomLicenseException(licenseDto.ConsumerKey,
                    "Kaltura integration is not enabled or settings are incorrect.");

            var client = await GetClient(SessionType.USER, userSecret, partnerId);

            return new KalturaSessionDto { KS = client.KS, ServiceUrl = client.Configuration.ServiceUrl };
        }

        public async Task<OperationResult> ApproveMedia(string mediaEntryId)//name, string description, IEnumerable<string> tagsToAdd, string uploadedFileTokenId
        {
            var licenseDto = await _licenseAccessor.GetLicense();

            var enabled = licenseDto.GetSetting<bool>(LmsLicenseSettingNames.EnableKaltura);
            var adminSecret = licenseDto.GetSetting<string>(LmsLicenseSettingNames.KalturaAdminSecret);
            var partnerId = licenseDto.GetSetting<int>(LmsLicenseSettingNames.KalturaAdminPartnerId);
            if (!enabled || string.IsNullOrEmpty(adminSecret))
                throw new ZoomLicenseException(licenseDto.ConsumerKey,
                    "Kaltura integration is not enabled or settings are incorrect.");

            var client = await GetClient(SessionType.ADMIN, adminSecret, partnerId);

            try
            {
                var result = MediaService.Approve(mediaEntryId).ExecuteAndWaitForResponse(client);
            }
            catch (Exception e)
            {
                //todo:
                return OperationResult.Error(e.Message);
            }

            return OperationResult.Success();
        }

        private async Task<Client> GetClient(SessionType sessionType, string secret, int partnerId)
        {
            var client = new Client(new Configuration()
            {
                ServiceUrl = _settings.KalturaApiUrl
            });
            
            var session = SessionService.Start(secret, "", sessionType, partnerId, 86400, "")
                .ExecuteAndWaitForResponse(client);
            client.KS = session;
            return client;
        }
    }
}