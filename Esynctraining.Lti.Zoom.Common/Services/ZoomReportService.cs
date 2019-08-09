using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Esynctraining.Lti.Zoom.Common.Dto;
using Esynctraining.Lti.Zoom.Common.Dto.Reports;
using Esynctraining.Lti.Zoom.Domain;
using Esynctraining.Zoom.ApiWrapper;
using Esynctraining.Zoom.ApiWrapper.Model;

namespace Esynctraining.Lti.Zoom.Common.Services
{
    public class ZoomReportService
    {
        private readonly ZoomApiWrapper _zoomApi;


        public ZoomReportService(ZoomApiWrapper zoomApi)
        {
            _zoomApi = zoomApi ?? throw new ArgumentNullException(nameof(zoomApi));
        }

        public async Task<IEnumerable<ZoomSessionDto>> GetSessionsReport(LmsCourseMeeting meeting, UserInfoDto user, string sessionId = null, bool includeParticipants = false)
        {
            var allUserSessions = string.IsNullOrEmpty(user.SubAccountId) 
                                ? await _zoomApi.GetMeetingsReport(user.Id, DateTime.Now.AddDays(-30), DateTime.Now.AddDays(1))
                                : await _zoomApi.GetMeetingsReport(user.SubAccountId, user.Id, DateTime.Now.AddDays(-30), DateTime.Now.AddDays(1));

            var meetingSessions = allUserSessions.Meetings.Where(x => x.Id == meeting.ProviderMeetingId);
            if (sessionId != null)
            {
                meetingSessions = meetingSessions.Where(x => x.Uuid == sessionId);
            }

            var result = new List<ZoomSessionDto>();
            foreach (var s in meetingSessions)
            {
                var sessionDto = new ZoomSessionDto
                {
                    Duration = s.Duration,
                    SessionId = s.Uuid,
                    SessionNumber = "3",
                    StartedAt = s.StartTime.DateTime,
                    EndedAt = s.EndTime.DateTime,
                    Participants = includeParticipants ? (await GetParticipantsBySessionId(s.Uuid, meeting.SubAccountId)).ToList() : null
                };
                result.Add(sessionDto);
            }
            return result;
        }

        public async Task<IEnumerable<ZoomSessionParticipantDto>> GetParticipantsBySessionId(string sessionId, string accountId)
        {
            var participants = string.IsNullOrEmpty(accountId) 
                                ? await _zoomApi.GetMeetingParticipantsReport(sessionId)
                                : await _zoomApi.GetMeetingParticipantsReport(accountId, sessionId);

            var details = string.IsNullOrEmpty(accountId) 
                            ? await _zoomApi.GetMeetingParticipantsDetails(sessionId)
                            : await _zoomApi.GetMeetingParticipantsDetails(accountId, sessionId);

            List<ZoomSessionParticipantDto> result = participants.Participants.Select(x => new ZoomSessionParticipantDto
                {
                    ParticipantName = x.Name,
                    ParticipantEmail = x.Email,
                    EnteredAt = x.JoinTime.DateTime,
                    LeftAt = x.LeaveTime.DateTime,
                    Duration = x.Duration,
                    Score = x.AttentivenessScore,
                    Details = ConvertToDto(details.Participants.FirstOrDefault(d => d.UserId == x.UserId)),
                }).ToList();

            return result;
        }


        private static ZoomSessionParticipantDetailsDto ConvertToDto(ZoomMeetingParticipantDetails apiDetails)
        {
            if(apiDetails == null)
                return new ZoomSessionParticipantDetailsDto();
            return new ZoomSessionParticipantDetailsDto
            {
                Id = apiDetails.Id,
                Name = apiDetails.UserName,
                Device = apiDetails.Device,
                IpAddress = apiDetails.IpAddress,
                Location = apiDetails.Location,
                NetworkType = apiDetails.NetworkType,
                EnteredAt = apiDetails.JoinTime.DateTime,
                LeftAt = apiDetails.LeaveTime.DateTime,
                ShareApplication = apiDetails.ShareApplication,
                ShareDesktop = apiDetails.ShareDesktop,
                ShareWhiteboard = apiDetails.ShareWhiteboard,
                Recording = apiDetails.Recording,
                PcName = apiDetails.PcName,
                Domain = apiDetails.Domain,
                MacAddr = apiDetails.MacAddr,
                HarddiskId = apiDetails.HarddiskId,
                Version = apiDetails.Version,
            };
        }

    }

}