﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Esynctraining.Core.Domain;
using Esynctraining.Core.Json;
using Esynctraining.Lti.Lms.Common.API;
using Esynctraining.Lti.Zoom.Api.Dto;
using Esynctraining.Lti.Zoom.Api.Dto.Enums;
using Esynctraining.Lti.Zoom.Domain;
using Esynctraining.Zoom.ApiWrapper;
using Esynctraining.Zoom.ApiWrapper.Model;
using Microsoft.EntityFrameworkCore;

namespace Esynctraining.Lti.Zoom.Api.Services
{
    public class ZoomMeetingService
    {
        private ZoomApiWrapper _zoomApi;
        private ZoomUserService _userService;
        private ZoomDbContext _dbContext;
        private LmsUserServiceBase _lmsUserService;
        private IJsonSerializer _jsonSerializer;

        public ZoomMeetingService(ZoomApiWrapper zoomApi, ZoomUserService userService, ZoomDbContext dbContext, LmsUserServiceBase lmsUserService, IJsonSerializer jsonSerializer)
        {
            _zoomApi = zoomApi;
            _userService = userService;
            _dbContext = dbContext;
            _lmsUserService = lmsUserService;
            _jsonSerializer = jsonSerializer;
        }

        //public IEnumerable<MeetingDto> GetMeetings(string userId)
        //{
        //    var meetings = _zoomApi.GetMeetings(userId, MeetingListTypes.Scheduled);

        //    return meetings.Meetings.Select(x => ConvertZoomObjToDto(x, userId));
        //}

        public async Task<MeetingDetailsViewModel> GetMeetingDetails(int meetingId, int licenseId, string courseId)
        {
            var dbMeeting = await GetMeeting(meetingId, licenseId, courseId);

            var meeting = _zoomApi.GetMeeting(dbMeeting.ProviderMeetingId);
            //var details = ConvertZoomObjToDetailsDto(meeting);
            var result = ConvertToDetailsViewModel(meeting);
            result.Id = dbMeeting.Id;
            result.Type = dbMeeting.Type;
            return result;
        }

        public async Task<IEnumerable<MeetingViewModel>>
            GetMeetings(int licenseId, string courseId, string currentUserId = null)
        {
            var dbMeetings = _dbContext.LmsCourseMeetings.Where(x =>
                x.LicenseId == licenseId && courseId == x.CourseId);
            var zoomMeetingPairs = dbMeetings.GroupBy(x => x.ProviderHostId)
                .ToDictionary(k => k.Key, v => v.Select(va => va.ProviderMeetingId));
            List<MeetingViewModel> result = new List<MeetingViewModel>();
            foreach (var user in zoomMeetingPairs)
            {
                var userMeetings = _zoomApi.GetMeetings(user.Key);
                result.AddRange(userMeetings.Meetings.Where(m => user.Value.Contains(m.Id)).Select(x =>
                    ConvertToViewModel(x, dbMeetings.First(db => db.ProviderHostId == x.Id), currentUserId)));
            }

            if (currentUserId != null)
            {
                return await ProcessOfficeHours(licenseId, currentUserId, result);
            }

            return result;
        }

        private async Task<IEnumerable<MeetingViewModel>> ProcessOfficeHours(int licenseId, string userId, List<MeetingViewModel> result)
        {
            var oh = result.SingleOrDefault(x => x.Type == 2);
            if (oh == null)
            {
                var ohMeeting =  await _dbContext.LmsCourseMeetings.FirstOrDefaultAsync(x =>
                    x.LicenseId == licenseId && x.ProviderHostId == userId && x.Type == 2);
                if (ohMeeting != null)
                {
                    var ohDetails = _zoomApi.GetMeeting(ohMeeting.ProviderMeetingId);
                    var detailsVm = ConvertToDetailsViewModel(ohDetails);
                    detailsVm.Type = ohMeeting.Type;
                    detailsVm.Id = ohMeeting.Id;
                    var vm = ConvertFromDtoToOHViewModel(ohDetails, userId, ohMeeting.Type);
                    vm.Id = ohMeeting.Id;
                    vm.Details = detailsVm;
                    result.Add(vm);
                }
            }
            else
            {
                var ohDetails = _zoomApi.GetMeeting(oh.ConferenceId);
                oh.Description = ohDetails.Agenda;
            }

            return result;
        }

