using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Esynctraining.Core.Domain;
using Esynctraining.Core.Providers;
using Esynctraining.Lti.Lms.Common.Constants;
using Esynctraining.Lti.Zoom.Api.Dto;
using Esynctraining.Lti.Zoom.Api.Dto.Kaltura;
using Esynctraining.Lti.Zoom.Common;
using Esynctraining.Lti.Zoom.Common.Services;
using Kaltura;
using Kaltura.Enums;
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

        public async Task<IEnumerable<ExternalMediaDto>> GetMediaRecords(IEnumerable<string> ids)
        {
            if (ids == null)
                throw new ArgumentNullException(nameof(ids));

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
                var filter = new MediaEntryFilter {IdIn = string.Join(",", ids)};
                var result = MediaService.List(filter).ExecuteAndWaitForResponse(client);
                return result.Objects.Select(x=> new ExternalMediaDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    CreatedAt = x.CreatedAt,
                    MsDuration = x.MsDuration,
                    DataUrl = x.DataUrl,
                    DownloadUrl = x.DownloadUrl,
                    Status = x.Status.ToString(),
                    ThumbnailUrl = x.ThumbnailUrl,
                    Description = x.Description,
                    Views = x.Views
                });
            }
            catch (Exception e)
            {
                _logger.Error($"[GetMediaRecords] Error. licenseKey={licenseDto.ConsumerKey}", e);
                return new List<ExternalMediaDto>();
            }
        }

        public async Task<OperationResult> ApproveMedia(string mediaEntryId)
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
                _logger.Error($"[ApproveMedia] Error. licenseKey={licenseDto.ConsumerKey}", e);
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