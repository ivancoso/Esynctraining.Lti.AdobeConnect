using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using EdugameCloud.Lti.API.AdobeConnect;
using EdugameCloud.Lti.Core.DTO;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.Extensions;
using Esynctraining.AC.Provider.Entities;
using Esynctraining.Core.Extensions;
using Esynctraining.Core.Utils;

namespace EdugameCloud.Lti.Controllers
{
    public partial class LtiController
    {
        private IRecordingsService RecordingsService
        {
            get { return IoC.Resolve<IRecordingsService>(); }
        }

        #region Public Methods and Operators

        [HttpPost]
        public virtual JsonResult DeleteRecording(string lmsProviderName, int meetingId, string id)
        {
            LmsCompany lmsCompany = null;
            try
            {
                var session = GetReadOnlySession(lmsProviderName);
                lmsCompany = session.LmsCompany;
                var param = session.LtiSession.With(x => x.LtiParam);

                OperationResult result = RecordingsService.RemoveRecording(
                    lmsCompany,
                    this.GetAdobeConnectProvider(lmsCompany),
                    param.course_id,
                    id,
                    meetingId);

                return Json(result);
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("DeleteRecording", lmsCompany, ex);
                return Json(OperationResult.Error(errorMessage));
            }
        }
        
        [HttpPost]
        public virtual JsonResult GetRecordings(string lmsProviderName, int meetingId)
        {
            LmsCompany lmsCompany = null;
            try
            {
                var session = GetReadOnlySession(lmsProviderName);
                lmsCompany = session.LmsCompany;
                var param = session.LtiSession.LtiParam;    

                List<RecordingDTO> recordings = RecordingsService.GetRecordings(
                    lmsCompany,
                    this.GetAdobeConnectProvider(lmsCompany),
                    param.course_id,
                    meetingId);

                return Json(OperationResult.Success(recordings));
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("GetRecordings", lmsCompany, ex);
                return Json(OperationResult.Error(errorMessage));
            }
        }
        
        [HttpGet]
        public virtual ActionResult JoinRecording(string lmsProviderName, string recordingUrl)
        {
            LmsCompany lmsCompany = null;
            try
            {
                var session = GetReadOnlySession(lmsProviderName);
                lmsCompany = session.LmsCompany;
                var param = session.LtiSession.With(x => x.LtiParam);
                var breezeSession = string.Empty;
                var provider = GetAdobeConnectProvider(lmsCompany);
                string url = RecordingsService.JoinRecording(lmsCompany, param, recordingUrl, ref breezeSession, adobeConnectProvider:provider);
                return this.LoginToAC(url, breezeSession, lmsCompany);
            }
            catch (Exception ex)
            {
                return RecordingsError("JoinRecording", lmsProviderName, ex);
            }
        }
        
        [HttpPost]
        public virtual ActionResult ShareRecording(string lmsProviderName, string recordingId, bool isPublic, string password)
        {
            LmsCompany lmsCompany = null;
            try
            {
                var session = GetReadOnlySession(lmsProviderName);
                lmsCompany = session.LmsCompany;
                var link = RecordingsService.UpdateRecording(lmsCompany, this.GetAdobeConnectProvider(lmsCompany), recordingId, isPublic, password);

                return Json(OperationResult.Success(link));
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("ShareRecording", lmsCompany, ex);
                return Json(OperationResult.Error(errorMessage));
            }
        }
        
        [HttpGet]
        public virtual ActionResult EditRecording(string lmsProviderName, string recordingUrl)
        {
            LmsCompany lmsCompany = null;
            try
            {
                var session = GetReadOnlySession(lmsProviderName);
                lmsCompany = session.LmsCompany;
                var param = session.LtiSession.With(x => x.LtiParam);
                var breezeSession = string.Empty;
                var provider = GetAdobeConnectProvider(lmsCompany);

                string url = RecordingsService.JoinRecording(lmsCompany, param, recordingUrl, ref breezeSession, "edit", adobeConnectProvider: provider);
                return this.LoginToAC(url, breezeSession, lmsCompany);
            }
            catch (Exception ex)
            {
                return RecordingsError("EditRecording", lmsProviderName, ex);
            }
        }
        
        [HttpGet]
        public virtual ActionResult GetRecordingFlv(string lmsProviderName, string recordingUrl)
        {
            LmsCompany lmsCompany = null;
            try
            {
                var session = GetReadOnlySession(lmsProviderName);
                lmsCompany = session.LmsCompany;
                var param = session.LtiSession.With(x => x.LtiParam);
                var breezeSession = string.Empty;
                var provider = GetAdobeConnectProvider(lmsCompany);

                string url = RecordingsService.JoinRecording(lmsCompany, param, recordingUrl, ref breezeSession, "offline", adobeConnectProvider: provider);
                return this.LoginToAC(url, breezeSession, lmsCompany);
            }
            catch (Exception ex)
            {
                return RecordingsError("GetRecordingFlv", lmsProviderName, ex);
            }
        }