        public async Task<string> GetMeetingUrl(string userId, string meetingId, string email, Func<Task<RegistrantDto>> getRegistrant)
        {
            var meeting = _zoomApi.GetMeeting(meetingId);
            string baseUrl = meeting.JoinUrl;
            if (meeting.Settings.ApprovalType != MeetingApprovalTypes.NoRegistration)
            {
                var registrants = _zoomApi.GetMeetingRegistrants(meetingId);
                var userReg = registrants.Registrants.FirstOrDefault(x =>
                    x.Email.Equals(email, StringComparison.InvariantCultureIgnoreCase));
                if (userReg != null)
                    baseUrl = userReg.JoinUrl;
                else
                {
                    var registrant = await getRegistrant();
                    if (registrant != null)
                    {
                        var registration = _userService.RegisterUsersToMeetingAndApprove(meetingId,
                            new List<RegistrantDto> {registrant}, false);
                        var registrants2 = _zoomApi.GetMeetingRegistrants(meetingId);
                        var userReg2 = registrants2.Registrants.FirstOrDefault(x =>
                            x.Email.Equals(email, StringComparison.InvariantCultureIgnoreCase));
                        if (userReg2 != null)
                            baseUrl = userReg2.JoinUrl;
                    }
                }
            }
            //foreach (var meeting in meetings.Meetings)
            //{
            //    var m = _client.Meetings.GetMeeting(meeting.Id);
            //    if (m.Agenda == meetingId.ToString())
            //    {
                    var userToken = _zoomApi.GetUserZpkToken(userId);
                    return baseUrl + (baseUrl.Contains("?") ? "&" : "?") + "zpk=" + userToken;
            //    }
            //}

            //return null;
        }

        public async Task<OperationResult> DeleteMeeting(int meetingId, int licenseId, string courseId, string email, bool remove, string occurenceId = null)
        {
            var meeting = await GetMeeting(meetingId, licenseId, courseId);
            if (meeting == null)
                return OperationResult.Error("Meeting not found");
            //check permissions

            if (meeting.Type == 2)
            {
                if (remove)
                {
                    _dbContext.Remove(meeting);
                }
                else
                {

                    string userId = null;
                    var user = _userService.GetUser(email);
                    userId = user.Id;
                    var isDeleted = _zoomApi.DeleteMeeting(meeting.ProviderMeetingId, occurenceId);
                    // find all user's OH and delete
                    var meetings = _dbContext.LmsCourseMeetings.Where(x =>
                        x.LicenseId == licenseId && x.ProviderHostId == userId && x.Type == 2);
                    _dbContext.RemoveRange(meetings);
                }
            }
            else
            {
                var isDeleted = _zoomApi.DeleteMeeting(meeting.ProviderMeetingId, occurenceId);
                _dbContext.Remove(meeting);
            }
            await _dbContext.SaveChangesAsync();
            return OperationResult.Success();

        }

