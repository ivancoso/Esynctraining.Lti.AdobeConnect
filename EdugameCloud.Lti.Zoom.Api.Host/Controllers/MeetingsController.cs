using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Edugamecloud.Lti.Zoom.Dto;
using Edugamecloud.Lti.Zoom.Dto.Enums;
using Edugamecloud.Lti.Zoom.Services;
using EdugameCloud.Lti.API;
using EdugameCloud.Lti.Core.Business.Models;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.Zoom.Api.Host.FIlters;
using EdugameCloud.Lti.Zoom.Api.Host.Models;
using Esynctraining.Core.Caching;
using Esynctraining.Core.Domain;
using Esynctraining.Core.Json;
using Esynctraining.Core.Logging;
using Esynctraining.Core.Providers;
using Microsoft.AspNetCore.Mvc;
using ActionResult = Microsoft.AspNetCore.Mvc.ActionResult;
using MeetingRegistrationTypes = EdugameCloud.Lti.Zoom.Api.Host.Models.MeetingRegistrationTypes;
using ZoomMeetingType = EdugameCloud.Lti.Zoom.Api.Host.Models.ZoomMeetingType;

namespace EdugameCloud.Lti.Zoom.Api.Host.Controllers
{
    [Microsoft.AspNetCore.Mvc.Route("meetings")]
    public class MeetingsController : BaseApiController
    {
        private readonly IJsonSerializer _jsonSerializer;
        private readonly LmsCourseMeetingModel _lmsCourseMeetingModel;
        private readonly LmsUserSessionModel _userSessionModel;
        private readonly ZoomUserService _userService;
        private readonly ZoomMeetingService _meetingService;
        private readonly LmsFactory _lmsFactory;

        #region Constructors and Destructors

        public MeetingsController(
            //MeetingSetup meetingSetup,
            //API.AdobeConnect.IAdobeConnectAccountService acAccountService,
            ApplicationSettingsProvider settings,
            ILogger logger, ICache cache, IJsonSerializer jsonSerializer, LmsCourseMeetingModel lmsCourseMeetingModel, LmsUserSessionModel userSessionModel,
            ZoomUserService userService, ZoomRecordingService recordingService, ZoomMeetingService meetingService, LmsFactory lmsFactory)
            : base(settings, logger, cache)
        {
            //_meetingSetup = meetingSetup;
            _jsonSerializer = jsonSerializer;
            _lmsCourseMeetingModel = lmsCourseMeetingModel;
            _userSessionModel = userSessionModel;
            _userService = userService;
            _meetingService = meetingService;
            _lmsFactory = lmsFactory;
        }

        #endregion

        [Microsoft.AspNetCore.Mvc.Route("")]
        [Microsoft.AspNetCore.Mvc.HttpGet]
        [LmsAuthorizeBase(ApiCallEnabled = true)]
        public virtual async Task<OperationResultWithData<List<MeetingViewModel>>> GetCourseMeetings()
        {
            var param = Session.LtiSession.LtiParam;
            StringBuilder trace = null;

            string userId = null;
            try
            {
                var user = _userService.GetUser(param.lis_person_contact_email_primary);
                userId = user.Id;
            }
            catch (Exception e)
            {
                Logger.Error("User doesn't exist or doesn't belong to this account", e);
            }
            //if (!string.IsNullOrEmpty(userId))
                //{
                var dbMeetings = _lmsCourseMeetingModel.GetAllByCourseId(LmsCompany.Id, CourseId);
                    var zoomMeetingPairs = dbMeetings.GroupBy(x => x.AudioProfileId).ToDictionary(k=>k.Key, v=> v.Select(va => va.ScoId)); //audioProfileId is HostId
                var zoomMeetings = _meetingService.GetMeetings(zoomMeetingPairs, userId);
            var result = new List<MeetingViewModel>();
            foreach (var meeting in zoomMeetings)
            {
                var dbMeeting = dbMeetings.First(db => db.ScoId == meeting.Id);
                result.Add(ConvertFromDto(meeting, dbMeeting.Id, dbMeeting.LmsMeetingType));
            }
            var oh = result.SingleOrDefault(x => x.Type == 2);
            if (oh == null)
            {
                var ohMeeting = _lmsCourseMeetingModel.GetByCompanyWithAudioProfiles(LmsCompany).FirstOrDefault(x =>
                    x.LmsCompanyId == LmsCompany.Id && x.AudioProfileId == userId && x.LmsMeetingType == 2);
                if (ohMeeting != null)
                {
                    var ohDetails = _meetingService.GetMeetingDetails(ohMeeting.ScoId);
                    var detailsVm = ConvertToViewModel(ohDetails, 2);
                    var vm = ConvertFromDtoToOHViewModel(ohDetails, userId, ohMeeting.LmsMeetingType);
                    vm.Details = detailsVm;
                    result.Add(vm);
                }
            }
            else
            {
                var ohDetails = _meetingService.GetMeetingDetails(oh.ConferenceId);
                oh.Description = ohDetails.Agenda;
            }
            return result.ToSuccessResult();
            //}

            //return OperationResultWithData<IEnumerable<MeetingViewModel>>.Error("User does not exist in Zoom.");
        }

