using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Esynctraining.Core.Domain;
using Esynctraining.Core.Logging;
using Esynctraining.Core.Providers;
using Esynctraining.Lti.Zoom.Api.Dto;
using Esynctraining.Lti.Zoom.Api.Host.Filters;
using Esynctraining.Lti.Zoom.Api.Services;
using Esynctraining.Lti.Zoom.Common;
using Esynctraining.Lti.Zoom.Common.Services;
using Microsoft.AspNetCore.Mvc;

namespace Esynctraining.Lti.Zoom.Api.Host.Controllers
{
    [Route("")]
    public class RecordingsController : BaseApiController
    {
        private readonly ZoomRecordingService _recordingService;
        private readonly ZoomUserService _userService;
        private readonly ZoomMeetingService _meetingService;
        private readonly ExternalStorageService _storageService;
        private readonly KalturaService _kalturaService;

        public RecordingsController(
            //MeetingSetup meetingSetup,
            //API.AdobeConnect.IAdobeConnectAccountService acAccountService,
            ApplicationSettingsProvider settings,
            ILogger logger, ZoomRecordingService recordingService, ZoomMeetingService meetingService,
            ZoomUserService userService, ExternalStorageService storageService, KalturaService kalturaService)
            : base(settings, logger)
        {
            _meetingService = meetingService;
            _recordingService = recordingService;
            _userService = userService;
            _storageService = storageService;
            _kalturaService = kalturaService;
        }

        [Route("meetings/{meetingId}/recordings")]
        [HttpGet]
        [LmsAuthorizeBase(ApiCallEnabled = true)]
        public async Task<OperationResultWithData<RecordingsDto>> GetRecordings(int meetingId)
        {
            var dbMeeting = await _meetingService.GetMeeting(meetingId, CourseId);
            if (dbMeeting == null)
                return OperationResultWithData<RecordingsDto>.Error("Meeting not found");

            var zoomRecordings = await _recordingService.GetRecordings(dbMeeting.ProviderHostId, dbMeeting.ProviderMeetingId);
            var externalRecordings = await _storageService.GetExternalFileRecords(meetingId);
            var result = new RecordingsDto{ ZoomRecordings = zoomRecordings, ExternalRecordings = externalRecordings};
            return result.ToSuccessResult();
        }

