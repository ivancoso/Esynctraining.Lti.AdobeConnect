using System.Linq;
using Esynctraining.AC.Provider;
using Esynctraining.AC.Provider.Entities;
using NHibernate.Linq.Functions;
using Remotion.Linq.Parsing;

namespace EdugameCloud.Lti.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using EdugameCloud.Lti.DTO;
    using Esynctraining.Core.Extensions;

    /// <summary>
    ///     The LTI controller.
    /// </summary>
    public partial class LtiController
    {
        #region Public Methods and Operators

        /// <summary>
        /// The delete recording.
        /// </summary>
        /// <param name="lmsProviderName">
        /// The LMS Provider Name.
        /// </param>
        /// <param name="scoId">
        /// The SCO Id.
        /// </param>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="JsonResult"/>.
        /// </returns>
        [HttpPost]
        public virtual JsonResult DeleteRecording(string lmsProviderName, string scoId, string id)
        {
            try
            {
                var session = this.GetSession(lmsProviderName);
                var credentials = session.LmsCompany;
                var param = session.LtiSession.With(x => x.LtiParam);

                OperationResult result = this.meetingSetup.RemoveRecording(
                    credentials,
                    this.GetAdobeConnectProvider(credentials),
                    param.course_id,
                    id,
                    scoId);

                return Json(result);
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("DeleteRecording", ex);
                return Json(OperationResult.Error(errorMessage));
            }
        }
        
        /// <summary>
        /// The get recordings.
        /// </summary>
        /// <param name="lmsProviderName">
        /// The LMS Provider Name.
        /// </param>
        /// <param name="scoId">
        /// The SCO Id.
        /// </param>
        /// <returns>
        /// The <see cref="JsonResult"/>.
        /// </returns>
        [HttpPost]
        public virtual JsonResult GetRecordings(string lmsProviderName, string scoId)
        {
            try
            {
                var session = this.GetSession(lmsProviderName);
                var credentials = session.LmsCompany;
                var param = session.LtiSession.LtiParam;    

                List<RecordingDTO> recordings = this.meetingSetup.GetRecordings(
                    credentials,
                    this.GetAdobeConnectProvider(credentials),
                    param.course_id,
                    scoId);

                return Json(OperationResult.Success(recordings));
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("GetRecordings", ex);
                return Json(OperationResult.Error(errorMessage));
            }
        }

        /// <summary>
        /// The join recording.
        /// </summary>
        /// <param name="lmsProviderName">
        /// The LMS Provider Name.
        /// </param>
        /// <param name="recordingUrl">
        /// The recording url.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        [HttpGet]
        public virtual ActionResult JoinRecording(string lmsProviderName, string recordingUrl)
        {
            try
            {
                var session = this.GetSession(lmsProviderName);
                var credentials = session.LmsCompany;
                var param = session.LtiSession.With(x => x.LtiParam);
                var userSettings = this.GetLmsUserSettingsForJoin(lmsProviderName, credentials, param, session);
                var breezeSession = string.Empty;

                string url = this.meetingSetup.JoinRecording(credentials, param, userSettings, recordingUrl, ref breezeSession);
                return this.LoginToAC(url, breezeSession, credentials);
            }
            catch (WarningMessageException ex)
            {
                return Json(OperationResult.Error(ex.Message), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("JoinRecording", ex);
                return Json(OperationResult.Error(errorMessage), JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// The share recording.
        /// </summary>
        /// <param name="lmsProviderName">
        /// The LMS provider name.
        /// </param>
        /// <param name="recordingId">
        /// The recording id.
        /// </param>
        /// <param name="isPublic">
        /// The is public.
        /// </param>
        /// <param name="password">
        /// The password.
        /// </param>
        /// <returns>
        /// The <see cref="JsonResult"/>.
        /// </returns>
        [HttpPost]
        public virtual ActionResult ShareRecording(string lmsProviderName, string recordingId, bool isPublic, string password)
        {
            try
            {
                var session = this.GetSession(lmsProviderName);
                var credentials = session.LmsCompany;
                var link = this.meetingSetup.UpdateRecording(credentials, this.GetAdobeConnectProvider(credentials), recordingId, isPublic, password);

                return Json(OperationResult.Success(link));
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("ShareRecording", ex);
                return Json(OperationResult.Error(errorMessage));
            }
        }

        /// <summary>
        /// The edit recording.
        /// </summary>
        /// <param name="lmsProviderName">
        /// The LMS provider name.
        /// </param>
        /// <param name="recordingUrl">
        /// The recording url.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        [HttpGet]
        public virtual ActionResult EditRecording(string lmsProviderName, string recordingUrl)
        {
            try
            {
                var session = this.GetSession(lmsProviderName);
                var credentials = session.LmsCompany;
                var param = session.LtiSession.With(x => x.LtiParam);
                var userSettings = this.GetLmsUserSettingsForJoin(lmsProviderName, credentials, param, session);
                var breezeSession = string.Empty;

                string url = this.meetingSetup.JoinRecording(credentials, param, userSettings, recordingUrl, ref breezeSession, "edit");
                return this.LoginToAC(url, breezeSession, credentials);
            }
            catch (WarningMessageException ex)
            {
                return Json(OperationResult.Error(ex.Message), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("EditRecording", ex);
                return Json(OperationResult.Error(errorMessage), JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// The get recording FLV.
        /// </summary>
        /// <param name="lmsProviderName">
        /// The LMS provider name.
        /// </param>
        /// <param name="recordingUrl">
        /// The recording url.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        [HttpGet]
        public virtual ActionResult GetRecordingFlv(string lmsProviderName, string recordingUrl)
        {
            try
            {
                var session = this.GetSession(lmsProviderName);
                var credentials = session.LmsCompany;
                var param = session.LtiSession.With(x => x.LtiParam);
                var userSettings = this.GetLmsUserSettingsForJoin(lmsProviderName, credentials, param, session);
                var breezeSession = string.Empty;

                string url = this.meetingSetup.JoinRecording(credentials, param, userSettings, recordingUrl, ref breezeSession, "offline");
                return this.LoginToAC(url, breezeSession, credentials);
            }
            catch (WarningMessageException ex)
            {
                return Json(OperationResult.Error(ex.Message), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("GetRecordingFlv", ex);
                return Json(OperationResult.Error(errorMessage), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public virtual JsonResult ConvertToMP4(string lmsProviderName, string recordingId, string meetingScoId)
        {
            try
            {
                var session = this.GetSession(lmsProviderName);
                var credentials = session.LmsCompany;

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
                string errorMessage = GetOutputErrorMessage("GetRecordings", ex);
                return Json(OperationResult.Error(errorMessage));
            }
        }

        [HttpPost]
        public virtual JsonResult CancelMP4Converting(string lmsProviderName, string recordingId, string meetingScoId)
        {
            try
            {
                var session = this.GetSession(lmsProviderName);
                var credentials = session.LmsCompany;

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

                if (!string.IsNullOrEmpty(recording.EncoderServiceJobStatus) && recording.EncoderServiceJobStatus == "WORKING")
                {
                    return Json(OperationResult.Error("MP4 converting is already in progress. Try to delete recording after converting."));
                }

                var recordingJob = adobeConnectProvider.CancelRecordingJob(recordingId);

                if (recordingJob == null)
                {
                    throw new InvalidOperationException("Adobe connect provider");
                }

                return Json(OperationResult.Success());
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("GetRecordings", ex);
                return Json(OperationResult.Error(errorMessage));
            }
        }

        #endregion
        #region methods

        private Recording GetScheduledRecording(string recordingScoId, string meetingScoId, AdobeConnectProvider adobeConnectProvider)
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