        public async Task<OperationResultWithData<MeetingViewModel>> CreateMeeting(int licenseId, string courseId, string userId, string email,
            CreateMeetingViewModel requestDto)
        {
            MeetingViewModel vm = null;
            LmsCourseMeeting dbOfficeHours = null;
            if (requestDto.Type.GetValueOrDefault(1) == 2) //Office Hours
            {
                var dbMeetings = _dbContext.LmsCourseMeetings.Where(x =>
                    x.LicenseId == licenseId && x.Type == 2);
                if (dbMeetings.Any(x => x.CourseId == courseId))
                {
                    return OperationResultWithData<MeetingViewModel>.Error(
                        "There is already created Office Hours meeting for this course. Please refresh page");
                }
                dbOfficeHours = dbMeetings.FirstOrDefault();
            }

            var dbMeeting = new LmsCourseMeeting
            {
                LicenseId = licenseId,
                CourseId = courseId,
                Type = requestDto.Type.GetValueOrDefault(1),
                Reused = false,
            };
            if (dbOfficeHours != null)
            {
                /*
                 var ohDetails = _zoomApi.GetMeeting(ohMeeting.ProviderMeetingId);
                    var detailsVm = ConvertToDetailsViewModel(ohDetails);
                    detailsVm.Type = ohMeeting.Type;
                    detailsVm.Id = ohMeeting.Id;
                    var vm = ConvertFromDtoToOHViewModel(ohDetails, userId, ohMeeting.Type);
                    vm.Id = ohMeeting.Id;
                    vm.Details = detailsVm;
                 */
                var ohDetails = _zoomApi.GetMeeting(dbOfficeHours.ProviderMeetingId);
                vm = ConvertToViewModel(ohDetails, dbOfficeHours, userId);
                var ohJson = _jsonSerializer.JsonSerialize(ohDetails);
                dbMeeting.ProviderMeetingId = ohDetails.Id;
                dbMeeting.Details = ohJson;
                dbMeeting.ProviderHostId = ohDetails.HostId;
            }
            else
            {
                var m = await CreateApiMeeting(userId, requestDto);
                vm = ConvertToViewModel(m, null, userId);
                var json = _jsonSerializer.JsonSerialize(m);
                dbMeeting.ProviderMeetingId = m.Id;
                dbMeeting.Details = json;
                dbMeeting.ProviderHostId = m.HostId;
            }

            var entity = _dbContext.Add(dbMeeting);
            await _dbContext.SaveChangesAsync();

            if (requestDto.Type.GetValueOrDefault(1) != 2 && requestDto.Settings.ApprovalType.GetValueOrDefault() == 1
            ) //manual approval(secure connection)
            {
                var lmsService =
                    _lmsUserService; //_lmsFactory.GetUserService(LmsProviderEnum.Canvas); //add other LMSes later
                var lmsUsers = await lmsService.GetUsers(new Dictionary<string, object>(), courseId);

                //take users by email, throwing out current user
                var registrants = lmsUsers.Data.Where(x =>
                    !String.IsNullOrEmpty(x.Email) &&
                    !x.Email.Equals(email, StringComparison.InvariantCultureIgnoreCase)).Select(x =>
                    new RegistrantDto
                    {
                        Email = x.Email,
                        FirstName = x.GetFirstName(),
                        LastName = x.GetLastName()
                    });
                _userService.RegisterUsersToMeetingAndApprove(dbMeeting.ProviderMeetingId, registrants,
                    checkRegistrants: false);
            }
            vm.Id = dbMeeting.Id;
            vm.Type = dbMeeting.Type;

            return vm.ToSuccessResult();
        }

        private async Task<Meeting> CreateApiMeeting(string userId, CreateMeetingViewModel dto)
        {
            var meetingDto = ConvertFromDto(dto);

            var meeting = _zoomApi.CreateMeeting(userId, meetingDto);
            return meeting;
        }

        public async Task<LmsCourseMeeting> GetMeeting(int meetingId, int licenseId, string courseId)
        {
            return await _dbContext.LmsCourseMeetings.FirstOrDefaultAsync(x =>
                x.Id == meetingId && x.LicenseId == licenseId && x.CourseId == courseId);
        }