        [Microsoft.AspNetCore.Mvc.Route("{meetingId}")]
        [Microsoft.AspNetCore.Mvc.HttpGet]
        [LmsAuthorizeBase(ApiCallEnabled = true)]
        public virtual async Task<OperationResultWithData<MeetingDetailsViewModel>> GetMeetingDetails(int meetingId)
        {
            StringBuilder trace = null;

            try
            {
                var dbMeeting = _lmsCourseMeetingModel.GetOneByCourseAndId(LmsCompany.Id, CourseId, meetingId);
                var apiMeeting = _meetingService.GetMeetingDetails(dbMeeting.ScoId);
                var viewModel = ConvertToViewModel(apiMeeting, dbMeeting.LmsMeetingType);
                viewModel.Id = dbMeeting.Id;
                return viewModel.ToSuccessResult();
            }
            catch (Exception e)
            {
                Logger.Error("Meeting Details error.", e);
            }
            return OperationResultWithData<MeetingDetailsViewModel>.Error("Unexpected error happened");
        }

        [Microsoft.AspNetCore.Mvc.Route("")]
        [Microsoft.AspNetCore.Mvc.HttpPost]
        [LmsAuthorizeBase(ApiCallEnabled = true)]
        public virtual async Task<OperationResultWithData<MeetingViewModel>> Create([FromBody]CreateMeetingViewModel requestDto)
        {
            OperationResultWithData<MeetingViewModel> result = null;
            try
            {
                var param = Session.LtiSession.LtiParam;
                var trace = new StringBuilder();

                //var ac = this.GetAdminProvider();
                //var useLmsUserEmailForSearch = !string.IsNullOrEmpty(param.lis_person_contact_email_primary);
                //var fb = new MeetingFolderBuilder(Session.LmsCompany, ac, useLmsUserEmailForSearch, requestDto.GetMeetingType());

                //OperationResult ret = await _meetingSetup.SaveMeeting(
                //    LmsCompany,
                //    ac,
                //    param,
                //    requestDto,
                //    trace,
                //    fb);
                //if (isNewMeeting && LmsCompany.ConsumerKey == "b622bf8b-a120-4b40-816e-05f530a750d9" && param.course_id == 557)
                //{

                string userId = null;
                try
                {
                    var user = _userService.GetUser(param.lis_person_contact_email_primary);
                    userId = user.Id;
                }
                catch (Exception e)
                {
                    Logger.Error("User doesn't exist or doesn't belong to this account", e);
                    /*{
"code": 1005,
"message": "User already in the account: ivanr+zoomapitest@esynctraining.com"
}*/
                    var userInfo = _userService.CreateUser(new CreateUserDto
                    {
                        Email = param.lis_person_contact_email_primary,
                        FirstName = param.PersonNameGiven,
                        LastName = param.PersonNameFamily
                    });

                    return OperationResultWithData<MeetingViewModel>.Error(
                        "User either in 'pending' or 'inactive' status. Please check your email or contact Administrator and try again.");
                }
                if (!string.IsNullOrEmpty(userId))
                {
                    MeetingViewModel vm = null;
                    LmsCourseMeeting dbOfficeHours = null;
                    if (requestDto.Type.GetValueOrDefault(1) == 2) //Office Hours
                    {
                        var meetings = _lmsCourseMeetingModel.GetByCompanyWithAudioProfiles(LmsCompany).Where(x =>
                            x.LmsCompanyId == LmsCompany.Id && x.LmsMeetingType == 2);
                        if (meetings.Any(x => x.CourseId == CourseId))
                        {
                            return OperationResultWithData<MeetingViewModel>.Error(
                                "There is already created Office Hours meeting for this course. Please refresh page");
                        }
                        dbOfficeHours = meetings.FirstOrDefault();
                    }
                    
                    var dbMeeting = new LmsCourseMeeting
                    {
                        LmsCompanyId = LmsCompany.Id,
                        CourseId = param.course_id,
                        LmsMeetingType = requestDto.Type.GetValueOrDefault(1),
                        Reused = false,
                    };
                    if (dbOfficeHours != null)
                    {
                        var ohDetails = _meetingService.GetMeetingDetails(dbOfficeHours.ScoId);
                        vm = ConvertFromDto(ohDetails, userId, 2);
                        var ohJson = _jsonSerializer.JsonSerialize(ohDetails);
                        dbMeeting.ScoId = ohDetails.Id;
                        dbMeeting.MeetingNameJson = ohJson;
                        dbMeeting.AudioProfileId = ohDetails.HostId;
                    }
                    else
                    {
                        var dto = ConvertModelToDto(requestDto);
                        var m = _meetingService.CreateMeeting(userId, dto);
                        vm = ConvertFromDto(m, 0, 1);
                        var json = _jsonSerializer.JsonSerialize(m);
                        dbMeeting.ScoId = m.Id;
                        dbMeeting.MeetingNameJson = json;
                        dbMeeting.AudioProfileId = m.HostId;
                    }

                    _lmsCourseMeetingModel.RegisterSave(dbMeeting, true);
                    if (requestDto.Type.GetValueOrDefault(1) != 2 && requestDto.Settings.ApprovalType.GetValueOrDefault() == 1) //manual approval(secure connection)
                    {
                        var lmsService = _lmsFactory.GetUserService(LmsProviderEnum.Canvas); //add other LMSes later
                        var lmsUsers = await lmsService.GetUsers(LmsCompany, CourseId);
                        var registrants = lmsUsers.Data.Where(x => !String.IsNullOrEmpty(x.Email) && !x.Email.Equals(param.lis_person_contact_email_primary)).Select(x =>
                            new RegistrantDto
                            {
                                Email = x.Email,
                                FirstName = x.GetFirstName(),
                                LastName = x.GetLastName()
                            });
                        _userService.RegisterUsersToMeetingAndApprove(dbMeeting.ScoId, registrants, checkRegistrants: false);
                    }
                    vm.Id = dbMeeting.Id;
                    return vm.ToSuccessResult();
                }

                //}
                return OperationResultWithData<MeetingViewModel>.Error("User does not exist in Zoom.");
            }
            catch (Exception ex)
            {
                string errorMessage = GetOutputErrorMessage("CreateMeeting", ex);
                return OperationResultWithData<MeetingViewModel>.Error(errorMessage);
            }

        }

