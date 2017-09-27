using System;
using System.Linq;
using EdugameCloud.Lti.Api.Filters;
using EdugameCloud.Lti.Api.Models;
using EdugameCloud.Lti.API.AdobeConnect;
using EdugameCloud.Lti.Core.Business.Models;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.DTO;
using EdugameCloud.Lti.DTO.Recordings;
using Esynctraining.AdobeConnect;
using Esynctraining.Core.Caching;
using Esynctraining.Core.Domain;
using Esynctraining.Core.Logging;
using Esynctraining.Core.Providers;
using Esynctraining.Core.Utils;
using Microsoft.AspNetCore.Mvc;

namespace EdugameCloud.Lti.Api.Controllers
{
    [Route("recordings")]
    public partial class RecordingsController : BaseApiController
    {
        private IRecordingsService RecordingsService => IoC.Resolve<IRecordingsService>();

        private LmsCourseMeetingModel LmsCourseMeetingModel => IoC.Resolve<LmsCourseMeetingModel>();

        private UsersSetup UsersSetup => IoC.Resolve<UsersSetup>();


        public RecordingsController(
            API.AdobeConnect.IAdobeConnectAccountService acAccountService,
            ApplicationSettingsProvider settings,
            ILogger logger, ICache cache)
            : base(acAccountService, settings, logger, cache)
        {
        }

        // TODO: remove type - we fetch meetingitem within RecordingsService.GetRecordings
        // we can reuse that info to have type (change API)
        [HttpPost]
        [Route("")]
        //[ResponseType(typeof(IEnumerable<Country>))]
        [LmsAuthorizeBase(ApiCallEnabled = true)]
        public OperationResultWithData<PagedResult<IRecordingDto>> GetRecordings([FromBody]TypeMeetingRequestDto request)
        {
            try
            {
                var ac = GetAdminProvider();

                Func<IRoomTypeFactory> getRoomTypeFactory =
                    () => new RoomTypeFactory(ac, (LmsMeetingType)request.Type, IoC.Resolve<API.AdobeConnect.ISeminarService>());

                var publishOnly = !LmsCompany.AutoPublishRecordings && !UsersSetup.IsTeacher(Session.LtiSession.LtiParam, LmsCompany);

                var recordings = RecordingsService.GetRecordings(
                    LmsCompany,
                    ac,
                    CourseId,
                    request.MeetingId,
                    getRoomTypeFactory,
                    request.SortBy,
                    request.SortOder,
                    request.Search,
                    request.DateFrom,
                    request.DateTo,
                    (input) =>
                    {
                        // TRICK: for API UNIR uses AutoPublishRecordings==true; So no access to Session for them.
                        if (publishOnly)
                        {
                            return input.Where(x => x.Published);
                        }

                        return input;
                    },
                    request.Skip,
                    request.Take);

                return recordings.ToSuccessResult();
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("GetRecordings", ex);
                return OperationResultWithData<PagedResult<IRecordingDto>>.Error(errorMessage);
            }
        }


        // TODO: create DTO with validation!!
        [Route("edit")]
        [HttpPost]
        [LmsAuthorizeBase]
        public virtual OperationResult EditRecording([FromBody]EditDto dto)
        {
            try
            {
                OperationResult result = RecordingsService.EditRecording(
                    LmsCompany,
                    this.GetAdminProvider(),
                    CourseId,
                    dto.recordingId,
                    dto.meetingId,
                    dto.name,
                    dto.summary);

                return result;
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("EditRecording", ex);
                return OperationResult.Error(errorMessage);
            }
        }

        [Route("delete")]
        [HttpPost]
        [LmsAuthorizeBase]
        public virtual OperationResult DeleteRecording([FromBody]RecordingRequestDto request)
        {
            try
            {
                // TODO : FeatureName - but default value is true here!
                if (!LmsCompany.CanRemoveRecordings)
                    throw new Core.WarningMessageException("Recording deletion is not enabled for the LMS license");

                var param = Session.LtiSession.LtiParam;
                OperationResult result = RecordingsService.RemoveRecording(
                    LmsCompany,
                    this.GetAdminProvider(),
                    param.course_id,
                    request.RecordingId,
                    request.MeetingId);

                return result;
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("DeleteRecording", ex);
                return OperationResult.Error(errorMessage);
            }
        }
        
        [Route("publish")]
        [HttpPost]
        [LmsAuthorizeBase]
        public OperationResult PublishRecording([FromBody]RecordingRequestDto request)
        {
            try
            {
                // TODO : FeatureName - but default value is true here!
                if (LmsCompany.AutoPublishRecordings)
                    throw new Core.WarningMessageException("Publishing is not allowed by LMS license settings");

                LmsCourseMeeting meeting = this.LmsCourseMeetingModel.GetOneByCourseAndId(LmsCompany.Id, CourseId, request.MeetingId);
                var recording = new LmsCourseMeetingRecording
                {
                    LmsCourseMeeting = meeting,
                    ScoId = request.RecordingId,
                };
                meeting.MeetingRecordings.Add(recording);
                LmsCourseMeetingModel.RegisterSave(meeting, flush: true);

                return OperationResult.Success();
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("PublishRecording", ex);
                return OperationResult.Error(errorMessage);
            }
        }

