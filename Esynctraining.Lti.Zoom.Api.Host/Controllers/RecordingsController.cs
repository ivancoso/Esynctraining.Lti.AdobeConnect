using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Esynctraining.Core.Caching;
using Esynctraining.Core.Domain;
using Esynctraining.Core.Logging;
using Esynctraining.Core.Providers;
using Esynctraining.Lti.Zoom.Api.Dto;
using Esynctraining.Lti.Zoom.Api.Host.FIlters;
using Esynctraining.Lti.Zoom.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Esynctraining.Lti.Zoom.Api.Host.Controllers
{
    [Microsoft.AspNetCore.Mvc.Route("")]
    public class RecordingsController : BaseApiController
    {
        private readonly ZoomRecordingService _recordingService;
        private readonly ZoomUserService _userService;
        private readonly ZoomMeetingService _meetingService;

        public RecordingsController(
            //MeetingSetup meetingSetup,
            //API.AdobeConnect.IAdobeConnectAccountService acAccountService,
            ApplicationSettingsProvider settings,
            ILogger logger, ZoomRecordingService recordingService, ZoomMeetingService meetingService, ZoomUserService userService)
            : base(settings, logger)
        {
            _meetingService = meetingService;
            _recordingService = recordingService;
            _userService = userService;
        }

        [Microsoft.AspNetCore.Mvc.Route("meetings/{meetingId}/recordings")]
        [Microsoft.AspNetCore.Mvc.HttpGet]
        [LmsAuthorizeBase(ApiCallEnabled = true)]
        public virtual async Task<OperationResultWithData<IEnumerable<ZoomRecordingSessionDto>>> GetRecordings(int meetingId)
        {
            var dbMeeting = await _meetingService.GetMeeting(meetingId, CourseId);
            if (dbMeeting == null)
                return OperationResultWithData<IEnumerable<ZoomRecordingSessionDto>>.Error("Meeting not found");
            var recordings = _recordingService.GetRecordings(dbMeeting.ProviderHostId, dbMeeting.ProviderMeetingId);
            return recordings.ToSuccessResult();
        }
        [Microsoft.AspNetCore.Mvc.Route("meetings/{meetingId}/recordings/trash")]
        [Microsoft.AspNetCore.Mvc.HttpGet]
        [LmsAuthorizeBase(ApiCallEnabled = true)]
        public virtual async Task<OperationResultWithData<IEnumerable<ZoomRecordingsTrashItemDto>>> GetTrashRecordings(int meetingId)
        {
            var dbMeeting = await _meetingService.GetMeeting(meetingId, CourseId);
            if (dbMeeting == null)
                return OperationResultWithData<IEnumerable<ZoomRecordingsTrashItemDto>>.Error("Meeting not found");
            var recordings = _recordingService.GetTrashRecordings(dbMeeting.ProviderMeetingId, dbMeeting.ProviderHostId);
            return recordings.ToSuccessResult();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="meetingId"></param>
        /// <param name="recordingFileId"></param>
        /// <param name="trash">Flag whicj means how to remove recording. TRUE : Remove to TRASH, FALSE : Remove without restoring.</param>
        /// <returns></returns>
        [Route("meetings/{meetingId}/recordings/files/{recordingFileId}")]
        [HttpDelete]
        [LmsAuthorizeBase(ApiCallEnabled = true)]
        public virtual async Task<OperationResult> DeleteRecording(int meetingId, string recordingFileId, [FromQuery]bool trash = true ) 
        {
            string userId = null;
            try
            {
                var user = _userService.GetUser(Param.lis_person_contact_email_primary);
                userId = user.Id;
            }
            catch (Exception e)
            {
                Logger.Error("User doesn't exist or doesn't belong to this account", e);
            }

            var dbMeeting = await _meetingService.GetMeeting(meetingId, CourseId);
            if (dbMeeting == null)
                return OperationResult.Error("Meeting not found");

            var meetingSessionId =
                _recordingService.GetMeetingUuId(userId, dbMeeting.ProviderMeetingId, recordingFileId, !trash);

            if (meetingSessionId == null)
            {
                return OperationResult.Error("Recording file not found."); //either removed or in trash(i.e. if request was made with trash=false)
            }

            var result = _recordingService.DeleteRecordings(meetingSessionId, recordingFileId,  trash);
            return result ? OperationResult.Success() : OperationResult.Error("Error when deleting recordings.");
        }

        [Microsoft.AspNetCore.Mvc.Route("meetings/{meetingId}/recordings/{recordingId}")]
        [Microsoft.AspNetCore.Mvc.HttpDelete]
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

        [Microsoft.AspNetCore.Mvc.Route("meetings/{meetingId}/recordings/files/{recordingFileId}/status/recover")]
        [Microsoft.AspNetCore.Mvc.HttpPut]
        [LmsAuthorizeBase(ApiCallEnabled = true)]
        public virtual async Task<OperationResult> RecoverRecording(int meetingId, string recordingFileId)
        {
            string userId = null;
            try
            {
                var user = _userService.GetUser(Param.lis_person_contact_email_primary);
                userId = user.Id;
            }
            catch (Exception e)
            {
                Logger.Error("User doesn't exist or doesn't belong to this account", e);
            }

            var dbMeeting = await _meetingService.GetMeeting(meetingId, CourseId);
            if (dbMeeting == null)
                return OperationResult.Error("Meeting not found");
            //if (meeting.AudioProfileId != userId)
            //    return OperationResult.Error("You are trying to delete other user's recording file.");

            var meetingSessionId =
                _recordingService.GetMeetingUuId(userId, dbMeeting.ProviderMeetingId, recordingFileId, true);
            if (meetingSessionId == null)
            {
                return OperationResult.Error("Recording file not found."); //either removed or not in trash
            }

            var result = _recordingService.RecoverRecordings(meetingSessionId, recordingFileId);
            return result ? OperationResult.Success() : OperationResult.Error("Error when recovering recordings.");
        }

        [Microsoft.AspNetCore.Mvc.Route("meetings/{meetingId}/recordings/{recordingId}/status/recover")]
        [Microsoft.AspNetCore.Mvc.HttpPut]
        [LmsAuthorizeBase(ApiCallEnabled = true)]
        public virtual async Task<OperationResult> RecoverAllRecordings(int meetingId, string recordingId)
        {
            var dbMeeting = await _meetingService.GetMeeting(meetingId, CourseId);
            if (dbMeeting == null)
                return OperationResult.Error("Meeting not found");
            var result = _recordingService.RecoverRecordings(WebUtility.UrlDecode(recordingId).Replace(" ", "+"));
            return result? OperationResult.Success() : OperationResult.Error("Error when recovering recordings.");
        }
    }
}