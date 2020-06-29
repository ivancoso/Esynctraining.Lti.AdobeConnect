using System;

namespace EdugameCloud.Lti.Core.DTO.OfficeHours
{
    public class DenyDateDto
    {
        public int MeetingId { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string Message { get; set; }
    }
}