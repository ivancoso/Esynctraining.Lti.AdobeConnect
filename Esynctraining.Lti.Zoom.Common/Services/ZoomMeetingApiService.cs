using System;
using System.Threading.Tasks;
using Esynctraining.Core.Logging;
using Esynctraining.Lti.Zoom.Common.Dto;
using Esynctraining.Lti.Zoom.Common.Dto.Enums;
using Esynctraining.Lti.Zoom.Domain;
using Esynctraining.Zoom.ApiWrapper;
using Esynctraining.Zoom.ApiWrapper.Model;

namespace Esynctraining.Lti.Zoom.Common.Services
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
            
            var meeting = string.IsNullOrEmpty(dbMeeting.SubAccountId) 
                            ? await _zoomApi.GetMeeting(dbMeeting.ProviderMeetingId)
                            : await _zoomApi.GetMeeting(dbMeeting.SubAccountId, dbMeeting.ProviderMeetingId);

            if (!meeting.IsSuccess)
            {
                throw new Exception(meeting.Message);
            }
            //var details = ConvertZoomObjToDetailsDto(meeting);
            var result = ConvertToDetailsViewModel(meeting.Data);
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
                StartTime = dto.StartTime.ToUnixTimeMilliseconds(),
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
                    RecordingType = (int)ConvertAutoRecordingEnum(dto.Settings.AutoRecording),
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

        private AutomaticRecordingType ConvertAutoRecordingEnum(string option)
        {
            if (option == AutoRecorgingOptions.None)
                return AutomaticRecordingType.None;
            if (option == AutoRecorgingOptions.Local)
                return AutomaticRecordingType.Local;
            if (option == AutoRecorgingOptions.Cloud)
                return AutomaticRecordingType.Cloud;

                return AutomaticRecordingType.None;
        }
    }
}