        [Route("meetings/{meetingId}/recordings/trash")]
        [HttpGet]
        [LmsAuthorizeBase(ApiCallEnabled = true)]
        public virtual async Task<OperationResultWithData<IEnumerable<ZoomRecordingsTrashItemDto>>> GetTrashRecordings(int meetingId)
        {
            var dbMeeting = await _meetingService.GetMeeting(meetingId, CourseId);
            if (dbMeeting == null)
                return OperationResultWithData<IEnumerable<ZoomRecordingsTrashItemDto>>.Error("Meeting not found");
            var recordings = await _recordingService.GetTrashRecordings(dbMeeting.ProviderMeetingId, dbMeeting.ProviderHostId);
            return recordings.ToSuccessResult();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="meetingId"></param>
        /// <param name="recordingFileId"></param>
        /// <param name="trash">Flag which means how to remove recording. TRUE : Remove to TRASH, FALSE : Remove without restoring.</param>
        /// <returns></returns>
        [Route("meetings/{meetingId}/recordings/files/{recordingFileId}")]
        [HttpDelete]
        [LmsAuthorizeBase(ApiCallEnabled = true)]
        public virtual async Task<OperationResult> DeleteRecording(int meetingId, string recordingFileId, [FromQuery]bool trash = true ) 
        {
            string userId = null;
            try
            {
                var user = await _userService.GetUser(Param.lis_person_contact_email_primary);
                userId = user.Id;
            }
            catch (Exception e)
            {
                Logger.Error($"User {Param.lis_person_contact_email_primary} doesn't exist or doesn't belong to this account", e);
            }

            var dbMeeting = await _meetingService.GetMeeting(meetingId, CourseId);
            if (dbMeeting == null)
                return OperationResult.Error("Meeting not found");

            var meetingSessionId =
                await _recordingService.GetMeetingUuId(userId, dbMeeting.ProviderMeetingId, recordingFileId, !trash);

            if (meetingSessionId == null)
            {
                return OperationResult.Error("Recording file not found."); //either removed or in trash(i.e. if request was made with trash=false)
            }

            var result = await _recordingService.DeleteRecordings(meetingSessionId, recordingFileId,  trash);
            return result ? OperationResult.Success() : OperationResult.Error("Error when deleting recordings.");
        }

        [Route("meetings/{meetingId}/recordings/{recordingId}")]
        [HttpDelete]
        [LmsAuthorizeBase(ApiCallEnabled = true)]
        public virtual async Task<OperationResult> DeleteAllRecordingFiles(int meetingId, string recordingId, [FromQuery]bool trash = false) //this method is called only from trash, so no need to set trash=true by default
        {
            try
            {
                var dbMeeting = await _meetingService.GetMeeting(meetingId, CourseId);
                if (dbMeeting == null)
                    return OperationResult.Error("Meeting not found");
                var result = _recordingService.DeleteRecordings(WebUtility.UrlDecode(recordingId).Replace(" ", "+"),
                    null, trash);
            }
            catch (ZoomLicenseException e)
            {
                throw;
            }
            catch (Exception e)
            {
                //when delete recordings from trash, Zoom API sends 404 -> exception.
            }

            return OperationResult.Success();
        }

        [Route("meetings/{meetingId}/recordings/files/{recordingFileId}/status/recover")]
        [HttpPut]
        [LmsAuthorizeBase(ApiCallEnabled = true)]
        public virtual async Task<OperationResult> RecoverRecording(int meetingId, string recordingFileId)
        {
            string userId = null;
            try
            {
                var user = await _userService.GetUser(Param.lis_person_contact_email_primary);
                userId = user.Id;
            }
            catch (Exception e)
            {
                Logger.Error($"User {Param.lis_person_contact_email_primary} doesn't exist or doesn't belong to this account", e);
            }

            var dbMeeting = await _meetingService.GetMeeting(meetingId, CourseId);
            if (dbMeeting == null)
                return OperationResult.Error("Meeting not found");
            //if (meeting.AudioProfileId != userId)
            //    return OperationResult.Error("You are trying to delete other user's recording file.");

            var meetingSessionId =
                await _recordingService.GetMeetingUuId(userId, dbMeeting.ProviderMeetingId, recordingFileId, true);
            if (meetingSessionId == null)
            {
                return OperationResult.Error("Recording file not found."); //either removed or not in trash
            }

            var result = await _recordingService.RecoverRecordings(meetingSessionId, recordingFileId);
            return result ? OperationResult.Success() : OperationResult.Error("Error when recovering recordings.");
        }

        [Route("meetings/{meetingId}/recordings/{recordingId}/status/recover")]
        [HttpPut]
        [LmsAuthorizeBase(ApiCallEnabled = true)]
        public virtual async Task<OperationResult> RecoverAllRecordings(int meetingId, string recordingId)
        {
            var dbMeeting = await _meetingService.GetMeeting(meetingId, CourseId);
            if (dbMeeting == null)
                return OperationResult.Error("Meeting not found");
            var result = await _recordingService.RecoverRecordings(WebUtility.UrlDecode(recordingId).Replace(" ", "+"));
            return result? OperationResult.Success() : OperationResult.Error("Error when recovering recordings.");
        }

        [Route("meetings/{meetingId}/external-recordings")]
        [HttpPost]
        [LmsAuthorizeBase]
        public async Task<OperationResult> AddExternalRecording(int meetingId, [FromBody]AddExternalRecordingDto dto)
        {
            var dbMeeting = await _meetingService.GetMeeting(meetingId, CourseId);
            if (dbMeeting == null)
                return OperationResult.Error("Meeting not found");

            var result = await _storageService.AddExternalFileRecord(dbMeeting, dto.ProviderId, dto.ProviderFileRecordId);
            return result;
        }

        [Route("meetings/{meetingId}/external-recordings")]
        [HttpDelete]
        [LmsAuthorizeBase]
        public async Task<OperationResult> DeleteExternalRecording(int meetingId, [FromQuery]int providerId, [FromQuery]string providerFileRecordId)
        {
            var dbMeeting = await _meetingService.GetMeeting(meetingId, CourseId);
            if (dbMeeting == null)
                return OperationResult.Error("Meeting not found");

            var result = await _storageService.DeleteExternalFileRecord(meetingId, providerId, providerFileRecordId);
            return result;
        }
    }
}