        [Microsoft.AspNetCore.Mvc.Route("{meetingId}")]
        [Microsoft.AspNetCore.Mvc.HttpPut]
        [LmsAuthorizeBase(ApiCallEnabled = true)]
        public virtual async Task<OperationResult> Update([FromBody]CreateMeetingViewModel vm, [FromRoute]int meetingId)
        {
                var param = Session.LtiSession.LtiParam;
            string userId = null;
            try
            {
                var dbMeeting = _lmsCourseMeetingModel.GetOneByCourseAndId(LmsCompany.Id, CourseId, meetingId);
                var dto = ConvertModelToDto(vm);
                var updated = _meetingService.UpdateMeeting(dbMeeting.ScoId, dto);
                if (updated)
                {
                    if (vm.Settings.ApprovalType.GetValueOrDefault() == 1) //manual approval(secure connection)
                    {
                        var lmsService = _lmsFactory.GetUserService(LmsProviderEnum.Canvas); //add other LMSes later
                        var lmsUsers = await lmsService.GetUsers(LmsCompany, CourseId);
                        var registrants = lmsUsers.Data.Where(x => !String.IsNullOrEmpty(x.Email) && !x.Email.Equals(param.lis_person_contact_email_primary)).Select(x =>
                            new RegistrantDto
                            {
                                Email = x.Email,
                                FirstName = x.GetFirstName(),
                                LastName = x.GetLastName()
                            });
                        _userService.RegisterUsersToMeetingAndApprove(dbMeeting.ScoId, registrants,
                            checkRegistrants: true);
                    }

                    return OperationResult.Success();
                }
            }
            catch (Exception e)
            {
                Logger.Error("Meeting Update error.", e);
            }
            return OperationResult.Error("Unexpected error happened");
        }

