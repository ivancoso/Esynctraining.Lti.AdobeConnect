using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using EdugameCloud.Lti.API.AdobeConnect;
using EdugameCloud.Lti.Core.Business.Models;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.DTO.Recordings;
using Esynctraining.AC.Provider.Entities;
using Esynctraining.AdobeConnect;
using Esynctraining.Core.Caching;
using Esynctraining.Core.Domain;
using Esynctraining.Core.Extensions;
using Esynctraining.Core.Logging;
using Esynctraining.Core.Providers;
using Esynctraining.Core.Utils;

namespace EdugameCloud.Lti.Controllers
{
    public partial class LtiRecordingController : BaseController
    {
        private IRecordingsService RecordingsService => IoC.Resolve<IRecordingsService>();

        private LmsCourseMeetingModel LmsCourseMeetingModel => IoC.Resolve<LmsCourseMeetingModel>();

        private UsersSetup UsersSetup => IoC.Resolve<UsersSetup>();


        public LtiRecordingController(
            LmsUserSessionModel userSessionModel,
            API.AdobeConnect.IAdobeConnectAccountService acAccountService,
            ApplicationSettingsProvider settings,
            ILogger logger, ICache cache)
            : base(userSessionModel, acAccountService, settings, logger, cache)
        {
        }

        #region Public Methods and Operators

        // TODO: create DTO with validation!!
        [HttpPost]
        public virtual JsonResult EditRecording(EditDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            LmsCompany lmsCompany = null;
            try
            {
                // TRICK: 1st to enable culture for thread
                var session = GetReadOnlySession(dto.lmsProviderName);
                
                ///
                /// TODO: REUSE!!!!
                ///
                foreach (var key in ModelState.Keys.ToList().Where(key => ModelState.ContainsKey(key)))
                {
                    ModelState[key].Errors.Clear();
                }
                TryValidateModel(dto);
                if (!ModelState.IsValid)
                {
                    var errorMessage = new StringBuilder();
                    if (ModelState != null)
                    {
                        foreach (var msgSet in ModelState.Values)
                            foreach (var msg in msgSet.Errors)
                            {
                                string txt = msg.ErrorMessage;
                                if (!string.IsNullOrWhiteSpace(txt) && txt.Contains("#_#"))
                                {
                                    var errorDetails = txt.Split(new[] { "#_#" }, StringSplitOptions.RemoveEmptyEntries);
                                    int errorCode;
                                    if (errorDetails.FirstOrDefault() == null || !int.TryParse(errorDetails.FirstOrDefault(), out errorCode))
                                    {
                                        txt = errorDetails.FirstOrDefault();
                                    }
                                    else
                                    {
                                        txt = errorDetails.ElementAtOrDefault(1);
                                    }
                                }

                                errorMessage.Append(txt);
                                errorMessage.Append(" ");
                            }
                    }

                    return Json(OperationResult.Error(errorMessage.ToString()));
                }
                
                lmsCompany = session.LmsCompany;
                var param = session.LtiSession.With(x => x.LtiParam);

                OperationResult result = RecordingsService.EditRecording(
                    lmsCompany,
                    this.GetAdminProvider(lmsCompany),
                    param.course_id,
                    dto.id,
                    dto.meetingId,
                    dto.name,
                    dto.summary);

                return Json(result);
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("EditRecording", lmsCompany, ex);
                return Json(OperationResult.Error(errorMessage));
            }
        }

