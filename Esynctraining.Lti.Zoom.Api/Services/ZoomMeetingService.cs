using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Esynctraining.Core.Domain;
using Esynctraining.Core.Json;
using Esynctraining.Core.Logging;
using Esynctraining.Lti.Lms.Common;
using Esynctraining.Lti.Lms.Common.API;
using Esynctraining.Lti.Lms.Common.Constants;
using Esynctraining.Lti.Zoom.Api.Dto;
using Esynctraining.Lti.Zoom.Api.Dto.Enums;
using Esynctraining.Lti.Zoom.Domain;
using Esynctraining.Zoom.ApiWrapper;
using Esynctraining.Zoom.ApiWrapper.Model;
using Microsoft.EntityFrameworkCore;
using NodaTime;

namespace Esynctraining.Lti.Zoom.Api.Services
{
    public class ZoomMeetingService
    {
        private readonly ZoomApiWrapper _zoomApi;
        private readonly ZoomMeetingApiService _zoomMeetingApiService;
        private readonly ZoomUserService _userService;
        private readonly ZoomDbContext _dbContext;
        private readonly LmsUserServiceFactory _lmsUserServiceFactory;
        private readonly IJsonSerializer _jsonSerializer;
        private readonly ILmsLicenseAccessor _licenseAccessor;
        private readonly ZoomOfficeHoursService _ohService;
        private readonly ILogger _logger;

        public ZoomMeetingService(ZoomApiWrapper zoomApi, ZoomUserService userService, ZoomDbContext dbContext,
            LmsUserServiceFactory lmsUserServiceFactory, IJsonSerializer jsonSerializer, ILmsLicenseAccessor licenseAccessor,
            ZoomOfficeHoursService ohService, ZoomMeetingApiService zoomMeetingApiService, ILogger logger)
        {
            _zoomApi = zoomApi;
            _zoomMeetingApiService = zoomMeetingApiService;
            _userService = userService;
            _dbContext = dbContext;
            _lmsUserServiceFactory = lmsUserServiceFactory;
            _jsonSerializer = jsonSerializer;
            _licenseAccessor = licenseAccessor;
            _ohService = ohService;
            _logger = logger;
        }

        //public IEnumerable<MeetingDto> GetMeetings(string userId)
        //{
        //    var meetings = _zoomApi.GetMeetings(userId, MeetingListTypes.Scheduled);

        //    return meetings.Meetings.Select(x => ConvertZoomObjToDto(x, userId));
        //}

        public async Task<MeetingDetailsViewModel> GetMeetingDetails(int meetingId, string courseId)
        {
            var dbMeeting = await GetMeeting(meetingId, courseId);

            return await _zoomMeetingApiService.GetMeetingApiDetails(dbMeeting);
        }

        public async Task<IEnumerable<MeetingViewModel>> GetMeetings(string courseId, string currentUserId = null)
        {
            var licenseDto = await _licenseAccessor.GetLicense();
            List<MeetingViewModel> result = new List<MeetingViewModel>();
            var dbMeetings = _dbContext.LmsCourseMeetings.Where(x => 
                x.LicenseKey == licenseDto.ConsumerKey && courseId == x.CourseId);
            if (dbMeetings.Any())
            {
                var zoomMeetingPairs = dbMeetings.GroupBy(x => x.ProviderHostId)
                    .ToDictionary(k => k.Key, v => v.Select(va => va.ProviderMeetingId));
                foreach (var user in zoomMeetingPairs)
                {
                    var zoomApiResult = _zoomApi.GetMeetings(user.Key);
                    if (!zoomApiResult.IsSuccess)
                        throw new Exception(zoomApiResult.Message);

                    var userMeetings = zoomApiResult.Data;

                    if (userMeetings == null)
                        continue;

                    result.AddRange(userMeetings.Meetings.Where(m => user.Value.Contains(m.Id)).Select(x =>
                        ConvertToViewModel(x, dbMeetings.First(db => db.ProviderMeetingId == x.Id), currentUserId)));
                    //zoom does not return meeting with current start time within user's meeting request, so handle such meetings one-by-one
                    var notHandledMeetings = user.Value.Where(x => userMeetings.Meetings.All(m => m.Id != x));
                    foreach (var notHandledId in notHandledMeetings)
                    {
                        var details = _zoomApi.GetMeeting(notHandledId);
                        if (details != null)
                        {
                            var vm = ConvertToViewModel(details,
                                dbMeetings.First(db => db.ProviderMeetingId == details.Id), currentUserId);
                            result.Add(vm);
                        }
                    }
                }
            }
            if (currentUserId != null)
            {
                return await ProcessOfficeHours(licenseDto.ConsumerKey, currentUserId, result);
            }

            return result;
        }

