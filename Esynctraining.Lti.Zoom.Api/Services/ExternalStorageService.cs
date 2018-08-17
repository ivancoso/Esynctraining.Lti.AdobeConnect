using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Esynctraining.Core.Domain;
using Esynctraining.Lti.Zoom.Api.Dto;
using Esynctraining.Lti.Zoom.Domain;
using Microsoft.EntityFrameworkCore;

namespace Esynctraining.Lti.Zoom.Api.Services
{
    public class ExternalStorageService
    {
        private readonly ZoomDbContext _dbContext;
        private readonly KalturaService _kalturaService;

        public ExternalStorageService(ZoomDbContext dbContext, KalturaService kalturaService)
        {
            _dbContext = dbContext;
            _kalturaService = kalturaService;
        }

        public async Task<OperationResult> AddExternalFileRecord(LmsCourseMeeting dbMeeting,
            int providerId, string providerFileRecordId)
        {
            if (await _dbContext.ExternalFiles.AnyAsync(x => x.Meeting.Id == dbMeeting.Id))
                return OperationResult.Error("File with this external id is already added.");

            var beforeSaveResult = await BeforeSave(providerId, providerFileRecordId);
            if (!beforeSaveResult.IsSuccess)
                return beforeSaveResult;

            var fileInfo = new ExternalFileInfo{Meeting = dbMeeting, ProviderId = (ExternalStorageProvider)providerId, ProviderFileRecordId = providerFileRecordId };
            await _dbContext.ExternalFiles.AddAsync(fileInfo);
            await _dbContext.SaveChangesAsync();
            return OperationResult.Success();
        }

        public async Task<OperationResult> BeforeSave(int providerId, string providerFileRecordId)
        {
            //todo: separate service if needed
            switch ((ExternalStorageProvider)providerId)
            {
                case ExternalStorageProvider.Kaltura:
                    return await _kalturaService.ApproveMedia(providerFileRecordId);
                default:
                    return OperationResult.Success();
            }
        }

        public async Task<OperationResult> DeleteExternalFileRecord(int meetingId,
            int providerId, string providerFileRecordId)
        {
            ExternalStorageProvider prId = (ExternalStorageProvider) providerId;
            var fileInfo = await _dbContext.ExternalFiles.FirstOrDefaultAsync(x =>
                x.Meeting.Id == meetingId && x.ProviderId == prId &&
                x.ProviderFileRecordId == providerFileRecordId);
            if (fileInfo == null)
                return OperationResult.Error("File not found.");
            
            _dbContext.ExternalFiles.Remove(fileInfo);
            await _dbContext.SaveChangesAsync();
            return OperationResult.Success();
        }

        public async Task<IEnumerable<ExternalRecordingsDto>> GetExternalFileRecords(int meetingId)
        {
            var fileInfos = await _dbContext.ExternalFiles.Where(x => x.Meeting.Id == meetingId).ToListAsync();
            var result = fileInfos.GroupBy(x => x.ProviderId).Select(x =>
                new ExternalRecordingsDto {ProviderId = x.Key, RecordingIds = x.Select(ef => ef.ProviderFileRecordId)});
            return result.ToList();
        }
    }
}