using System;
using System.Collections.Generic;
using System.Linq;
using Esynctraining.Lti.Zoom.Common.Dto.Sessions;
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


        public IEnumerable<ZoomSessionDto> GetSessionsReport(string meetingId, string userId, string sessionId = null)
        {
            var allUserSessions = _zoomApi.GetMeetingsReport(userId, DateTime.Now.AddDays(-30), DateTime.Now.AddDays(1));
            var meetingSessions = allUserSessions.Meetings.Where(x => x.Id == meetingId);
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
                    StartedAt = s.StartTime.DateTime,
                    EndedAt = s.EndTime.DateTime,
                };
                result.Add(sessionDto);
            }
            return result;
        }

        public IEnumerable<ZoomSessionParticipantDto> GetParticipantsBySessionId(string sessionId)
        {
            var participants = _zoomApi.GetMeetingParticipantsReport(sessionId);
            var details = _zoomApi.GetMeetingParticipantsDetails(sessionId);
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