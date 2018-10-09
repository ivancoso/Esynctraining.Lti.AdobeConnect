using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Esynctraining.Core.Domain;
using Esynctraining.Core.Logging;
using Esynctraining.Lti.Lms.Common.Constants;
using Esynctraining.Lti.Zoom.Api.Dto;
using Esynctraining.Lti.Zoom.Common.Services;
using Esynctraining.Lti.Zoom.Domain;
using Microsoft.EntityFrameworkCore;

namespace Esynctraining.Lti.Zoom.Api.Services
{
    public class ExternalStorageService
    {
        private readonly ZoomDbContext _dbContext;
        private readonly KalturaService _kalturaService;
        private readonly ILmsLicenseAccessor _licenseAccessor;
        private readonly ILogger _logger;

        public ExternalStorageService(ZoomDbContext dbContext, KalturaService kalturaService,
            ILmsLicenseAccessor licenseAccessor, ILogger logger)
        {
            _dbContext = dbContext;
            _kalturaService = kalturaService;
            _licenseAccessor = licenseAccessor;
            _logger = logger;
        }

        public async Task<OperationResult> AddExternalFileRecord(LmsCourseMeeting dbMeeting,
            int providerId, string providerFileRecordId)
        {
            ExternalStorageProvider provider = (ExternalStorageProvider) providerId;
            if (await _dbContext.ExternalFiles.AnyAsync(x => x.Meeting.Id == dbMeeting.Id
                                                             && x.ProviderId == provider &&
                                                             x.ProviderFileRecordId == providerFileRecordId))
                return OperationResult.Error("File with this external id is already added.");

            var beforeSaveResult = await BeforeSave(providerId, providerFileRecordId);
            if (!beforeSaveResult.IsSuccess)
                return beforeSaveResult;

            var fileInfo = new ExternalFileInfo
            {
                Meeting = dbMeeting,
                ProviderId = (ExternalStorageProvider) providerId,
                ProviderFileRecordId = providerFileRecordId
            };
            await _dbContext.ExternalFiles.AddAsync(fileInfo);
            await _dbContext.SaveChangesAsync();
            return OperationResult.Success();
        }

        public async Task<OperationResult> BeforeSave(int providerId, string providerFileRecordId)
        {
            //todo: separate service
            switch ((ExternalStorageProvider)providerId)
            {
                case ExternalStorageProvider.Kaltura:
                    return await _kalturaService.ApproveMedia(providerFileRecordId);
                default:
                    return OperationResult.Success();
            }
        }

        private async Task<IEnumerable<ExternalMediaDto>> GetExternalMedia(ExternalStorageProvider providerId,
            IEnumerable<string> ids)
        {
            //todo: separate service
            switch ((ExternalStorageProvider)providerId)
            {
                case ExternalStorageProvider.Kaltura:
                    return await _kalturaService.GetMediaRecords(ids);
                default:
                    return new List<ExternalMediaDto>();
            }
        }

        public async Task<OperationResult> DeleteExternalFileRecord(int meetingId,
            int providerId, string providerFileRecordId)
        {
            ExternalStorageProvider provider = (ExternalStorageProvider) providerId;
            var fileInfo = await _dbContext.ExternalFiles.FirstOrDefaultAsync(x =>
                x.Meeting.Id == meetingId && x.ProviderId == provider &&
                x.ProviderFileRecordId == providerFileRecordId);
            if (fileInfo == null)
                return OperationResult.Error("File not found.");
            
            _dbContext.ExternalFiles.Remove(fileInfo);
            await _dbContext.SaveChangesAsync();
            return OperationResult.Success();
        }

        public async Task<IEnumerable<ExternalRecordingsDto>> GetExternalFileRecords(int meetingId)
        {
            var providers = await GetEnabledStorageProviders();
            if (!providers.Any())
            {
                return new List<ExternalRecordingsDto>();
            }

            var fileInfos = await _dbContext.ExternalFiles
                .Where(x => x.Meeting.Id == meetingId && providers.Any(p => p == x.ProviderId)).ToListAsync();
            var grouped = fileInfos.GroupBy(x => x.ProviderId);
                var result = grouped.Select(async (x) =>
            {
                var recordings = await GetExternalMedia(x.Key, x.Select(ef => ef.ProviderFileRecordId));
                return new ExternalRecordingsDto
                {
                    ProviderId = x.Key,
                    Recordings = recordings
                };
            });
            var recordingsDtos = new List<ExternalRecordingsDto>();
            foreach (var task in result)
            {
                recordingsDtos.Add(await task);
            }

            return recordingsDtos;
        }

        private async Task<List<ExternalStorageProvider>> GetEnabledStorageProviders()
        {
            var result = new List<ExternalStorageProvider>();
            var licenseDto = await _licenseAccessor.GetLicense();

            var enabledKaltura = licenseDto.GetSetting<bool>(LmsLicenseSettingNames.EnableKaltura);
            if (enabledKaltura)
            {
                result.Add(ExternalStorageProvider.Kaltura);
            }

            return result;
        }
    }
}