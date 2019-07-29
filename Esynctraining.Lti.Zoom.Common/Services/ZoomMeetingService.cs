using Esynctraining.Core.Domain;
using Esynctraining.Core.Json;
using Esynctraining.Core.Logging;
using Esynctraining.Lti.Lms.Common.Constants;
using Esynctraining.Lti.Lms.Common.Dto;
using Esynctraining.Lti.Zoom.Common.Dto;
using Esynctraining.Lti.Zoom.Common.Dto.Enums;
using Esynctraining.Lti.Zoom.Common.Services.MeetingLoader;
using Esynctraining.Lti.Zoom.Domain;
using Esynctraining.Zoom.ApiWrapper;
using Esynctraining.Zoom.ApiWrapper.Model;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        private readonly LmsCalendarEventServiceFactory _lmsCalendarEventServiceFactory;

        public ZoomMeetingService(ZoomApiWrapper zoomApi, 
            ZoomUserService userService, 
            ZoomDbContext dbContext,
            LmsUserServiceFactory lmsUserServiceFactory, 
            IJsonSerializer jsonSerializer, 
            ILmsLicenseAccessor licenseAccessor,
            ZoomOfficeHoursService ohService, 
            ZoomMeetingApiService zoomMeetingApiService, 
            ILogger logger, 
            LmsCalendarEventServiceFactory lmsCalendarEventServiceFactory)
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
            _lmsCalendarEventServiceFactory = lmsCalendarEventServiceFactory ?? throw new ArgumentNullException(nameof(lmsCalendarEventServiceFactory));
        }

        public async Task<MeetingDetailsViewModel> GetMeetingDetails(int meetingId, string courseId)
        {
            var dbMeeting = await GetMeeting(meetingId, courseId);
            return await _zoomMeetingApiService.GetMeetingApiDetails(dbMeeting);
        }

        public async Task<OperationResultWithData<IEnumerable<MeetingViewModel>>> GetMeetings(string courseId, CourseMeetingType type, string email, UserInfoDto user, string currentUserId = null)
        {
            var licenseDto = await _licenseAccessor.GetLicense();

            MeetingsLoader meetingsLoader = null;
            switch (type)
            {
                case CourseMeetingType.Basic:
                    meetingsLoader = new BasicMeetingsLoader(_dbContext, licenseDto.ConsumerKey, courseId, _zoomApi, currentUserId, user);
                    break;
                case CourseMeetingType.OfficeHour:
                    meetingsLoader = new OfficeHoursMeetingsLoader(_dbContext, licenseDto.ConsumerKey, courseId, _zoomApi, currentUserId, _ohService, user);
                    break;
                case CourseMeetingType.StudyGroup:
                    meetingsLoader = new StudyGroupMeetingsLoader(_dbContext, licenseDto.ConsumerKey, courseId, _zoomApi, currentUserId, email, user);
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
                var registrants = await _zoomApi.GetMeetingRegistrants(meetingId);
                var userReg = registrants.Registrants.FirstOrDefault(x =>
                    x.Email.Equals(email, StringComparison.InvariantCultureIgnoreCase));
                if (userReg != null)
                    baseUrl = userReg.JoinUrl;
                else
                {
                    var registrant = await getRegistrant();
                    if (registrant != null)
                    {
                        var newZoomAddRegistrantRequest = new ZoomAddRegistrantRequest(registrant.Email,
                            registrant.FirstName, registrant.LastName);
                        var addResult = await _zoomApi.AddRegistrant(meetingId, newZoomAddRegistrantRequest);
                        //if (!addResult.IsSuccess)
                        //{
                        //    return ;
                        //}
                        await _userService.UpdateRegistrantStatus(meetingId, new [] {registrant.Email}, nameof(RegistrantUpdateStatusAction.Approve));
                        var registrants2 = await _zoomApi.GetMeetingRegistrants(meetingId);
                        var userReg2 = registrants2.Registrants.FirstOrDefault(x =>
                            x.Email.Equals(email, StringComparison.InvariantCultureIgnoreCase));
                        if (userReg2 != null)
                            baseUrl = userReg2.JoinUrl;
                    }
                }
            }
            var userToken = await _zoomApi.GetUserToken(userId, "zpk");
            return baseUrl + (baseUrl.Contains("?") ? "&" : "?") + "zpk=" + userToken;
        }

        public async Task<string> GetToken(string userId, string type)
        {
            return await _zoomApi.GetUserToken(userId, type);
        }

        public async Task<OperationResult> DeleteMeeting(int meetingId, string courseId, string email, bool remove, Dictionary<string, object> lmsSettings, string occurenceId = null)
        {
            var meeting = await GetMeeting(meetingId, courseId);
            if (meeting == null)
                return OperationResult.Error("Meeting not found");
            //check permissions

            _logger.Info($"User {email} deleted meeting {meetingId} with details {meeting.Details}");

            if (meeting.Type == (int) CourseMeetingType.OfficeHour)
            {
                if (remove)
                {
                    _dbContext.Remove(meeting);
                }
                else
                {
                    string userId = null;
                    var user = await _userService.GetUser(email);
                    userId = user.Id;
                    var isDeleted = await _zoomApi.DeleteMeeting(meeting.ProviderMeetingId, occurenceId);
                    // find all user's OH and delete
                    LmsLicenseDto licenseDto = await _licenseAccessor.GetLicense();

                    var meetings = _dbContext.LmsCourseMeetings.Where(x =>
                        x.LicenseKey == licenseDto.ConsumerKey && x.ProviderHostId == userId &&
                        x.Type == (int) CourseMeetingType.OfficeHour);
                    _dbContext.RemoveRange(meetings);
                }
            }
            else
            {
                var isDeleted = await _zoomApi.DeleteMeeting(meeting.ProviderMeetingId, occurenceId);

                LmsLicenseDto licenseDto = await _licenseAccessor.GetLicense();
                await RemoveLmsCalendarEventForMeeting(licenseDto, lmsSettings, meeting);
                await RemoveLmsCalendarEventsForMeetingSessions(licenseDto, lmsSettings, meeting);

                _dbContext.Remove(meeting);
            }
            await _dbContext.SaveChangesAsync();
            return OperationResult.Success();

        }

        public async Task<OperationResultWithData<MeetingViewModel>> CreateMeeting(Dictionary<string, object> lmsSettings, string courseId, UserInfoDto user, ILtiParam extraData,
            CreateMeetingViewModel requestDto)
        {
            LmsLicenseDto licenseDto = await _licenseAccessor.GetLicense();

            MeetingViewModel vm = null;
            LmsCourseMeeting dbOfficeHours = null;
            if (requestDto.Type.GetValueOrDefault(1) == (int)CourseMeetingType.OfficeHour)
            {
                var dbMeetings = _dbContext.LmsCourseMeetings.Where(x => x.LicenseKey == licenseDto.ConsumerKey && x.Type == (int)CourseMeetingType.OfficeHour);
                if (dbMeetings.Any(x => x.CourseId == courseId && x.ProviderHostId == user.Id))
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

            if (requestDto.Type == (int)CourseMeetingType.Basic)
            {
                var lmsEvent = await CreateLmsCalendarEvent(licenseDto, lmsSettings, courseId, requestDto);
                dbMeeting.LmsCalendarEventId = lmsEvent?.Id;
            }

            var entity = _dbContext.Add(dbMeeting);
            await _dbContext.SaveChangesAsync();

            if (requestDto.Type.GetValueOrDefault(1) != (int)CourseMeetingType.OfficeHour 
                && requestDto.Type.GetValueOrDefault(1) != (int)CourseMeetingType.StudyGroup
                && requestDto.Settings.ApprovalType.GetValueOrDefault() == 1) //manual approval(secure roster)
            {
                var lmsService = _lmsUserServiceFactory.GetUserService(licenseDto.ProductId);

                var lmsUsers = await lmsService.GetUsers(lmsSettings, courseId, extraData);

                if (!lmsUsers.IsSuccess)
                {
                    return OperationResultWithData<MeetingViewModel>.Error(lmsUsers.Message);
                }

                //take users by email, throwing out current user
                var registrants = lmsUsers.Data.Where(x =>
                    !String.IsNullOrEmpty(x.Email) &&
                    !x.Email.Equals(extraData.lis_person_contact_email_primary, StringComparison.InvariantCultureIgnoreCase)).Select(x =>
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
                    var registrants = requestDto.Participants.Where(x => !x.Email.Equals(extraData.lis_person_contact_email_primary, StringComparison.InvariantCultureIgnoreCase));
                    await _userService.RegisterUsersToMeetingAndApprove(dbMeeting.ProviderMeetingId, registrants, false);
                }
            }

            vm.Id = dbMeeting.Id;
            vm.Type = dbMeeting.Type;

            return vm.ToSuccessResult();
        }

        private async Task<LmsCalendarEventDTO> CreateLmsCalendarEvent(LmsLicenseDto licenseDto, Dictionary<string, object> lmsSettings, string courseId,
            CreateMeetingViewModel requestDto)
        {
            var date = LongToDateTime(requestDto.StartTime.Value);

            var lmsCalendarEvent = new LmsCalendarEventDTO(date, date.AddMinutes(requestDto.Duration.Value), requestDto.Topic);
            var calendarEventService = _lmsCalendarEventServiceFactory.GetService(licenseDto.ProductId, lmsSettings);

            if (calendarEventService == null)
                return null;

            try
            {
                var lmsEvent = await calendarEventService.CreateEvent(courseId, lmsSettings, lmsCalendarEvent);
                return lmsEvent;
            }
            catch (Exception e)
            {
                _logger.Error(e.Message);
                return null;
            }
        }

        private async Task RemoveLmsCalendarEventForMeeting(LmsLicenseDto licenseDto, Dictionary<string, object> lmsSettings, LmsCourseMeeting meeting)
        {
            var calendarEventService = _lmsCalendarEventServiceFactory.GetService(licenseDto.ProductId, lmsSettings);
            if (calendarEventService == null)
                return;

            try
            {
                if (!string.IsNullOrEmpty(meeting.LmsCalendarEventId))
                {
                    await calendarEventService.DeleteCalendarEvent(meeting.LmsCalendarEventId, lmsSettings);
                }
            }
            catch (Exception e)
            {
                _logger.Error(e.Message);
            }
        }

        private async Task RemoveLmsCalendarEventsForMeetingSessions(LmsLicenseDto licenseDto, Dictionary<string, object> lmsSettings, LmsCourseMeeting meeting)
        {
            if (meeting.MeetingSessions == null)
                return;
            var calendarEventService = _lmsCalendarEventServiceFactory.GetService(licenseDto.ProductId, lmsSettings);
            if (calendarEventService == null)
                return;

            try
            {
                foreach (var meetingSession in meeting.MeetingSessions.Where(s => !string.IsNullOrEmpty(s.LmsCalendarEventId)))
                {
                    await calendarEventService.DeleteCalendarEvent(meetingSession.LmsCalendarEventId, lmsSettings);
                }
            }
            catch(Exception e)
            {
                _logger.Error(e.Message);
            }
        }

        private static DateTime LongToDateTime(long unixDate)
        {
            DateTime start = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            DateTime date = start.AddMilliseconds(unixDate);
            return date;
        }

        private async Task<OperationResultWithData<Meeting>> CreateApiMeeting(UserInfoDto user, CreateMeetingViewModel dto)
        {
            var meetingDto = ConvertFromDto(dto);
            var offset = GetTimezoneOffset(user, meetingDto.StartTime);
            meetingDto.StartTime = meetingDto.StartTime.AddSeconds(offset);
            meetingDto.Timezone = user.Timezone;

            var result = string.IsNullOrEmpty(user.SubAccountid) 
                            ? await _zoomApi.CreateMeeting(user.Id, meetingDto)
                            : await _zoomApi.CreateMeeting(user.SubAccountid, user.Id, meetingDto);

            if (!result.IsSuccess)
            {
                return OperationResultWithData<Meeting>.Error(result.Message);
            }

            return result.Data.ToSuccessResult();
        }

        private async Task<OperationResultWithData<Meeting>> CreateApiMeeting(string accountId, UserInfoDto user, CreateMeetingViewModel dto)
        {
            var meetingDto = ConvertFromDto(dto);
            var offset = GetTimezoneOffset(user, meetingDto.StartTime);
            meetingDto.StartTime = meetingDto.StartTime.AddSeconds(offset);
            meetingDto.Timezone = user.Timezone;

            var result = await _zoomApi.CreateMeeting(accountId, user.Id, meetingDto);

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
            var updatedResult = await UpdateApiMeeting(dbMeeting.ProviderMeetingId, user, vm);
            if (!updatedResult.IsSuccess)
            {
                return OperationResultWithData<bool>.Error(updatedResult.Message);
            }

            if (updatedResult.Data)
            {
                if (dbMeeting.Type != (int) CourseMeetingType.StudyGroup)
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
                        await _userService.RegisterUsersToMeetingAndApprove(dbMeeting.ProviderMeetingId, registrants,
                            true);
                    }


                    await UpdateLmsCalendarEvent(licenseDto, licenseSettings, courseId, vm, dbMeeting);
                }

                if (dbMeeting.Type == (int)CourseMeetingType.StudyGroup)
                {
                    var registrants = await _userService.GetMeetingRegistrants(dbMeeting.ProviderMeetingId);
                    await _userService.CleanApprovedRegistrant(dbMeeting.ProviderMeetingId, vm.Participants, registrants);

                    await _userService.RegisterUsersToMeetingAndApprove(dbMeeting.ProviderMeetingId, vm.Participants, true);
                }

                return true.ToSuccessResult();
            }

            return OperationResultWithData<bool>.Error("Meeting has not been updated");
        }

        private async Task UpdateLmsCalendarEvent(LmsLicenseDto licenseDto, Dictionary<string, object> licenseSettings, string courseId, CreateMeetingViewModel vm, LmsCourseMeeting dbMeeting)
        {
            var calendarEventService = _lmsCalendarEventServiceFactory.GetService(licenseDto.ProductId, licenseSettings);
            if (calendarEventService == null)
                return;

            if (string.IsNullOrEmpty(dbMeeting.LmsCalendarEventId))
                return;

            var lmsCalendarEvent = new LmsCalendarEventDTO
            {
                Id = dbMeeting.LmsCalendarEventId,
                StartAt = LongToDateTime(vm.StartTime.Value),
                EndAt = LongToDateTime(vm.StartTime.Value).AddMinutes(vm.Duration.Value),
                Title = vm.Topic
            };

            try
            {
                await calendarEventService.UpdateEvent(courseId, licenseSettings, lmsCalendarEvent);
            }
            catch (Exception e)
            {
                _logger.Error(e.Message);
            }
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
                    AutoRecording = ConvertAutoRecordingFromEnum((AutomaticRecordingType)dto.Settings.RecordingType.Value),
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

        public async Task<OperationResultWithData<bool>> UpdateApiMeeting(string meetingId, UserInfoDto user, CreateMeetingViewModel dto)
        {
            var meeting = ConvertFromDto(dto);

            var offset = GetTimezoneOffset(user, meeting.StartTime);
            meeting.StartTime = meeting.StartTime.AddSeconds(offset);
            meeting.Timezone = user.Timezone;

            var updatedResult = await _zoomApi.UpdateMeeting(meetingId, meeting);

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

        private string ConvertAutoRecordingFromEnum(AutomaticRecordingType option)
        {
            switch (option)
            {
                case AutomaticRecordingType.None:
                    return AutoRecorgingOptions.None;
                case AutomaticRecordingType.Local:
                    return AutoRecorgingOptions.Local;
                case AutomaticRecordingType.Cloud:
                    return AutoRecorgingOptions.Cloud;
            }

            return AutoRecorgingOptions.None;
        }
    }
}