        public async Task<bool> UpdateMeeting(int meetingId, int licenseId, string courseId, string email, CreateMeetingViewModel vm)
        {
            var dbMeeting = await GetMeeting(meetingId, licenseId, courseId);
            var updated = UpdateApiMeeting(dbMeeting.ProviderMeetingId, vm);
            if (updated)
            {
                if (vm.Settings.ApprovalType.GetValueOrDefault() == 1) //manual approval(secure connection)
                {
                    var lmsService = _lmsUserService;//_lmsFactory.GetUserService(LmsProviderEnum.Canvas); //add other LMSes later
                    var lmsUsers = await lmsService.GetUsers(new Dictionary<string, object>(), courseId);
                    var registrants = lmsUsers.Data.Where(x => !String.IsNullOrEmpty(x.Email) && !x.Email.Equals(email)).Select(x =>
                        new RegistrantDto
                        {
                            Email = x.Email,
                            FirstName = x.GetFirstName(),
                            LastName = x.GetLastName()
                        });
                    _userService.RegisterUsersToMeetingAndApprove(dbMeeting.ProviderMeetingId, registrants,
                        checkRegistrants: true);
                }

                return true;
            }

            return false;
        }

        private Meeting ConvertFromDto(CreateMeetingViewModel dto)
        {
            var meetingDto = new Meeting
            {
                Topic = dto.Topic,
                Type = MeetingTypes.Scheduled, //(MeetingTypes)(int)dto.Type,
                Password = dto.Password,
                Agenda = dto.Agenda,
                Duration = dto.Duration,
                Timezone = dto.Timezone,
                StartTime = new DateTimeOffset(dto.StartTime)
            };

            if (dto.Settings != null)
            {
                meetingDto.Settings = new MeetingSettings
                {
                    EnableJoinBeforeHost = dto.Settings.EnableJoinBeforeHost,

                    ApprovalType = (MeetingApprovalTypes)(int)dto.Settings.ApprovalType,
                    AlternativeHosts = dto.Settings.AlternativeHosts,
                    Audio = ConvertAudioFromEnum((MeetingAudioType)dto.Settings.AudioType),
                    AutoRecording = dto.Settings.RecordingType.ToString().ToLower(),
                    EnableHostVideo = dto.Settings.EnableHostVideo,
                    EnableMuteOnEntry = dto.Settings.EnableMuteOnEntry,
                    EnableParticipantVideo = dto.Settings.EnableParticipantVideo
                };

                if (dto.Settings.RecurrenceRegistrationType.HasValue)
                {
                    meetingDto.Settings.RegistrationType =
                        (Esynctraining.Zoom.ApiWrapper.Model.MeetingRegistrationTypes)dto.Settings
                            .RecurrenceRegistrationType.Value;
                }
            }
            /*
            if (dto.Recurrence != null)
            {
                meetingDto.Recurrence = new MeetingRecurrence
                {
                    Type = MeetingRecurrenceTypes.Weekly,
                    WeeklyDaysList = new List<MeetingRecurrenceWeekDays>(dto.Recurrence.DaysOfWeek.Select(x => (MeetingRecurrenceWeekDays)x)),
                    RepeatInterval = 1,
                    EndDateTime = new DateTimeOffset(dto.StartTime.Value.AddDays(dto.Recurrence.Weeks * 7)),

                };
            }
            */

            return meetingDto;
        }

        public bool UpdateApiMeeting(string meetingId, CreateMeetingViewModel dto)
        {
            var meeting = ConvertFromDto(dto);
            bool updated = _zoomApi.UpdateMeeting(meetingId, meeting);
            return updated;
        }

        private MeetingViewModel ConvertToViewModel(Meeting meeting, LmsCourseMeeting dbMeeting, string userId)
        {
            var vm = new MeetingViewModel
            {
                ConferenceId = meeting.Id,
                CanJoin = userId != null,
                CanEdit = meeting.HostId == userId,
                Duration = meeting.Duration,
                Timezone = meeting.Timezone,
                Topic = meeting.Topic,
                StartTime = meeting.StartTime.DateTime,
                HasSessions = meeting.Type == MeetingTypes.RecurringWithTime,
            };

            if (dbMeeting != null)
            {
                vm.Id = dbMeeting.Id;
                vm.Type = dbMeeting.Type;
            }

            return vm;
        }