        [Route("unpublish")]
        [HttpPost]
        [LmsAuthorizeBase]
        public OperationResult UnpublishRecording([FromBody]RecordingRequestDto request)
        {
            try
            {
                // TODO : FeatureName - but default value is true here!
                if (LmsCompany.AutoPublishRecordings)
                    throw new Core.WarningMessageException("UnPublishing is not allowed by LMS license settings");

                LmsCourseMeeting meeting = this.LmsCourseMeetingModel.GetOneByCourseAndId(LmsCompany.Id, CourseId, request.MeetingId);
                var recording = meeting.MeetingRecordings.FirstOrDefault(x => x.ScoId == request.RecordingId);
                if (recording != null)
                {
                    meeting.MeetingRecordings.Remove(recording);
                    LmsCourseMeetingModel.RegisterSave(meeting, flush: true);
                }

                return OperationResult.Success();
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("PublishRecording", ex);
                return OperationResult.Error(errorMessage);
            }
        }


        //[HttpGet]
        //public virtual ActionResult JoinRecording(string lmsProviderName, string recordingUrl)
        //{
        //    LmsCompany lmsCompany = null;
        //    try
        //    {
        //        var session = GetReadOnlySession(lmsProviderName);
        //        lmsCompany = session.LmsCompany;
        //        var param = session.LtiSession.With(x => x.LtiParam);
        //        var breezeSession = string.Empty;
        //        var provider = GetAdminProvider(lmsCompany);
        //        string url = RecordingsService.JoinRecording(lmsCompany, param, recordingUrl, ref breezeSession, provider);
        //        return this.LoginToAC(url, breezeSession, lmsCompany);
        //    }
        //    catch (Exception ex)
        //    {
        //        return RecordingsError("JoinRecording", lmsProviderName, ex);
        //    }
        //}

        [Route("share")]
        [HttpPost]
        [LmsAuthorizeBase]
        public virtual OperationResultWithData<string> ShareRecording([FromBody]ShareRecordingRequestDto request)
        {
            try
            {
                string link = RecordingsService.UpdateRecording(LmsCompany, GetAdminProvider(), request.RecordingId, request.IsPublic, request.Password);

                return link.ToSuccessResult();
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("ShareRecording", ex);
                return OperationResultWithData<string>.Error(errorMessage);
            }
        }

        [Route("passcode")]
        [HttpPost]
        public virtual OperationResultWithData<string> GetRecordingPasscode([FromBody]RecordingRequestDto input)
        {
            try
            {
                string passCode = RecordingsService.GetPasscode(LmsCompany, input.RecordingId);
                return passCode.ToSuccessResult();
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("MP4-GetPasscodeRecordings", ex);
                return OperationResultWithData<string>.Error(errorMessage);
            }
        }

        //[HttpGet]
        //public virtual ActionResult EditRecording(string lmsProviderName, string recordingUrl)
        //{
        //    LmsCompany lmsCompany = null;
        //    try
        //    {
        //        var session = GetReadOnlySession(lmsProviderName);
        //        lmsCompany = session.LmsCompany;
        //        var param = session.LtiSession.With(x => x.LtiParam);
        //        var breezeSession = string.Empty;
        //        var provider = GetAdminProvider(lmsCompany);

        //        string url = RecordingsService.JoinRecording(lmsCompany, param, recordingUrl, ref breezeSession, provider, "edit");
        //        return this.LoginToAC(url, breezeSession, lmsCompany);
        //    }
        //    catch (Exception ex)
        //    {
        //        return RecordingsError("EditRecording", lmsProviderName, ex);
        //    }
        //}

        //[HttpGet]
        //public virtual ActionResult GetRecordingFlv(string lmsProviderName, string recordingUrl)
        //{
        //    LmsCompany lmsCompany = null;
        //    try
        //    {
        //        var session = GetReadOnlySession(lmsProviderName);
        //        lmsCompany = session.LmsCompany;
        //        var param = session.LtiSession.With(x => x.LtiParam);
        //        var breezeSession = string.Empty;
        //        var provider = GetAdminProvider(lmsCompany);

        //        string url = RecordingsService.JoinRecording(lmsCompany, param, recordingUrl, ref breezeSession, provider, "offline");
        //        return this.LoginToAC(url, breezeSession, lmsCompany);
        //    }
        //    catch (Exception ex)
        //    {
        //        return RecordingsError("GetRecordingFlv", lmsProviderName, ex);
        //    }
        //}

    }

}