        [Microsoft.AspNetCore.Mvc.Route("{meetingId}/join")]
        [Microsoft.AspNetCore.Mvc.HttpGet]
        //[LmsAuthorizeBase]
        public virtual async Task<ActionResult> JoinMeeting(int meetingId, string session)
        {
            LmsCompany credentials = null;
            string sessionId = null;

                var s = GetReadOnlySession(session);
                sessionId = s.Id.ToString();
                credentials = s.LmsCompany;
                var param = s.LtiSession.LtiParam;
                var dbMeeting = _lmsCourseMeetingModel.GetOneByCourseAndId(credentials.Id, param.course_id, meetingId);

            //if (meeting == null && credentials.ConsumerKey == "b622bf8b-a120-4b40-816e-05f530a750d9" && param.course_id == 557)
            //{
            //    var mId = meetingId / 100000;
            //    viewModel = LmsCourseMeetingModel.GetOneByCourseAndId(credentials.Id, param.course_id, mId);
            if (dbMeeting == null)
                    //404
                    return NotFound(meetingId);

                string userId;
            try
                {
                    /*
                     {
    "code": 1010,
    "message": "User not belong to this account"
}*/
                    var user = _userService.GetUser(param.lis_person_contact_email_primary);
                    userId = user.Id;
                }
                catch (Exception e)
                {
                    Logger.Error("User doesn't exist or doesn't belong to this account", e);
                    /*{
"code": 1005,
"message": "User already in the account: ivanr+zoomapitest@esynctraining.com"
}*/
                    var userInfo = _userService.CreateUser(new CreateUserDto
                    {
                        Email = param.lis_person_contact_email_primary,
                        FirstName = param.PersonNameGiven,
                        LastName = param.PersonNameFamily
                    });

                    return Content(
                        "User either in 'pending' or 'inactive' status. Please check your email or contact Administrator and try again.");
                }
                if (!string.IsNullOrEmpty(userId))
                {
                    var url = await _meetingService.GetMeetingUrl(userId, dbMeeting.ScoId, param.lis_person_contact_email_primary,
                        async () =>
                        {
                            var lmsService = _lmsFactory.GetUserService(LmsProviderEnum.Canvas); //add other LMSes later
                            var lmsUsers = await lmsService.GetUsers(credentials, CourseId);
                            var registrant = lmsUsers.Data.FirstOrDefault(x =>
                                !String.IsNullOrEmpty(x.Email) && !x.Email.Equals(param.lis_person_contact_email_primary));
                            var registrantDto =
                                new RegistrantDto
                                {
                                    Email = registrant?.Email,
                                    FirstName = registrant?.GetFirstName(),
                                    LastName = registrant?.GetLastName()
                                };
                            return registrantDto;
                        });
                    return Redirect(url);
                }

                return Content("Error when joining.");
        }

