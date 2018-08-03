using System;
using System.Threading.Tasks;
using Esynctraining.Core.Logging;
using Esynctraining.Lti.Zoom.Api.Dto;
using Esynctraining.Lti.Zoom.Api.Dto.Enums;
using Esynctraining.Lti.Zoom.Domain;
using Esynctraining.Zoom.ApiWrapper;
using Esynctraining.Zoom.ApiWrapper.Model;
using NodaTime;

namespace Esynctraining.Lti.Zoom.Api.Services
{
    public class ZoomMeetingApiService
    {
        private readonly ZoomApiWrapper _zoomApi;
        private readonly ILogger _logger;

        public ZoomMeetingApiService(ZoomApiWrapper zoomApi, ILogger logger)
        {
            _zoomApi = zoomApi;
            _logger = logger;
        }

        public async Task<MeetingDetailsViewModel> GetMeetingApiDetails(LmsCourseMeeting dbMeeting)
        {
            var meeting = _zoomApi.GetMeeting(dbMeeting.ProviderMeetingId);
            //var details = ConvertZoomObjToDetailsDto(meeting);
            var result = ConvertToDetailsViewModel(meeting);
            result.Id = dbMeeting.Id;
            result.Type = dbMeeting.Type;
            return result;
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

        private MeetingAudioType ConvertAudioToEnum(string options)
        {
            if (options == MeetingAudioOptions.Voip)
                return MeetingAudioType.Computer;
            if (options == MeetingAudioOptions.Telephone)
                return MeetingAudioType.Telephone;
            return MeetingAudioType.Both;
        }

        private long GetUtcTime(Meeting meeting)
        {
            if (meeting == null)
                throw new ArgumentNullException(nameof(meeting));

            if (string.IsNullOrEmpty(meeting.Timezone))
            {
                _logger.Warn($"Timezone property is empty or null for. Url={meeting.JoinUrl}.");
                return meeting.StartTime.ToUnixTimeMilliseconds();
            }
            var timezone = DateTimeZoneProviders.Tzdb.GetZoneOrNull(meeting.Timezone);
            if (timezone == null)
            {
                _logger.Warn($"Timezone not found. Url={meeting.JoinUrl}, timezone={meeting.Timezone}");
                return meeting.StartTime.ToUnixTimeMilliseconds();
            }

            var instant = Instant.FromDateTimeOffset(meeting.StartTime);
            var offset = timezone.GetUtcOffset(instant);
            return meeting.StartTime.AddSeconds(offset.Seconds).ToUnixTimeMilliseconds();
        }

    }
}