        [HttpPost]
        public virtual JsonResult ConvertToMP4(string lmsProviderName, string recordingId, int meetingId)
        {
            LmsCompany lmsCompany = null;
            try
            {
                var session = GetReadOnlySession(lmsProviderName);
                lmsCompany = session.LmsCompany;

                var adobeConnectProvider = this.GetAdobeConnectProvider(lmsCompany);
                if (adobeConnectProvider == null)
                {   
                    throw new InvalidOperationException("Adobe connect provider");
                }

                var recordingJob = adobeConnectProvider.ScheduleRecordingJob(recordingId);
                if (recordingJob == null)
                {
                    throw new InvalidOperationException("Adobe connect provider. Cannot get recording job.");
                }

                if (!recordingJob.Success)
                {
                    return Json(GenerateErrorResult(recordingJob.Status));
                }

                LmsCourseMeeting meeting = this.LmsCourseMeetingModel.GetOneByCourseAndId(lmsCompany.Id, session.LtiSession.LtiParam.course_id, meetingId);
                var scheduledRecording = GetScheduledRecording(recordingJob.RecordingJob.ScoId, meeting.GetMeetingScoId(), adobeConnectProvider);
                if (scheduledRecording == null)
                {
                    throw new InvalidOperationException("Adobe connect provider. Cannot get scheduled recording");
                }

                var recording = new RecordingDTO(scheduledRecording, lmsCompany.AcServer);

                return Json(OperationResult.Success(recording));
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("ConvertToMP4", lmsCompany, ex);
                return Json(OperationResult.Error(errorMessage));
            }
        }

        [HttpPost]
        public virtual JsonResult CancelMP4Converting(string lmsProviderName, string recordingId, int meetingId)
        {
            LmsCompany lmsCompany = null;
            try
            {
                var session = GetReadOnlySession(lmsProviderName);
                lmsCompany = session.LmsCompany;

                var adobeConnectProvider = this.GetAdobeConnectProvider(lmsCompany);
                if (adobeConnectProvider == null)
                {
                    throw new InvalidOperationException("Adobe connect provider");
                }

                LmsCourseMeeting meeting = this.LmsCourseMeetingModel.GetOneByCourseAndId(lmsCompany.Id, session.LtiSession.LtiParam.course_id, meetingId);
                var recording = GetScheduledRecording(recordingId, meeting.GetMeetingScoId(), adobeConnectProvider);
                if (recording == null)
                {
                    return Json(OperationResult.Error(Resources.Messages.RecordingMissedMP4));
                }

                if (recording.JobStatus == "job-queued")
                {
                    var recordingJob = adobeConnectProvider.CancelRecordingJob(recordingId);
                    if (recordingJob == null)
                    {
                        throw new InvalidOperationException("Adobe connect provider");
                    }

                    return Json(OperationResult.Success());                    
                }

                return Json(OperationResult.Error(Resources.Messages.RecordingCantDeleteMP4Progress));

            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("GetRecordings", lmsCompany, ex);
                return Json(OperationResult.Error(errorMessage));
            }
        }

        #endregion
        
        #region methods

        private static Recording GetScheduledRecording(string recordingScoId, string meetingScoId, IAdobeConnectProxy adobeConnectProvider)
        {
            var recordingsByMeeting = adobeConnectProvider.GetRecordingsList(meetingScoId);
            if (recordingsByMeeting == null || !recordingsByMeeting.Success || recordingsByMeeting.Values == null || !recordingsByMeeting.Values.Any() )
            {
                return null;
            }

            return recordingsByMeeting.Values.SingleOrDefault(x => x.ScoId == recordingScoId);
        }

        private static OperationResult GenerateErrorResult(StatusInfo status)
        {
            if (status.Code == StatusCodes.invalid && status.SubCode == StatusSubCodes.invalid_recording_job_in_progress)
            {
                return OperationResult.Error(Resources.Messages.RecordingAlreadyHasMP4);
            }
            if (status.Code == StatusCodes.no_access && status.SubCode == StatusSubCodes.denied)
            {
                return OperationResult.Error(Resources.Messages.RecordingDisabledMP4);
            }
            if (status.Code == StatusCodes.invalid && status.SubCode == StatusSubCodes.duplicate)
            {
                return OperationResult.Error(Resources.Messages.RecordingDuplicateMP4);
            }

            return OperationResult.Error("Unexpected error");
        }

        private ActionResult RecordingsError(string method, string sessionId, Exception ex)
        {
            logger.ErrorFormat(ex, "{0} exception. sessionId:{1}.", method, sessionId);
            this.ViewBag.DebugError = IsDebug ? (ex.Message + ex.StackTrace) : string.Empty;
            return this.View("~/Views/Lti/LtiError.cshtml");
        }

        #endregion

    }

}