        [Microsoft.AspNetCore.Mvc.Route("{meetingId}")]
        [Microsoft.AspNetCore.Mvc.HttpDelete]
        [LmsAuthorizeBase(ApiCallEnabled = true)]
        public virtual async Task<OperationResult> DeleteMeeting(int meetingId, [FromQuery] bool remove = false)
        {
            var meeting = _lmsCourseMeetingModel.GetOneByCourseAndId(LmsCompany.Id, CourseId, meetingId);
            if (meeting == null)
                return OperationResult.Error("Meeting not found");
            //check permissions

            if (meeting.LmsMeetingType == 2)
            {
                if (remove)
                {
                    _lmsCourseMeetingModel.RegisterDelete(meeting, true);

                }
                else
                {
                    var param = Session.LtiSession.LtiParam;

                    string userId = null;
                    var user = _userService.GetUser(param.lis_person_contact_email_primary);
                    userId = user.Id;
                    var isDeleted = _meetingService.DeleteMeeting(meeting.ScoId);
                    // find all user's OH and delete
                    var meetings = _lmsCourseMeetingModel.GetByCompanyWithAudioProfiles(LmsCompany).Where(x =>
                        x.LmsCompanyId == LmsCompany.Id && x.LmsMeetingType == 2 && x.AudioProfileId == userId);
                    foreach (var dbMeeting in meetings)
                    {
                        _lmsCourseMeetingModel.RegisterDelete(dbMeeting);
                    }
                }
            }
            else
            {
                var isDeleted = _meetingService.DeleteMeeting(meeting.ScoId);
                _lmsCourseMeetingModel.RegisterDelete(meeting, true);
            }

            return OperationResult.Success();

            //return OperationResult.Error("Error during delete. Please try again or contact support.");
        }

        protected LmsUserSession GetReadOnlySession(string key)
        {
            Guid uid;
            var session = Guid.TryParse(key, out uid) ? this._userSessionModel.GetByIdWithRelated(uid).Value : null;

            if (session == null)
            {
                Logger.WarnFormat("LmsUserSession not found. Key: {0}.", key);
                throw new Core.WarningMessageException(Resources.Messages.SessionTimeOut);
            }

            //System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(LanguageModel.GetById(session.LmsCompany.LanguageId).TwoLetterCode);

            return session;
        }

        private MeetingDetailsViewModel ConvertToViewModel(MeetingDetailsDto dto, int type=1)
        {
            int? regType = null;
            if (dto.Settings.RegistrationType.HasValue)
            {
                regType = (int)dto.Settings.RegistrationType.Value;
            }
            var vm = new MeetingDetailsViewModel
            {
                Duration = dto.Duration,
                ConferenceId = dto.Id,
                //Id = id,
                Timezone = dto.Timezone,
                Topic = dto.Topic,
                StartTime = dto.StartTime.Value,
                Agenda = dto.Agenda,
                Password = dto.Password,
                Type = type,
                //HasSessions = dto.HasSessions
                Settings = new CreateMeetingSettingsViewModel
                {
                    AudioType = (int)dto.Settings.AudioType,
                    EnableJoinBeforeHost = dto.Settings.EnableJoinBeforeHost,
                    ApprovalType = (int)dto.Settings.ApprovalType,
                    AlternativeHosts = dto.Settings.AlternativeHosts,
                    EnableParticipantVideo = dto.Settings.EnableParticipantVideo,
                    EnableMuteOnEntry = dto.Settings.EnableMuteOnEntry,
                    EnableHostVideo = dto.Settings.EnableHostVideo,
                    RecordingType = (int)dto.Settings.RecordingType,
                    //EnableWaitingRoom = dto.Settings.,
                    RecurrenceRegistrationType = regType,
                }
            };

            return vm;
        }

