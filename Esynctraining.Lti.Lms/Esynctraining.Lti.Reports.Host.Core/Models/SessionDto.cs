using System.Collections.Generic;

namespace Esynctraining.Lti.Reports.Host.Core.Models
{
    public class SessionDto
    {
        public string MeetingId { get; set; }
        public string SessionNumber { get; set; }
        public string SessionId { get; set; }
        public string DateStarted { get; set; }
        public string DateEnded { get; set; }
        public int ParticipantsCount { get; set; }
        public List<SessionParticipantDto> Participants { get; set; }
    }
}
