namespace EdugameCloud.Lti.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using EdugameCloud.Lti.API.AdobeConnect;
    using EdugameCloud.Lti.Core.DTO;
    using EdugameCloud.Lti.Domain.Entities;
    using Esynctraining.AC.Provider.Entities;
    using Esynctraining.Core.Extensions;
    
    public partial class LtiController
    {
        #region Public Methods and Operators
        
        [HttpPost]
        public virtual JsonResult DeleteRecording(string lmsProviderName, int meetingId, string id)
        {
            LmsCompany credentials = null;
            try
            {
                var session = this.GetSession(lmsProviderName);
                credentials = session.LmsCompany;
                var param = session.LtiSession.With(x => x.LtiParam);

                OperationResult result = this.meetingSetup.RemoveRecording(
                    credentials,
                    this.GetAdobeConnectProvider(credentials),
                    param.course_id,
                    id,
                    meetingId);

                return Json(result);
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("DeleteRecording", credentials, ex);
                return Json(OperationResult.Error(errorMessage));
            }
        }
        
        [HttpPost]
        public virtual JsonResult GetRecordings(string lmsProviderName, int meetingId)
        {
            LmsCompany credentials = null;
            try
            {
                var session = this.GetSession(lmsProviderName);
                credentials = session.LmsCompany;
                var param = session.LtiSession.LtiParam;    

                List<RecordingDTO> recordings = this.meetingSetup.GetRecordings(
                    credentials,
                    this.GetAdobeConnectProvider(credentials),
                    param.course_id,
                    meetingId);

                return Json(OperationResult.Success(recordings));
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("GetRecordings", credentials, ex);
                return Json(OperationResult.Error(errorMessage));
            }
        }
        
        [HttpGet]
        public virtual ActionResult JoinRecording(string lmsProviderName, string recordingUrl)
        {
            LmsCompany credentials = null;
            try
            {
                var session = this.GetSession(lmsProviderName);
                credentials = session.LmsCompany;
                var param = session.LtiSession.With(x => x.LtiParam);
                var userSettings = this.GetLmsUserSettingsForJoin(lmsProviderName, credentials, param, session);
                var breezeSession = string.Empty;

                string url = this.meetingSetup.JoinRecording(credentials, param, userSettings, recordingUrl, ref breezeSession);
                return this.LoginToAC(url, breezeSession, credentials);
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("JoinRecording", credentials, ex);
                return Json(OperationResult.Error(errorMessage), JsonRequestBehavior.AllowGet);
            }
        }
        
        [HttpPost]
        public virtual ActionResult ShareRecording(string lmsProviderName, string recordingId, bool isPublic, string password)
        {
            LmsCompany credentials = null;
            try
            {
                var session = this.GetSession(lmsProviderName);
                credentials = session.LmsCompany;
                var link = this.meetingSetup.UpdateRecording(credentials, this.GetAdobeConnectProvider(credentials), recordingId, isPublic, password);

                return Json(OperationResult.Success(link));
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("ShareRecording", credentials, ex);
                return Json(OperationResult.Error(errorMessage));
            }
        }
        
        [HttpGet]
        public virtual ActionResult EditRecording(string lmsProviderName, string recordingUrl)
        {
            LmsCompany credentials = null;
            try
            {
                var session = this.GetSession(lmsProviderName);
                credentials = session.LmsCompany;
                var param = session.LtiSession.With(x => x.LtiParam);
                var userSettings = this.GetLmsUserSettingsForJoin(lmsProviderName, credentials, param, session);
                var breezeSession = string.Empty;

                string url = this.meetingSetup.JoinRecording(credentials, param, userSettings, recordingUrl, ref breezeSession, "edit");
                return this.LoginToAC(url, breezeSession, credentials);
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("EditRecording", credentials, ex);
                return Json(OperationResult.Error(errorMessage), JsonRequestBehavior.AllowGet);
            }
        }
        
        [HttpGet]
        public virtual ActionResult GetRecordingFlv(string lmsProviderName, string recordingUrl)
        {
            LmsCompany credentials = null;
            try
            {
                var session = this.GetSession(lmsProviderName);
                credentials = session.LmsCompany;
                var param = session.LtiSession.With(x => x.LtiParam);
                var userSettings = this.GetLmsUserSettingsForJoin(lmsProviderName, credentials, param, session);
                var breezeSession = string.Empty;

                string url = this.meetingSetup.JoinRecording(credentials, param, userSettings, recordingUrl, ref breezeSession, "offline");
                return this.LoginToAC(url, breezeSession, credentials);
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("GetRecordingFlv", credentials, ex);
                return Json(OperationResult.Error(errorMessage), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public virtual JsonResult ConvertToMP4(string lmsProviderName, string recordingId, string meetingScoId)
        {
            LmsCompany credentials = null;
            try
            {
                var session = this.GetSession(lmsProviderName);
                credentials = session.LmsCompany;

                var adobeConnectProvider = this.GetAdobeConnectProvider(credentials);
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
                    return Json(this.GenerateErrorResult(recordingJob.Status));
                }

                var scheduledRecording = this.GetScheduledRecording(recordingJob.RecordingJob.ScoId, meetingScoId, adobeConnectProvider);
                if (scheduledRecording == null)
                {
                    throw new InvalidOperationException("Adobe connect provider. Cannot get scheduled recording");
                }

                var recording = new RecordingDTO(scheduledRecording, credentials.AcServer);

                return Json(OperationResult.Success(recording));
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("ConvertToMP4", credentials, ex);
                return Json(OperationResult.Error(errorMessage));
            }
        }

        [HttpPost]
        public virtual JsonResult CancelMP4Converting(string lmsProviderName, string recordingId, string meetingScoId)
        {
            LmsCompany credentials = null;
            try
            {
                var session = this.GetSession(lmsProviderName);
                credentials = session.LmsCompany;

                var adobeConnectProvider = this.GetAdobeConnectProvider(credentials);
                if (adobeConnectProvider == null)
                {
                    throw new InvalidOperationException("Adobe connect provider");
                }

                var recording = this.GetScheduledRecording(recordingId, meetingScoId, adobeConnectProvider);
                if (recording == null)
                {
                    return Json(OperationResult.Error("MP4 recording doesn't exist."));
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

                return Json(OperationResult.Error("Cannot delete. MP4 is already in progress."));

            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("GetRecordings", credentials, ex);
                return Json(OperationResult.Error(errorMessage));
            }
        }

        #endregion
        
        #region methods

        private Recording GetScheduledRecording(string recordingScoId, string meetingScoId, IAdobeConnectProxy adobeConnectProvider)
        {
            var recordingsByMeeting = adobeConnectProvider.GetRecordingsList(meetingScoId);
            if (recordingsByMeeting == null || !recordingsByMeeting.Success || recordingsByMeeting.Values == null || !recordingsByMeeting.Values.Any() )
            {
                return null;
            }

            return recordingsByMeeting.Values.SingleOrDefault(x => x.ScoId == recordingScoId);
        }

        private OperationResult GenerateErrorResult(StatusInfo status)
        {
            if (status.Code == StatusCodes.invalid && status.SubCode == StatusSubCodes.invalid_recording_job_in_progress)
            {
                return OperationResult.Error("Recording is already been converted to MP4.");
            }
            if (status.Code == StatusCodes.no_access && status.SubCode == StatusSubCodes.denied)
            {
                return OperationResult.Error("MP4 functionality is not enabled in Adobe Connect.");
            }
            if (status.Code == StatusCodes.invalid && status.SubCode == StatusSubCodes.duplicate)
            {
                return OperationResult.Error("Trying to create MP4 duplicate recording.");
            }

            return OperationResult.Error("Unexpected error");
        }

        #endregion

    }

}