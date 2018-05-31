using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Edugamecloud.Lti.Zoom.Dto;
using Edugamecloud.Lti.Zoom.Dto.Enums;
using Esynctraining.Zoom.ApiWrapper;
using Esynctraining.Zoom.ApiWrapper.Model;
using MeetingRegistrationTypes = Edugamecloud.Lti.Zoom.Dto.Enums.MeetingRegistrationTypes;

namespace Edugamecloud.Lti.Zoom.Services
{
    public class ZoomMeetingService
    {
        private ZoomApiWrapper _zoomApi;
        private ZoomUserService _userService;
        public ZoomMeetingService(ZoomApiWrapper zoomApi, ZoomUserService userService)
        {
            _zoomApi = zoomApi;
            _userService = userService;
        }

        public IEnumerable<MeetingDto> GetMeetings(string userId)
        {
            var meetings = _zoomApi.GetMeetings(userId, MeetingListTypes.Scheduled);

            return meetings.Meetings.Select(x => ConvertZoomObjToDto(x, userId));
        }

        public MeetingDetailsDto GetMeetingDetails(string meetingId)
        {
            var meeting = _zoomApi.GetMeeting(meetingId);
            var details = ConvertZoomObjToDetailsDto(meeting);
            return details;
        }

        public IEnumerable<MeetingDto> GetMeetings(Dictionary<string, IEnumerable<string>> pairs, string currentUserId = null) //key - userId, value - meetingIds
        {
            List<MeetingDto> result = new List<MeetingDto>();
            foreach (var user in pairs)
            {
                var userMeetings = _zoomApi.GetMeetings(user.Key);
                result.AddRange(userMeetings.Meetings.Where(m=> user.Value.Contains(m.Id)).Select(x => ConvertZoomObjToDto(x, currentUserId)));
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

        public bool DeleteMeeting(string meetingId, string occurenceId = null)
        {
            return _zoomApi.DeleteMeeting(meetingId, occurenceId);
            
        }

        public MeetingDto CreateMeeting(string userId, CreateZoomMeetingDto dto)
        {
            var meetingDto = new Meeting
            {
                Topic = dto.Topic,
                Type = (MeetingTypes)(int)dto.Type,
                Password = dto.Password,
                Agenda = dto.Agenda,
                Duration = dto.Duration,
                Timezone = dto.Timezone
            };

            if(dto.StartTime.HasValue)
                meetingDto.StartTime = new DateTimeOffset(dto.StartTime.Value);

            if (dto.Settings != null)
            {
                meetingDto.Settings = new MeetingSettings
                {
                    EnableJoinBeforeHost = dto.Settings.EnableJoinBeforeHost,
                    
                    ApprovalType = (MeetingApprovalTypes)(int)dto.Settings.ApprovalType,
                    AlternativeHosts = dto.Settings.AlternativeHosts,
                    Audio = ConvertAudioFromEnum(dto.Settings.AudioType),
                    AutoRecording = dto.Settings.RecordingType.ToString().ToLower(),
                    EnableHostVideo = dto.Settings.EnableHostVideo,
                    EnableMuteOnEntry = dto.Settings.EnableMuteOnEntry,
                    EnableParticipantVideo = dto.Settings.EnableParticipantVideo
                };
                if (dto.Settings.RegistrationType.HasValue)
                {
                    meetingDto.Settings.RegistrationType =
                        (Esynctraining.Zoom.ApiWrapper.Model.MeetingRegistrationTypes) (int) dto.Settings
                            .RegistrationType.Value;
                }
            }

            if (dto.Recurrence != null)
            {
                /*
                meetingDto.Recurrence = new MeetingRecurrence
                {
                    Type = MeetingRecurrenceTypes.Weekly,
                    WeeklyDaysList = new List<MeetingRecurrenceWeekDays>(dto.Recurrence.DaysOfWeek.Select(x => (MeetingRecurrenceWeekDays)x)),
                    RepeatInterval = 1,
                    EndDateTime = new DateTimeOffset(dto.StartTime.Value.AddDays(dto.Recurrence.Weeks * 7)),

                };
                */
            }

            var meeting = _zoomApi.CreateMeeting(userId, meetingDto);
            return ConvertZoomObjToDto(meeting, userId);

        }

        public bool UpdateMeeting(string meetingId, CreateZoomMeetingDto dto)
        {
            var meetingDto = new Meeting
            {
                Topic = dto.Topic,
                Type = (MeetingTypes)(int)dto.Type,
                Password = dto.Password,
                Agenda = dto.Agenda,
                Duration = dto.Duration,
                Timezone = dto.Timezone
            };

            if (dto.StartTime.HasValue)
                meetingDto.StartTime = new DateTimeOffset(dto.StartTime.Value);

            if (dto.Settings != null)
            {
                meetingDto.Settings = new MeetingSettings
                {
                    EnableJoinBeforeHost = dto.Settings.EnableJoinBeforeHost,

                    ApprovalType = (MeetingApprovalTypes)(int)dto.Settings.ApprovalType,
                    AlternativeHosts = dto.Settings.AlternativeHosts,
                    Audio = ConvertAudioFromEnum(dto.Settings.AudioType),
                    AutoRecording = dto.Settings.RecordingType.ToString().ToLower(),
                    EnableHostVideo = dto.Settings.EnableHostVideo,
                    EnableMuteOnEntry = dto.Settings.EnableMuteOnEntry,
                    EnableParticipantVideo = dto.Settings.EnableParticipantVideo
                };

                if (dto.Settings.RegistrationType.HasValue)
                {
                    meetingDto.Settings.RegistrationType =
                        (Esynctraining.Zoom.ApiWrapper.Model.MeetingRegistrationTypes)(int)dto.Settings
                            .RegistrationType.Value;
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
            bool updated = _zoomApi.UpdateMeeting(meetingId, meetingDto);
            return updated;

        }

        private MeetingDto ConvertZoomObjToDto(Meeting meeting, string userId)
        {
            return new MeetingDto
            {
                Duration = meeting.Duration,
                Id = meeting.Id,
                HostId = meeting.HostId,
                Timezone = meeting.Timezone,
                Topic = meeting.Topic,
                StartTime = meeting.StartTime.DateTime,
                CanEdit = meeting.HostId == userId,
                CanJoin = userId != null,
                HasSessions = meeting.Type == MeetingTypes.RecurringWithTime
            };
        }

        private MeetingDetailsDto ConvertZoomObjToDetailsDto(Meeting meeting)
        {
            MeetingRegistrationTypes? regType = null;
            if (meeting.Settings.RegistrationType.HasValue)
            {
                regType = (MeetingRegistrationTypes)(int)meeting.Settings.RegistrationType.Value;
            }
            return new MeetingDetailsDto
            {
                Duration = meeting.Duration,
                Id = meeting.Id,
                HostId = meeting.HostId,
                Timezone = meeting.Timezone,
                Topic = meeting.Topic,
                StartTime = meeting.StartTime.DateTime,
                Agenda = meeting.Agenda,
                JoinUrl = meeting.JoinUrl,
                Password = meeting.Password,
                Type = (ZoomMeetingType)(int)meeting.Type,
                Settings = new CreateMeetingSettingsDto
                {
                    RegistrationType = regType,
                    AlternativeHosts = meeting.Settings.AlternativeHosts,
                    EnableJoinBeforeHost = meeting.Settings.EnableJoinBeforeHost,
                    RecordingType = Enum.TryParse<AutomaticRecordingType>(meeting.Settings.AutoRecording, out AutomaticRecordingType recordingType) ? recordingType : AutomaticRecordingType.None,
                    EnableParticipantVideo = meeting.Settings.EnableParticipantVideo,
                    EnableMuteOnEntry = meeting.Settings.EnableMuteOnEntry,
                    AudioType = ConvertAudioToEnum(meeting.Settings.Audio),
                    EnableHostVideo = meeting.Settings.EnableHostVideo,
                    //EnableWaitingRoom = meeting.Settings.,
                    ApprovalType = (ApprovalTypes)(int)meeting.Settings.ApprovalType
                }
                
            };
        }

        //private MeetingDetailsDto ConvertZoomObjToDetailsDto(Meeting meeting)
        //{
        //    return new MeetingDetailsDto
        //    {
        //        Duration = meeting.Duration,
        //        Id = meeting.Id,
        //        Timezone = meeting.Timezone,
        //        Topic = meeting.Topic,
        //        StartTime = meeting.StartTime.DateTime,
        //        Agenda = meeting.Agenda,
        //        Password = meeting.Password,
        //        Type = (ZoomMeetingType)(int)meeting.Type,
        //        Settings = new CreateMeetingSettingsDto
        //        {
        //            RegistrationType = (MeetingRegistrationTypes)(int)meeting.Settings.RegistrationType,
        //            AlternativeHosts = meeting.Settings.AlternativeHosts,
        //            EnableJoinBeforeHost = meeting.Settings.EnableJoinBeforeHost,
        //            RecordingType = Enum.TryParse<AutomaticRecordingType>(meeting.Settings.AutoRecording, out AutomaticRecordingType recordingType) ? recordingType : AutomaticRecordingType.None,
        //            EnableParticipantVideo = meeting.Settings.EnableParticipantVideo,
        //            EnableMuteOnEntry = meeting.Settings.EnableMuteOnEntry,
        //            AudioType = ConvertAudioToEnum(meeting.Settings.Audio),
        //            EnableHostVideo = meeting.Settings.EnableHostVideo,
        //            //EnableWaitingRoom = meeting.Settings.,
        //            ApprovalType = (ApprovalTypes)(int)meeting.Settings.ApprovalType
        //        }

        //    };
        //}

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

    }
}