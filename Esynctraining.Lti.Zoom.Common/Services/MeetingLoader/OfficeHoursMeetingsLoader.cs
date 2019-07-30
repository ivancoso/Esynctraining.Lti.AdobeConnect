using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Esynctraining.Lti.Zoom.Common.Dto;
using Esynctraining.Lti.Zoom.Common.Dto.Enums;
using Esynctraining.Lti.Zoom.Domain;
using Esynctraining.Zoom.ApiWrapper;
using Esynctraining.Zoom.ApiWrapper.Model;
using Microsoft.EntityFrameworkCore;

namespace Esynctraining.Lti.Zoom.Common.Services.MeetingLoader
{
    public class OfficeHoursMeetingsLoader : MeetingsLoader
    {
        private readonly ZoomOfficeHoursService _ohService;

        public OfficeHoursMeetingsLoader(ZoomDbContext dbContext,
            LmsLicenseDto license, 
            string courseId, 
            ZoomApiWrapper zoomApi, 
            string currentUserId,
            ZoomOfficeHoursService ohService,
            UserInfoDto user,
            ZoomUserService zoomUserService) : base(dbContext, license, courseId, zoomApi, currentUserId, user, zoomUserService)
        {
            _ohService = ohService;
            CourseMeetingType = CourseMeetingType.OfficeHour;
        }

        protected override async Task<IEnumerable<MeetingViewModel>> ProcessMergedMeetings(List<MeetingViewModel> result)
        {
            if (result.Any())
            {
                foreach (var oh in result)
                {
                    var ohDetailsResult = await _zoomApi.GetMeeting(oh.ConferenceId);
                    if (!ohDetailsResult.IsSuccess)
                        throw new Exception(ohDetailsResult.Message);

                    oh.Description = ohDetailsResult.Data.Agenda;
                    oh.Availabilities = await _ohService.GetAvailabilities(oh.Id, _currentUserId);
                }
            }
            else
            {
                //getting current teacher's OH from other course
                var ohMeeting = await _dbContext.LmsCourseMeetings.FirstOrDefaultAsync(x =>
                    x.LicenseKey == _license.ConsumerKey && x.ProviderHostId == _currentUserId && x.Type == (int)CourseMeetingType.OfficeHour);
                if (ohMeeting != null)
                {
                    var ohDetailsResult = await _zoomApi.GetMeeting(ohMeeting.ProviderMeetingId);
                    if (!ohDetailsResult.IsSuccess)
                    {
                        var deleteErrorCodes = new List<ZoomApiErrorCodes>
                        {
                            ZoomApiErrorCodes.MeetingNotFound,
                            ZoomApiErrorCodes.UserNotFound,
                            ZoomApiErrorCodes.UserNotBelongToAccount
                        };

                        if (deleteErrorCodes.Any(x => (int)x == ohDetailsResult.Code))
                        {
                            _dbContext.Remove(ohMeeting);
                            await _dbContext.SaveChangesAsync();
                            return result;
                        }
                        else
                        {
                            throw new Exception(ohDetailsResult.Message);
                        }
                    }

                    var detailsVm = ConvertToDetailsViewModel(ohDetailsResult.Data);
                    detailsVm.Type = ohMeeting.Type;
                    detailsVm.Id = ohMeeting.Id;
                    var vm = ConvertFromDtoToOHViewModel(ohDetailsResult.Data, _currentUserId, ohMeeting.Type);
                    vm.Id = ohMeeting.Id;
                    vm.Details = detailsVm;
                    vm.Availabilities = await _ohService.GetAvailabilities(ohMeeting.Id, _currentUserId);
                    result.Add(vm);
                }
            }

            return result;
        }

        private OfficeHoursViewModel ConvertFromDtoToOHViewModel(Meeting dto, string userId, int type = (int)CourseMeetingType.Basic)
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
    }
}
