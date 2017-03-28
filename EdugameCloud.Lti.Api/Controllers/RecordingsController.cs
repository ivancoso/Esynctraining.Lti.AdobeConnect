using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
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
        // TODO: remove type - we fetch meetingitem within RecordingsService.GetRecordings
        // we can reuse that info to have type (change API)
        [DataContract]
        public class TypeMeetingRequestDto : MeetingRequestDto
        {
            [Required]
            [DataMember]
            public int Type { get; set; }

        }

        [DataContract]
        public class ShareRecordingRequestDto : RequestDto
        {
            [Required]
            [DataMember]
            public string RecordingId { get; set; }

            [Required]
            [DataMember]
            public bool IsPublic { get; set; }

            // TODO: is it used??
            [DataMember]
            public string Password { get; set; }

        }

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
        [EdugameCloud.Lti.Api.Filters.LmsAuthorizeBase(ApiCallEnabled = true)]
        public OperationResultWithData<IEnumerable<IRecordingDto>> GetRecordings([FromBody]TypeMeetingRequestDto request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            try
            {
                var ac = GetAdminProvider();

                Func<IRoomTypeFactory> getRoomTypeFactory =
                    () => new RoomTypeFactory(ac, (LmsMeetingType)request.Type, IoC.Resolve<API.AdobeConnect.ISeminarService>());

                IEnumerable<IRecordingDto> recordings = RecordingsService.GetRecordings(
                    LmsCompany,
                    ac,
                    CourseId,
                    request.MeetingId,
                    getRoomTypeFactory);

                // TRICK: for API UNIR uses AutoPublishRecordings==true; So no access to Session for them.
                if (!LmsCompany.AutoPublishRecordings && !UsersSetup.IsTeacher(Session.LtiSession.LtiParam))
                {
                    recordings = recordings.Where(x => x.Published);
                }

                return recordings.ToSuccessResult();
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("GetRecordings", ex);
                return OperationResultWithData<IEnumerable<IRecordingDto>>.Error(errorMessage);
            }
        }


        // TODO: create DTO with validation!!
        [Route("edit")]
        [HttpPost]
        [EdugameCloud.Lti.Api.Filters.LmsAuthorizeBase]
        public virtual OperationResult EditRecording([FromBody]EditDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            try
            {
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

                    return OperationResult.Error(errorMessage.ToString());
                }

                var param = Session.LtiSession.LtiParam;

                OperationResult result = RecordingsService.EditRecording(
                    LmsCompany,
                    this.GetAdminProvider(),
                    param.course_id,
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

        // NOTE: quick fix. defined 2 routes due to http://stackoverflow.com/questions/35011192/how-to-define-an-optional-parameter-in-path-using-swagger
        [Route("delete")]
        [Route("delete/{id}")]
        [HttpPost]
        [EdugameCloud.Lti.Api.Filters.LmsAuthorizeBase]
        public virtual OperationResult DeleteRecording(int meetingId, string id)
        {
            try
            {
                var param = Session.LtiSession.LtiParam;

                if (!LmsCompany.CanRemoveRecordings)
                    throw new Core.WarningMessageException("Recording deletion is not enabled for the LMS license");

                OperationResult result = RecordingsService.RemoveRecording(
                    LmsCompany,
                    this.GetAdminProvider(),
                    param.course_id,
                    id,
                    meetingId);

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
        [EdugameCloud.Lti.Api.Filters.LmsAuthorizeBase]
        public OperationResult PublishRecording([FromBody]RecordingRequestDto request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            try
            {
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
        [EdugameCloud.Lti.Api.Filters.LmsAuthorizeBase]
        public OperationResult UnpublishRecording([FromBody]RecordingRequestDto request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            try
            {
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
        [EdugameCloud.Lti.Api.Filters.LmsAuthorizeBase]
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