        private MeetingDetailsViewModel ConvertToDetailsViewModel(Meeting dto)
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
                StartTime = dto.StartTime.DateTime,
                Agenda = dto.Agenda,
                Password = dto.Password,
                //Type = type,
                //HasSessions = dto.HasSessions
                Settings = new CreateMeetingSettingsViewModel
                {
                    AudioType = (int)ConvertAudioToEnum(dto.Settings.Audio),
                    EnableJoinBeforeHost = dto.Settings.EnableJoinBeforeHost,
                    ApprovalType = (int)dto.Settings.ApprovalType,
                    AlternativeHosts = dto.Settings.AlternativeHosts,
                    EnableParticipantVideo = dto.Settings.EnableParticipantVideo,
                    EnableMuteOnEntry = dto.Settings.EnableMuteOnEntry,
                    EnableHostVideo = dto.Settings.EnableHostVideo,
                    RecordingType = (int)(Enum.TryParse<AutomaticRecordingType>(dto.Settings.AutoRecording, out AutomaticRecordingType recordingType) ? recordingType : AutomaticRecordingType.None),
                    //EnableWaitingRoom = dto.Settings.,
                    RecurrenceRegistrationType = regType,
                }
            };

            return vm;
        }

        private MeetingViewModel ConvertToViewModel(MeetingDetailsDto dto, string userId, int type)
        {
            return new MeetingViewModel
            {
                ConferenceId = dto.Id,
                CanJoin = userId != null,
                CanEdit = dto.HostId == userId,
                Duration = dto.Duration,
                //Id = dbMeeting.Id,
                Timezone = dto.Timezone,
                Topic = dto.Topic,
                StartTime = dto.StartTime,
                HasSessions = dto.Type == ZoomMeetingType.RecurringWithTime,
                Type = type
            };
        }

        private OfficeHoursViewModel ConvertFromDtoToOHViewModel(Meeting dto, string userId, int type = 1)
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
                StartTime = dto.StartTime.DateTime,
                HasSessions = (dto.Type == MeetingTypes.RecurringWithTime),
                Type = type
            };

        }

        private string ConvertAudioFromEnum(MeetingAudioType options)
        {
            switch (options)
            {
                case MeetingAudioType.Computer:
                    return MeetingAudioOptions.Voip;
                case MeetingAudioType.Telephone:
                    return MeetingAudioOptions.Telephone;
                case MeetingAudioType.Both:
                    return MeetingAudioOptions.Both;
            }

            return MeetingAudioOptions.Both;
        }

        private MeetingAudioType ConvertAudioToEnum(string options)
        {
            if (options == MeetingAudioOptions.Voip)
                return MeetingAudioType.Telephone;
            if (options == MeetingAudioOptions.Telephone)
                return MeetingAudioType.Computer;
            return MeetingAudioType.Both;
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
                Type = Esynctraining.Lti.Zoom.Api.Dto.Enums.ZoomMeetingType.Scheduled
                //(EdugameCloud.Lti.Zoom.Api.Host.Services.Dto.Enums.ZoomMeetingType)vm.Type.GetValueOrDefault(2)
            };
            if (vm.Settings != null)
            {
                Esynctraining.Lti.Zoom.Api.Dto.Enums.MeetingRegistrationTypes? regType = null;
                if (vm.Settings.RecurrenceRegistrationType.HasValue)
                {
                    regType = (Esynctraining.Lti.Zoom.Api.Dto.Enums.MeetingRegistrationTypes)(int)vm.Settings.RecurrenceRegistrationType.Value;
                }
                dto.Settings = new CreateMeetingSettingsDto
                {
                    EnableJoinBeforeHost = vm.Settings.EnableJoinBeforeHost,

                    EnableWaitingRoom = vm.Settings.EnableWaitingRoom,
                    AlternativeHosts = vm.Settings.AlternativeHosts,
                    AudioType = (MeetingAudioType)(int)vm.Settings.AudioType,
                    RecordingType = (AutomaticRecordingType)(int)vm.Settings.RecordingType.GetValueOrDefault(0),
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
    }
}