        private async Task<IEnumerable<MeetingViewModel>> ProcessOfficeHours(Guid licenseKey, string userId, List<MeetingViewModel> result)
        {
            var oh = result.SingleOrDefault(x => x.Type == 2);
            if (oh == null)
            {
                var ohMeeting =  await _dbContext.LmsCourseMeetings.FirstOrDefaultAsync(x =>
                    x.LicenseKey == licenseKey && x.ProviderHostId == userId && x.Type == 2);
                if (ohMeeting != null)
                {
                    var ohDetails = _zoomApi.GetMeeting(ohMeeting.ProviderMeetingId);
                    var detailsVm = ConvertToDetailsViewModel(ohDetails);
                    detailsVm.Type = ohMeeting.Type;
                    detailsVm.Id = ohMeeting.Id;
                    var vm = ConvertFromDtoToOHViewModel(ohDetails, userId, ohMeeting.Type);
                    vm.Id = ohMeeting.Id;
                    vm.Details = detailsVm;
                    vm.Availabilities = await _ohService.GetAvailabilities(ohMeeting.Id, userId);
                    result.Add(vm);
                }
            }
            else
            {
                var ohDetails = _zoomApi.GetMeeting(oh.ConferenceId);
                oh.Description = ohDetails.Agenda;
                oh.Availabilities = await _ohService.GetAvailabilities(oh.Id, userId);
            }

            return result;
        }

