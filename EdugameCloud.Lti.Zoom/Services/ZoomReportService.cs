﻿using System;
using System.Collections.Generic;
using System.Linq;
using Edugamecloud.Lti.Zoom.Dto;
using Esynctraining.Zoom.ApiWrapper;
using Esynctraining.Zoom.ApiWrapper.Model;

namespace Edugamecloud.Lti.Zoom.Services
{
    public class ZoomReportService
    {
        private ZoomApiWrapper _zoomApi;

        public ZoomReportService(ZoomApiWrapper zoomApi)
        {
            _zoomApi = zoomApi;
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
                    EndedAt = s.EndTime.DateTime
                };
                var participants = _zoomApi.GetMeetingParticipantsReport(s.Uuid);
                var details = _zoomApi.GetMeetingParticipantsDetails(s.Uuid);
                sessionDto.Participants = participants.Participants.Select(x => new ZoomSessionParticipantDto
                {
                    ParticipantName = x.Name,
                    ParticipantEmail = x.Email,
                    EnteredAt = x.JoinTime.DateTime,
                    LeftAt = x.LeaveTime.DateTime,
                    Duration = x.Duration,
                    Score = x.AttentivenessScore,
                    Details = ConvertToDto(details.Participants.FirstOrDefault(d => d.UserId == x.UserId))
                }).ToList();

                result.Add(sessionDto);
                
            }
            return result;
        }

        private ZoomSessionParticipantDetailsDto ConvertToDto(ZoomMeetingParticipantDetails apiDetails)
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
                Version = apiDetails.Version
            };
        }
    }
}