        // TODO: id -> recordingId
        [HttpPost]
        public virtual JsonResult DeleteRecording(string lmsProviderName, int meetingId, string id)
        {
            LmsCompany lmsCompany = null;
            try
            {
                var session = GetReadOnlySession(lmsProviderName);
                lmsCompany = session.LmsCompany;
                var param = session.LtiSession.With(x => x.LtiParam);

                if (!lmsCompany.CanRemoveRecordings)
                    throw new Core.WarningMessageException("Recording deletion is not enabled for the LMS license");

                OperationResult result = RecordingsService.RemoveRecording(
                    lmsCompany,
                    this.GetAdminProvider(lmsCompany),
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

        // TODO: remove type - we fetch meetingitem within RecordingsService.GetRecordings
        // we can reuse that info to have type (change API)
        [HttpPost]
        public virtual JsonResult GetRecordings(string lmsProviderName, int meetingId, int type)
        {
            LmsCompany lmsCompany = null;
            try
            {
                var session = GetReadOnlySession(lmsProviderName);
                lmsCompany = session.LmsCompany;
                var param = session.LtiSession.LtiParam;
                var ac = this.GetAdminProvider(lmsCompany);

                Func<IRoomTypeFactory> getRoomTypeFactory =
                    () => new RoomTypeFactory(ac, (LmsMeetingType)type, IoC.Resolve<API.AdobeConnect.ISeminarService>());

                IEnumerable<IRecordingDto> recordings = RecordingsService.GetRecordings(
                    lmsCompany,
                    ac,
                    param.course_id,
                    meetingId,
                    getRoomTypeFactory);

                if (!UsersSetup.IsTeacher(param) && !lmsCompany.AutoPublishRecordings)
                {
                    recordings = recordings.Where(x => x.Published);
                }
                
                return Json(recordings.ToSuccessResult());
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("GetRecordings", lmsCompany, ex);
                return Json(OperationResult.Error(errorMessage));
            }
        }

        [HttpPost]
        public virtual JsonResult PublishRecording(string lmsProviderName, int meetingId, string recordingId)
        {
            LmsCompany lmsCompany = null;
            try
            {
                var session = GetReadOnlySession(lmsProviderName);
                lmsCompany = session.LmsCompany;

                if (lmsCompany.AutoPublishRecordings)
                    throw new Core.WarningMessageException("Publishing is not allowed by LMS license settings");
                
                LmsCourseMeeting meeting = this.LmsCourseMeetingModel.GetOneByCourseAndId(lmsCompany.Id, session.LtiSession.LtiParam.course_id, meetingId);
                var recording = new LmsCourseMeetingRecording
                {
                    LmsCourseMeeting = meeting,
                    ScoId = recordingId,
                };
                meeting.MeetingRecordings.Add(recording);
                LmsCourseMeetingModel.RegisterSave(meeting, flush: true);

                return Json(OperationResult.Success());
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("PublishRecording", lmsCompany, ex);
                return Json(OperationResult.Error(errorMessage));
            }
        }

        public virtual JsonResult UnpublishRecording(string lmsProviderName, int meetingId, string recordingId)
        {
            LmsCompany lmsCompany = null;
            try
            {
                var session = GetReadOnlySession(lmsProviderName);
                lmsCompany = session.LmsCompany;

                if (lmsCompany.AutoPublishRecordings)
                    throw new Core.WarningMessageException("UnPublishing is not allowed by LMS license settings");

                LmsCourseMeeting meeting = this.LmsCourseMeetingModel.GetOneByCourseAndId(lmsCompany.Id, session.LtiSession.LtiParam.course_id, meetingId);
                var recording = meeting.MeetingRecordings.FirstOrDefault(x => x.ScoId == recordingId);
                if (recording != null)
                {
                    meeting.MeetingRecordings.Remove(recording);
                    LmsCourseMeetingModel.RegisterSave(meeting, flush: true);
                }
                
                return Json(OperationResult.Success());
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("PublishRecording", lmsCompany, ex);
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
                var provider = GetAdminProvider(lmsCompany);
                string url = RecordingsService.JoinRecording(lmsCompany, param, recordingUrl, ref breezeSession, provider);
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
                string link = RecordingsService.UpdateRecording(lmsCompany, this.GetAdminProvider(lmsCompany), recordingId, isPublic, password);

                return Json(link.ToSuccessResult());
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
                var provider = GetAdminProvider(lmsCompany);

                string url = RecordingsService.JoinRecording(lmsCompany, param, recordingUrl, ref breezeSession, provider, "edit");
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
                var provider = GetAdminProvider(lmsCompany);

                string url = RecordingsService.JoinRecording(lmsCompany, param, recordingUrl, ref breezeSession, provider, "offline");
                return this.LoginToAC(url, breezeSession, lmsCompany);
            }
            catch (Exception ex)
            {
                return RecordingsError("GetRecordingFlv", lmsProviderName, ex);
            }
        }

        //[HttpPost]
        //public virtual JsonResult ConvertToMP4(string lmsProviderName, int meetingId, string recordingId)
        //{
        //    LmsCompany lmsCompany = null;
        //    try
        //    {
        //        var session = GetReadOnlySession(lmsProviderName);
        //        lmsCompany = session.LmsCompany;

        //        var adobeConnectProvider = this.GetAdobeConnectProvider(lmsCompany);
        //        if (adobeConnectProvider == null)
        //        {   
        //            throw new InvalidOperationException("Adobe connect provider");
        //        }

        //        var recordingJob = adobeConnectProvider.ScheduleRecordingJob(recordingId);
        //        if (recordingJob == null)
        //        {
        //            throw new InvalidOperationException("Adobe connect provider. Cannot get recording job.");
        //        }

        //        if (!recordingJob.Success)
        //        {
        //            return Json(GenerateErrorResult(recordingJob.Status));
        //        }

        //        LmsCourseMeeting meeting = this.LmsCourseMeetingModel.GetOneByCourseAndId(lmsCompany.Id, session.LtiSession.LtiParam.course_id, meetingId);
        //        var scheduledRecording = GetScheduledRecording(recordingJob.RecordingJob.ScoId, meeting.GetMeetingScoId(), adobeConnectProvider);
        //        if (scheduledRecording == null)
        //        {
        //            throw new InvalidOperationException("Adobe connect provider. Cannot get scheduled recording");
        //        }

        //        var timeZone = acAccountService.GetAccountDetails(adobeConnectProvider, IoC.Resolve<ICache>()).TimeZoneInfo;
        //        var recording = new RecordingDto(scheduledRecording, lmsCompany.AcServer, timeZone);

        //        return Json(recording.ToSuccessResult());
        //    }
        //    catch (Exception ex)
        //    {
        //        string errorMessage = GetOutputErrorMessage("ConvertToMP4", lmsCompany, ex);
        //        return Json(OperationResult.Error(errorMessage));
        //    }
        //}

        //[HttpPost]
        //public virtual JsonResult CancelMP4Converting(string lmsProviderName, int meetingId, string recordingId)
        //{
        //    LmsCompany lmsCompany = null;
        //    try
        //    {
        //        var session = GetReadOnlySession(lmsProviderName);
        //        lmsCompany = session.LmsCompany;

        //        var adobeConnectProvider = this.GetAdobeConnectProvider(lmsCompany);
        //        if (adobeConnectProvider == null)
        //        {
        //            throw new InvalidOperationException("Adobe connect provider");
        //        }

        //        LmsCourseMeeting meeting = this.LmsCourseMeetingModel.GetOneByCourseAndId(lmsCompany.Id, session.LtiSession.LtiParam.course_id, meetingId);
        //        var recording = GetScheduledRecording(recordingId, meeting.GetMeetingScoId(), adobeConnectProvider);
        //        if (recording == null)
        //        {
        //            return Json(OperationResult.Error(Resources.Messages.RecordingMissedMP4));
        //        }

        //        if (recording.JobStatus == "job-queued")
        //        {
        //            var recordingJob = adobeConnectProvider.CancelRecordingJob(recordingId);
        //            if (recordingJob == null)
        //            {
        //                throw new InvalidOperationException("Adobe connect provider");
        //            }

        //            return Json(OperationResult.Success());                    
        //        }

        //        return Json(OperationResult.Error(Resources.Messages.RecordingCantDeleteMP4Progress));

        //    }
        //    catch (Exception ex)
        //    {
        //        string errorMessage = GetOutputErrorMessage("GetRecordings", lmsCompany, ex);
        //        return Json(OperationResult.Error(errorMessage));
        //    }
        //}

        #endregion

        #region methods

        private ActionResult LoginToAC(string realUrl, string breezeSession, LmsCompany credentials)
        {
            if (!credentials.LoginUsingCookie.GetValueOrDefault())
            {
                return this.Redirect(realUrl);
            }

            this.ViewBag.MeetingUrl = realUrl;
            this.ViewBag.BreezeSession = breezeSession;
            this.ViewBag.AcServer = credentials.AcServer + "/";

            return this.View("~/Views/Lti/LoginToAC.cshtml");
        }

        //private static Recording GetScheduledRecording(string recordingScoId, string meetingScoId, Esynctraining.AdobeConnect.IAdobeConnectProxy adobeConnectProvider)
        //{
        //    var recordingsByMeeting = adobeConnectProvider.GetRecordingsList(meetingScoId);
        //    if (recordingsByMeeting == null || !recordingsByMeeting.Success || recordingsByMeeting.Values == null || !recordingsByMeeting.Values.Any() )
        //    {
        //        return null;
        //    }

        //    return recordingsByMeeting.Values.SingleOrDefault(x => x.ScoId == recordingScoId);
        //}

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
            Logger.ErrorFormat(ex, "{0} exception. sessionId:{1}.", method, sessionId);
            this.ViewBag.DebugError = IsDebug ? (ex.Message + ex.StackTrace) : string.Empty;
            return this.View("~/Views/Lti/LtiError.cshtml");
        }

        #endregion

    }

}