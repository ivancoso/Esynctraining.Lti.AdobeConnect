using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using EdugameCloud.Lti.API.AdobeConnect;
using EdugameCloud.Lti.Core;
using EdugameCloud.Lti.Core.Business.Models;
using EdugameCloud.Lti.Core.Constants;
using EdugameCloud.Lti.Core.DTO;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.Extensions;
using Esynctraining.AC.Provider.Entities;
using Esynctraining.Core.Extensions;
using Esynctraining.Core.Logging;
using Esynctraining.Core.Providers;
using Esynctraining.Core.Utils;
using Esynctraining.Mp4Service.Tasks.Client;
using Esynctraining.WebApi.Client;
using MP4Service.Contract.Client;

namespace EdugameCloud.Lti.Controllers
{
    public partial class LtiRecordingController : BaseController
    {
        private IRecordingsService RecordingsService
        {
            get { return IoC.Resolve<IRecordingsService>(); }
        }

        private LmsCourseMeetingModel LmsCourseMeetingModel
        {
            get { return IoC.Resolve<LmsCourseMeetingModel>(); }
        }

        private UsersSetup UsersSetup
        {
            get { return IoC.Resolve<UsersSetup>(); }
        }

        public LtiRecordingController(
            LmsUserSessionModel userSessionModel,
            IAdobeConnectAccountService acAccountService,
            ApplicationSettingsProvider settings,
            ILogger logger)
            : base(userSessionModel, acAccountService, settings, logger)
        {
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

                if (!lmsCompany.CanRemoveRecordings)
                    throw new WarningMessageException("Recording deletion is not enabled for the LMS license");

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
        public virtual async Task<JsonResult> GetRecordings(string lmsProviderName, int meetingId)
        {
            LmsCompany lmsCompany = null;
            try
            {
                var session = GetReadOnlySession(lmsProviderName);
                lmsCompany = session.LmsCompany;
                var param = session.LtiSession.LtiParam;    

                IEnumerable<RecordingDTO> recordings = RecordingsService.GetRecordings(
                    lmsCompany,
                    this.GetAdobeConnectProvider(lmsCompany),
                    param.course_id,
                    meetingId);

                if (!UsersSetup.IsTeacher(param) && !lmsCompany.AutoPublishRecordings)
                {
                    recordings = recordings.Where(x => x.published);
                }

                string mp4LicenseKey = lmsCompany.GetSetting<string>(LmsCompanySettingNames.Mp4ServiceLicenseKey);
                string mp4WithSubtitlesLicenseKey = lmsCompany.GetSetting<string>(LmsCompanySettingNames.Mp4ServiceWithSubtitlesLicenseKey);
                if (!string.IsNullOrWhiteSpace(mp4LicenseKey) || !string.IsNullOrWhiteSpace(mp4WithSubtitlesLicenseKey))
                {
                    // TODO: call MP$ serive
                    var mp4Tasks = new Dictionary<string, MP4Service.Contract.Client.DataTask>();
                    foreach (var recordingScoId in recordings.Where(x => !x.is_mp4).Select(x => x.id))
                    {
                        mp4Tasks.Add(recordingScoId, new MP4Service.Contract.Client.DataTask());
                    }

                    var mp4 = new ConcurrentDictionary<string, MP4Service.Contract.Client.DataTask>(mp4Tasks);

                    if (!string.IsNullOrWhiteSpace(mp4LicenseKey))
                    {
                        foreach (var recording in mp4)
                        //Parallel.ForEach(mp4, (recording) =>
                        {
                            var mp4Client = IoC.Resolve<Mp4ServiceTaskClient>();
                            try
                            {
                                var status = await mp4Client.GetStatus(new MP4Service.Contract.Client.TaskParam
                                {
                                    LicenseId = mp4LicenseKey,
                                    ScoId = long.Parse(recording.Key),
                                }).ConfigureAwait(false);

                                if (status != null)
                                {
                                    recording.Value.Id = status.Id;
                                    recording.Value.ScoId = status.ScoId;
                                    recording.Value.UploadScoId = status.UploadScoId;
                                    recording.Value.LicenseId = status.LicenseId;
                                    recording.Value.Modified = status.Modified;
                                    recording.Value.Duration = status.Duration;
                                    recording.Value.Status = status.Status;
                                    recording.Value.TranscriptScoId = status.TranscriptScoId;
                                }
                            }
                            catch (AggregateException ex)
                            {
                                // TRICK: for error handling
                                recording.Value.Duration =-777;
                            }
                            catch (ApiException ex)
                            {
                                // TRICK: for error handling
                                recording.Value.Duration = -777;
                            }

                        }//);
                    }

                    if (!string.IsNullOrWhiteSpace(mp4WithSubtitlesLicenseKey))
                    {
                        foreach (var recording in mp4)
                        //Parallel.ForEach(mp4, (recording) =>
                        {
                            try
                            {
                                var mp4Client = IoC.Resolve<Mp4ServiceTaskClient>();
                                var status = await mp4Client.GetStatus(new MP4Service.Contract.Client.TaskParam
                                {
                                    LicenseId = mp4WithSubtitlesLicenseKey,
                                    ScoId = long.Parse(recording.Key),
                                }).ConfigureAwait(false);

                                if (status != null)
                                {
                                    recording.Value.Id = status.Id;
                                    recording.Value.ScoId = status.ScoId;
                                    recording.Value.UploadScoId = status.UploadScoId;
                                    recording.Value.LicenseId = status.LicenseId;
                                    recording.Value.Modified = status.Modified;
                                    recording.Value.Duration = status.Duration;
                                    recording.Value.Status = status.Status;
                                    recording.Value.TranscriptScoId = status.TranscriptScoId;
                                }
                            }
                            catch (AggregateException ex)
                            {
                                // TRICK: for error handling
                                recording.Value.Duration = -777;
                            }
                            catch (ApiException ex)
                            {
                                // TRICK: for error handling
                                recording.Value.Duration = -777;
                            }
                        }//);
                    }

                    //foreach (var item in recordings)
                    //{
                    //    item.mp4 = new Mp4ServiceStatusDto
                    //    {
                    //        mp4_sco_id = "40297",
                    //        cc_sco_id = "40345",
                    //        status = "Transcripted",
                    //    };
                    //}

                    foreach (var item in mp4)
                    {
                        if (string.IsNullOrEmpty(item.Value.ScoId))
                            continue;

                        var recording = recordings.FirstOrDefault(x => x.id == item.Key);
                        recording.mp4 = new Mp4ServiceStatusDto()
                        {
                            mp4_sco_id = (item.Value.Status >= MP4Service.Contract.Client.TaskStatus.Uploaded) ? item.Value.UploadScoId : null,
                            cc_sco_id = (item.Value.Status >= MP4Service.Contract.Client.TaskStatus.Transcripted) ? item.Value.TranscriptScoId : null,
                            // TRICK:
                            status = item.Value.Duration == -777 ? "MP4 Service Error" : item.Value.Status.ToString(),

                            lmsProviderName = lmsProviderName,
                        };
                    }

                }

                return Json(OperationResult.Success(recordings));
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("GetRecordings", lmsCompany, ex);
                return Json(OperationResult.Error(errorMessage));
            }
        }

        [HttpPost]
        public virtual JsonResult PublishRecording(string lmsProviderName, string recordingId, int meetingId)
        {
            LmsCompany lmsCompany = null;
            try
            {
                var session = GetReadOnlySession(lmsProviderName);
                lmsCompany = session.LmsCompany;

                if (lmsCompany.AutoPublishRecordings)
                    throw new WarningMessageException("Publishing is not allowed by LMS license settings");
                
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

        public virtual JsonResult UnpublishRecording(string lmsProviderName, string recordingId, int meetingId)
        {
            LmsCompany lmsCompany = null;
            try
            {
                var session = GetReadOnlySession(lmsProviderName);
                lmsCompany = session.LmsCompany;

                if (lmsCompany.AutoPublishRecordings)
                    throw new WarningMessageException("UnPublishing is not allowed by LMS license settings");

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