        public async Task<string> GetMeetingUrl(string userId, string meetingId, string email,
            Func<Task<RegistrantDto>> getRegistrant)
        {
            var meeting = _zoomApi.GetMeeting(meetingId);
            string baseUrl = meeting.JoinUrl;
            if (meeting.HostId != userId && meeting.Settings.ApprovalType != MeetingApprovalTypes.NoRegistration)
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

        public async Task<OperationResult> DeleteMeeting(int meetingId, string courseId, string email, bool remove, string occurenceId = null)
        {
            var meeting = await GetMeeting(meetingId, courseId);
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
                    LmsLicenseDto licenseDto = await _licenseAccessor.GetLicense();

                    var meetings = _dbContext.LmsCourseMeetings.Where(x =>
                        x.LicenseKey == licenseDto.ConsumerKey && x.ProviderHostId == userId && x.Type == 2);
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

        public async Task<OperationResultWithData<MeetingViewModel>> CreateMeeting(Dictionary<string, object> licenseSettings, string courseId, UserInfoDto user, string email,
            CreateMeetingViewModel requestDto)
        {
            LmsLicenseDto licenseDto = await _licenseAccessor.GetLicense();
            MeetingViewModel vm = null;
            LmsCourseMeeting dbOfficeHours = null;
            if (requestDto.Type.GetValueOrDefault(1) == 2) //Office Hours
            {
                var dbMeetings = _dbContext.LmsCourseMeetings.Where(x =>
                    x.LicenseKey == licenseDto.ConsumerKey && x.Type == 2);
                if (dbMeetings.Any(x => x.CourseId == courseId))
                {
                    return OperationResultWithData<MeetingViewModel>.Error(
                        "There is already created Office Hours meeting for this course. Please refresh page");
                }
                dbOfficeHours = dbMeetings.FirstOrDefault();
            }

            var dbMeeting = new LmsCourseMeeting
            {
                LicenseKey = licenseDto.ConsumerKey,
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
                vm = ConvertToViewModel(ohDetails, dbOfficeHours, user.Id);
                var ohJson = _jsonSerializer.JsonSerialize(ohDetails);
                dbMeeting.ProviderMeetingId = ohDetails.Id;
                dbMeeting.Details = ohJson;
                dbMeeting.ProviderHostId = ohDetails.HostId;
            }
            else
            {
                var m = await CreateApiMeeting(user, requestDto);
                vm = ConvertToViewModel(m, null, user.Id);
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
                var lmsService = _lmsUserServiceFactory.GetUserService(licenseDto.ProductId);

                var lmsUsers = await lmsService.GetUsers(licenseSettings, courseId);

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

        ////todo: factory. now for Canvas only
        //private Dictionary<string, object> GetSettings(string userToken, LmsLicenseDto license)
        //{
        //    var optionNamesForCanvas = new List<string> { LmsLicenseSettingNames.OAuthAppId, LmsLicenseSettingNames.OAuthAppKey };
        //    var result = license.Settings.Where(x => optionNamesForCanvas.Any(o => o == x.Key)).ToDictionary(k => k.Key, v => (object)v.Value);
        //    result.Add(LmsLicenseSettingNames.LicenseKey, license.ConsumerKey);
        //    result.Add(LmsLicenseSettingNames.LmsDomain, license.Domain);


        //    result.Add(LmsUserSettingNames.Token, userToken);
        //    return result;
        //}

        private async Task<Meeting> CreateApiMeeting(UserInfoDto user, CreateMeetingViewModel dto)
        {
            var meetingDto = ConvertFromDto(dto);
            var offset = GetTimezoneOffset(user, meetingDto.StartTime);
            meetingDto.StartTime = meetingDto.StartTime.AddSeconds(offset);
            meetingDto.Timezone = user.Timezone;

            var meeting = _zoomApi.CreateMeeting(user.Id, meetingDto);
            return meeting;
        }

        public async Task<LmsCourseMeeting> GetMeeting(int meetingId, string courseId)
        {
            LmsLicenseDto licenseDto = await _licenseAccessor.GetLicense();
            return await _dbContext.LmsCourseMeetings.FirstOrDefaultAsync(x =>
                x.Id == meetingId && x.LicenseKey == licenseDto.ConsumerKey && x.CourseId == courseId);
        }

        public async Task<bool> UpdateMeeting(int meetingId, Dictionary<string, object> licenseSettings, string courseId, string email, CreateMeetingViewModel vm, UserInfoDto user)
        {
            var dbMeeting = await GetMeeting(meetingId, courseId);
            var updated = UpdateApiMeeting(dbMeeting.ProviderMeetingId, user, vm);
            if (updated)
            {
                if (vm.Settings.ApprovalType.GetValueOrDefault() == 1) //manual approval(secure connection)
                {
                    LmsLicenseDto license = await _licenseAccessor.GetLicense();
                    //var settings = GetSettings(userToken, license);
                    var lmsService = _lmsUserServiceFactory.GetUserService(license.ProductId);//_lmsFactory.GetUserService(LmsProviderEnum.Canvas); //add other LMSes later
                    var lmsUsers = await lmsService.GetUsers(licenseSettings, courseId);
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
                Duration = dto.Duration.GetValueOrDefault(60), //default duration=60 if OH
                Timezone = dto.Timezone,
                StartTime = dto.StartTime.HasValue
                    ? DateTimeOffset.FromUnixTimeMilliseconds(dto.StartTime.Value)
                    : DateTimeOffset.UtcNow // default is now if OH
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

        public bool UpdateApiMeeting(string meetingId, UserInfoDto user, CreateMeetingViewModel dto)
        {
            var meeting = ConvertFromDto(dto);

            var offset = GetTimezoneOffset(user, meeting.StartTime);
            meeting.StartTime = meeting.StartTime.AddSeconds(offset);
            meeting.Timezone = user.Timezone;

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
                StartTime = GetUtcTime(meeting),
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
                StartTime = GetUtcTime(dto),
                Agenda = dto.Agenda,
                Password = dto.Password,
                JoinUrl = dto.JoinUrl,
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
                StartTime = GetUtcTime(dto),
                HasSessions = (dto.Type == MeetingTypes.RecurringWithTime),
                Type = type
            };

        }

        private long GetTimezoneOffset(UserInfoDto user, DateTimeOffset meetingTime)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (string.IsNullOrEmpty(user.Timezone))
            {
                _logger.Warn($"Timezone property is empty or null for UserId={user.Id}, Email={user.Email}");
                return 0;
            }
            var timezone = DateTimeZoneProviders.Tzdb.GetZoneOrNull(user.Timezone);
            if (timezone == null)
            {
                _logger.Warn($"Timezone not found. UserId={user.Id}, Email={user.Email}");
                return 0;
            }

            var instant = Instant.FromDateTimeOffset(meetingTime);
            var offset = timezone.GetUtcOffset(instant);
            return offset.Seconds;
        }

        private long GetUtcTime(Meeting meeting)
        {
            if (meeting == null)
                throw new ArgumentNullException(nameof(meeting));

            return meeting.StartTime.ToUnixTimeMilliseconds();

            //todo: remove method. It looks like time is coming in UTC from API
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
                return MeetingAudioType.Computer;
            if (options == MeetingAudioOptions.Telephone)
                return MeetingAudioType.Telephone;
            return MeetingAudioType.Both;
        }
    }
}