        private CreateZoomMeetingDto ConvertModelToDto(CreateMeetingViewModel vm)
        {
            var dto = new CreateZoomMeetingDto
            {
                Agenda = vm.Agenda,
                Duration = vm.Duration,
                Password = vm.Password,
                StartTime = vm.StartTime,
                Timezone = vm.Timezone,
                Topic = vm.Topic,
                Type = Edugamecloud.Lti.Zoom.Dto.Enums.ZoomMeetingType.Scheduled
                    //(EdugameCloud.Lti.Zoom.Api.Host.Services.Dto.Enums.ZoomMeetingType)vm.Type.GetValueOrDefault(2)
            };
            if (vm.Settings != null)
            {
                Edugamecloud.Lti.Zoom.Dto.Enums.MeetingRegistrationTypes? regType = null;
                if (vm.Settings.RecurrenceRegistrationType.HasValue)
                {
                    regType = (Edugamecloud.Lti.Zoom.Dto.Enums.MeetingRegistrationTypes)(int)vm.Settings.RecurrenceRegistrationType.Value;
                }
                dto.Settings = new CreateMeetingSettingsDto
                {
                    EnableJoinBeforeHost = vm.Settings.EnableJoinBeforeHost,

                    EnableWaitingRoom = vm.Settings.EnableWaitingRoom,
                    AlternativeHosts = vm.Settings.AlternativeHosts,
                    AudioType = (Edugamecloud.Lti.Zoom.Dto.Enums.MeetingAudioType)(int)vm.Settings.AudioType,
                    RecordingType = (Edugamecloud.Lti.Zoom.Dto.Enums.AutomaticRecordingType)(int)vm.Settings.RecordingType.GetValueOrDefault(0),
                    EnableHostVideo = vm.Settings.EnableHostVideo,
                    EnableMuteOnEntry = vm.Settings.EnableMuteOnEntry,
                    EnableParticipantVideo = vm.Settings.EnableParticipantVideo,
                    RegistrationType = regType,
                    ApprovalType = (ApprovalTypes)vm.Settings.ApprovalType.GetValueOrDefault(2)
                };
            }
            if (vm.Recurrence != null)
            {
                dto.Recurrence = new CreateMeetingRecurrenceDto
                {
                    DaysOfWeek = vm.Recurrence.DaysOfWeek,
                    Weeks = vm.Recurrence.Weeks
                };
            }

            return dto;
        }

        private MeetingViewModel ConvertFromDto(MeetingDto dto, int id = 0, int type = 1)
        {
            return new MeetingViewModel
            {
                ConferenceId = dto.Id,
                CanJoin = dto.CanJoin,
                CanEdit = dto.CanEdit,
                Duration = dto.Duration,
                Id = id,
                Timezone = dto.Timezone,
                Topic = dto.Topic,
                StartTime = dto.StartTime,
                HasSessions = dto.HasSessions,
                Type = type
            };

        }

        private MeetingViewModel ConvertFromDto(MeetingDetailsDto dto, string userId, int type = 1)
        {
            return new MeetingViewModel
            {
                ConferenceId = dto.Id,
                CanJoin = userId != null,
                CanEdit = dto.HostId == userId,
                Duration = dto.Duration,
               // Id = id,
                Timezone = dto.Timezone,
                Topic = dto.Topic,
                StartTime = dto.StartTime,
                HasSessions = (dto.Type == Edugamecloud.Lti.Zoom.Dto.Enums.ZoomMeetingType.RecurringWithTime),
                Type = type
            };

        }

        private OfficeHoursViewModel ConvertFromDtoToOHViewModel(MeetingDetailsDto dto, string userId, int type = 1)
        {
            return new OfficeHoursViewModel
            {
                ConferenceId = dto.Id,
                CanJoin = userId != null,
                CanEdit = dto.HostId == userId,
                Duration = dto.Duration,
                // Id = id,
                Timezone = dto.Timezone,
                Topic = dto.Topic,
                StartTime = dto.StartTime,
                HasSessions = (dto.Type == Edugamecloud.Lti.Zoom.Dto.Enums.ZoomMeetingType.RecurringWithTime),
                Type = type
            };

        }

    }

    public class RedirectController : Controller
    {
        
    }
}