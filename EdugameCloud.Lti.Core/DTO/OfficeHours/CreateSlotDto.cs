using System;

namespace EdugameCloud.Lti.Core.DTO.OfficeHours
{
    public class CreateSlotDto
    {
        public int MeetingId { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string Subject { get; set; }
        public string Questions { get; set; }
    }
}