using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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

        public async Task<KalturaSessionDto> GetKalturaSession()
        {
            var client = await GetClient();

            return new KalturaSessionDto { KS = client.KS, ServiceUrl = client.Configuration.ServiceUrl };
        }

        public async Task<MediaEntryDto> UploadVideoMediaAsync(string name, string description, IEnumerable<string> tagsToAdd, string uploadedFileTokenId)
        {
            var client = await GetClient();

            MediaEntryDto result = null;

            var tags = new List<string>();
            tags.AddRange(tagsToAdd);

            var mediaEntry = new MediaEntry
            {
                MediaType = MediaType.VIDEO,
                Name = name,
                Description = description,
                Tags = string.Join(",", tags),
            };

            var response = MediaService.AddFromUploadedFile(mediaEntry, uploadedFileTokenId)
                .ExecuteAndWaitForResponse(client);
            result = new MediaEntryDto
            {
                Id = response.Id,
                Name = response.Name,
                Description = response.Description,
                Views = mediaEntry.Views,
                Duration = mediaEntry.Duration,
                DataUrl = mediaEntry.DataUrl,
                ThumbnailUrl = response.ThumbnailUrl,
                CreatedAt = response.CreatedAt,
                Status = response.Status.ToString(),
            };

            if (!tags.Contains("overview"))
            {
                MediaService.Approve(result.Id).ExecuteAndWaitForResponse(client);
            }

            return result;
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
            var session = SessionService.Start(adminSecret, "", SessionType.ADMIN, partnerId, 86400, "")
                .ExecuteAndWaitForResponse(client);
            client.KS = session;
            return client;
        }
    }
}