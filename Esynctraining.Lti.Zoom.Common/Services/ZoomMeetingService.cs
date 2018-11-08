using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Esynctraining.Core.Domain;
using Esynctraining.Core.Json;
using Esynctraining.Core.Logging;
using Esynctraining.Lti.Lms.Common.Constants;
using Esynctraining.Lti.Zoom.Common.Dto;
using Esynctraining.Lti.Zoom.Common.Dto.Enums;
using Esynctraining.Lti.Zoom.Common.Services.MeetingLoader;
using Esynctraining.Lti.Zoom.Domain;
using Esynctraining.Zoom.ApiWrapper;
using Esynctraining.Zoom.ApiWrapper.Model;
using Microsoft.EntityFrameworkCore;
using NodaTime;

namespace Esynctraining.Lti.Zoom.Common.Services
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

        public async Task<MeetingDetailsViewModel> GetMeetingDetails(int meetingId, string courseId)
        {
            var dbMeeting = await GetMeeting(meetingId, courseId);

            return await _zoomMeetingApiService.GetMeetingApiDetails(dbMeeting);
        }

        public async Task<OperationResultWithData<IEnumerable<MeetingViewModel>>> GetMeetings(string courseId, CourseMeetingType type, string email, string currentUserId = null)
        {
            var licenseDto = await _licenseAccessor.GetLicense();

            MeetingsLoader meetingsLoader = null;
            switch (type)
            {
                case CourseMeetingType.Basic:
                    meetingsLoader = new BasicMeetingsLoader(_dbContext, licenseDto.ConsumerKey, courseId, _zoomApi, currentUserId);
                    break;
                case CourseMeetingType.OfficeHour:
                    meetingsLoader = new OfficeHoursMeetingsLoader(_dbContext, licenseDto.ConsumerKey, courseId, _zoomApi, currentUserId, _ohService);
                    break;
                case CourseMeetingType.StudyGroup:
                    meetingsLoader = new StudyGroupMeetingsLoader(_dbContext, licenseDto.ConsumerKey, courseId, _zoomApi, currentUserId, email);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }

            var newResult = await meetingsLoader.Load();
            return newResult.ToSuccessResult();
        }


        public async Task<string> GetMeetingUrl(string userId, string meetingId, string email,
            Func<Task<RegistrantDto>> getRegistrant)
        {
            var licenseDto = await _licenseAccessor.GetLicense();
            var meetingResult = await _zoomApi.GetMeeting(meetingId);
            string baseUrl = meetingResult.Data.JoinUrl;
            if (meetingResult.Data.HostId != userId
                && meetingResult.Data.Settings.ApprovalType != MeetingApprovalTypes.NoRegistration
                && licenseDto.GetSetting<bool>(LmsLicenseSettingNames.EnableClassRosterSecurity))
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
                        var registration = await _userService.RegisterUsersToMeetingAndApprove(meetingId,
                            new List<RegistrantDto> { registrant }, false);
                        var registrants2 = _zoomApi.GetMeetingRegistrants(meetingId);
                        var userReg2 = registrants2.Registrants.FirstOrDefault(x =>
                            x.Email.Equals(email, StringComparison.InvariantCultureIgnoreCase));
                        if (userReg2 != null)
                            baseUrl = userReg2.JoinUrl;
                    }
                }
            }
            var userToken = _zoomApi.GetUserToken(userId, "zpk");
            return baseUrl + (baseUrl.Contains("?") ? "&" : "?") + "zpk=" + userToken;
        }

        public string GetToken(string userId, string type)
        {
            return _zoomApi.GetUserToken(userId, type);
        }

        public async Task<OperationResult> DeleteMeeting(int meetingId, string courseId, string email, bool remove, string occurenceId = null)
        {

            var meeting = await GetMeeting(meetingId, courseId);
            if (meeting == null)
                return OperationResult.Error("Meeting not found");
            //check permissions

            _logger.Info($"User {email} deleted meeting {meetingId} with details {meeting.Details}");

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

        public async Task<OperationResultWithData<MeetingViewModel>> CreateMeeting(Dictionary<string, object> lmsSettings, string courseId, UserInfoDto user, string email,
            CreateMeetingViewModel requestDto)
        {
            LmsLicenseDto licenseDto = await _licenseAccessor.GetLicense();

            MeetingViewModel vm = null;
            LmsCourseMeeting dbOfficeHours = null;
            if (requestDto.Type.GetValueOrDefault(1) == 2) //Office Hours
            {
                var dbMeetings = _dbContext.LmsCourseMeetings.Where(x => x.LicenseKey == licenseDto.ConsumerKey && x.Type == 2);
                if (dbMeetings.Any(x => x.CourseId == courseId))
                {
                    return OperationResultWithData<MeetingViewModel>.Error(
                        "There is already created Office Hours meeting for this course. Please refresh page");
                }
                dbOfficeHours = dbMeetings.FirstOrDefault(x => x.ProviderHostId == user.Id);
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
                var ohDetailsResult = await _zoomApi.GetMeeting(dbOfficeHours.ProviderMeetingId);
                if (!ohDetailsResult.IsSuccess)
                    throw new Exception(ohDetailsResult.Message);

                vm = ConvertToViewModel(ohDetailsResult.Data, dbOfficeHours, user.Id);
                var ohJson = _jsonSerializer.JsonSerialize(ohDetailsResult.Data);
                dbMeeting.ProviderMeetingId = ohDetailsResult.Data.Id;
                dbMeeting.Details = ohJson;
                dbMeeting.ProviderHostId = ohDetailsResult.Data.HostId;
            }
            else
            {
                var m = await CreateApiMeeting(user, requestDto);
                if (!m.IsSuccess)
                {
                    return OperationResultWithData<MeetingViewModel>.Error(m.Message);
                }
                vm = ConvertToViewModel(m.Data, null, user.Id);
                var json = _jsonSerializer.JsonSerialize(m);
                dbMeeting.ProviderMeetingId = m.Data.Id;
                dbMeeting.Details = json;
                dbMeeting.ProviderHostId = m.Data.HostId;
            }

            var entity = _dbContext.Add(dbMeeting);
            await _dbContext.SaveChangesAsync();

            if (requestDto.Type.GetValueOrDefault(1) != 2 && requestDto.Type.GetValueOrDefault(1) != 3
                && requestDto.Settings.ApprovalType.GetValueOrDefault() == 1) //manual approval(secure roster)
            {
                var lmsService = _lmsUserServiceFactory.GetUserService(licenseDto.ProductId);

                var lmsUsers = await lmsService.GetUsers(lmsSettings, courseId);

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
                await _userService.RegisterUsersToMeetingAndApprove(dbMeeting.ProviderMeetingId, registrants, false);
            }

            if (requestDto.Type.GetValueOrDefault(1) == (int)CourseMeetingType.StudyGroup)
            {
                if (requestDto.Participants != null)
                {
                    var registrants = requestDto.Participants.Where(x => !x.Email.Equals(email, StringComparison.InvariantCultureIgnoreCase));
                    await _userService.RegisterUsersToMeetingAndApprove(dbMeeting.ProviderMeetingId, registrants, false);
                }
            }

            vm.Id = dbMeeting.Id;
            vm.Type = dbMeeting.Type;

            return vm.ToSuccessResult();
        }

        private async Task<OperationResultWithData<Meeting>> CreateApiMeeting(UserInfoDto user, CreateMeetingViewModel dto)
        {
            var meetingDto = ConvertFromDto(dto);
            var offset = GetTimezoneOffset(user, meetingDto.StartTime);
            meetingDto.StartTime = meetingDto.StartTime.AddSeconds(offset);
            meetingDto.Timezone = user.Timezone;

            var result = _zoomApi.CreateMeeting(user.Id, meetingDto);

            if (!result.IsSuccess)
            {
                return OperationResultWithData<Meeting>.Error(result.Message);
            }

            return result.Data.ToSuccessResult();
        }

        public async Task<LmsCourseMeeting> GetMeeting(int meetingId, string courseId)
        {
            LmsLicenseDto licenseDto = await _licenseAccessor.GetLicense();
            return await _dbContext.LmsCourseMeetings.FirstOrDefaultAsync(x =>
                x.Id == meetingId && x.LicenseKey == licenseDto.ConsumerKey && x.CourseId == courseId);
        }

        public async Task<OperationResultWithData<bool>> UpdateMeeting(int meetingId, Dictionary<string, object> licenseSettings, string courseId, string email, CreateMeetingViewModel vm, UserInfoDto user)
        {
            var dbMeeting = await GetMeeting(meetingId, courseId);
            var updatedResult = UpdateApiMeeting(dbMeeting.ProviderMeetingId, user, vm);
            if (!updatedResult.IsSuccess)
            {
                return OperationResultWithData<bool>.Error(updatedResult.Message);
            }

            if (updatedResult.Data)
            {
                if (dbMeeting.Type != (int)CourseMeetingType.StudyGroup)
                {
                    LmsLicenseDto licenseDto = await _licenseAccessor.GetLicense();
                    if (vm.Settings.ApprovalType.GetValueOrDefault() == 1) //manual approval(secure roster)
                    {
                        var lmsService = _lmsUserServiceFactory.GetUserService(licenseDto.ProductId);
                        var lmsUsers = await lmsService.GetUsers(licenseSettings, courseId);
                        var registrants = lmsUsers.Data
                            .Where(x => !String.IsNullOrEmpty(x.Email) && !x.Email.Equals(email)).Select(x =>
                                new RegistrantDto
                                {
                                    Email = x.Email,
                                    FirstName = x.GetFirstName(),
                                    LastName = x.GetLastName()
                                });
                        await _userService.RegisterUsersToMeetingAndApprove(dbMeeting.ProviderMeetingId, registrants, true);
                    }
                }

                if (dbMeeting.Type == (int)CourseMeetingType.StudyGroup)
                {
                    var registrants = _userService.GetMeetingRegistrants(dbMeeting.ProviderMeetingId);
                    _userService.CleanApprovedRegistrant(dbMeeting.ProviderMeetingId, vm.Participants, registrants);

                    await _userService.RegisterUsersToMeetingAndApprove(dbMeeting.ProviderMeetingId, vm.Participants, true);
                }

                return true.ToSuccessResult();
            }

            return OperationResultWithData<bool>.Error("Meeting has not been updated");
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
            return meetingDto;
        }

        public OperationResultWithData<bool> UpdateApiMeeting(string meetingId, UserInfoDto user, CreateMeetingViewModel dto)
        {
            var meeting = ConvertFromDto(dto);

            var offset = GetTimezoneOffset(user, meeting.StartTime);
            meeting.StartTime = meeting.StartTime.AddSeconds(offset);
            meeting.Timezone = user.Timezone;

            var updatedResult = _zoomApi.UpdateMeeting(meetingId, meeting);

            return updatedResult.IsSuccess
                ? updatedResult.Data.ToSuccessResult()
                : OperationResultWithData<bool>.Error(updatedResult.Message);
        }

        private MeetingViewModel ConvertToViewModel(Meeting meeting, LmsCourseMeeting dbMeeting, string userId)
        {

            var vm = new MeetingViewModel
            {
                ConferenceId = meeting.Id,
                CanJoin = userId != null,
                CanEdit = meeting.HostId == userId,
                HostId = meeting.HostId,
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
    }
}