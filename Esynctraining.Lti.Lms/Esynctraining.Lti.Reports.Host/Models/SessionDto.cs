using System.Collections.Generic;

namespace EdugameCloud.Lti.Reports.Host.Models
{
    public class SessionDto
    {
        public string MeetingId { get; set; }
        public string SessionId { get; set; }
        public string DateStarted { get; set; }
        public string DateEnded { get; set; }
        public int ParticipantsCount{ get; set; }
        public List<SessionParticipantDto> Participants { get; set; }
    }
}