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

            var userSecret = licenseDto.GetSetting<string>(LmsLicenseSettingNames.KalturaUserSecret);
            var partnerId = licenseDto.GetSetting<int>(LmsLicenseSettingNames.KalturaAdminPartnerId);
            var client = await GetClient(SessionType.USER, userSecret, partnerId);

            return new KalturaSessionDto { KS = client.KS, ServiceUrl = client.Configuration.ServiceUrl };
        }

        public async Task<OperationResult> ApproveMedia(string mediaEntryId)//name, string description, IEnumerable<string> tagsToAdd, string uploadedFileTokenId
        {
            var licenseDto = await _licenseAccessor.GetLicense();

            var adminSecret = licenseDto.GetSetting<string>(LmsLicenseSettingNames.KalturaAdminSecret);
            var partnerId = licenseDto.GetSetting<int>(LmsLicenseSettingNames.KalturaAdminPartnerId);
            var client = await GetClient(SessionType.ADMIN, adminSecret, partnerId);

            //MediaEntryDto result = null;

            //var tags = new List<string>();
            //tags.AddRange(tagsToAdd);

            //var mediaEntry = new MediaEntry
            //{
            //    MediaType = MediaType.VIDEO,
            //    Name = name,
            //    Description = description,
            //    Tags = string.Join(",", tags),
            //};

            //var response = MediaService.AddFromUploadedFile(mediaEntry, uploadedFileTokenId)
            //    .ExecuteAndWaitForResponse(client);
            //result = new MediaEntryDto
            //{
            //    Id = response.Id,
            //    Name = response.Name,
            //    Description = response.Description,
            //    Views = mediaEntry.Views,
            //    Duration = mediaEntry.Duration,
            //    DataUrl = mediaEntry.DataUrl,
            //    ThumbnailUrl = response.ThumbnailUrl,
            //    CreatedAt = response.CreatedAt,
            //    Status = response.Status.ToString(),
            //};
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