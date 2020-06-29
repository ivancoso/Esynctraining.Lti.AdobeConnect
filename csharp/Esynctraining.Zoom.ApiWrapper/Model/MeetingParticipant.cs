using System;
using RestSharp.Deserializers;

namespace Esynctraining.Zoom.ApiWrapper.Model
{
    public class MeetingParticipant
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string Name { get; set; }

        [DeserializeAs(Name = "user_email")]
        public string Email { get; set; }

        public DateTimeOffset JoinTime { get; set; }

        public DateTimeOffset LeaveTime { get; set; }

        public int Duration { get; set; }

        public string AttentivenessScore { get; set; }
    }

    public class ZoomMeetingParticipantDetails
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Device { get; set; }
        public string IpAddress { get; set; }
        public string Location { get; set; }
        public string NetworkType { get; set; }
        public DateTimeOffset JoinTime { get; set; }
        public DateTimeOffset LeaveTime { get; set; }
        public bool ShareApplication { get; set; }
        public bool ShareDesktop { get; set; }
        public bool ShareWhiteboard { get; set; }
        public bool Recording { get; set; }
        public string PcName { get; set; }
        public string Domain { get; set; }
        public string MacAddr { get; set; }
        public string HarddiskId { get; set; }
        public string Version { get; set; }
    }
}