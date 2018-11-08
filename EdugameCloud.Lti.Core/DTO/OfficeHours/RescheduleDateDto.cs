using System;

namespace EdugameCloud.Lti.Core.DTO.OfficeHours
{
    public class RescheduleDateDto
    {
        public int MeetingId { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public long FirstSlotTimeshift { get; set; } //miliseconds
        public bool KeepRegistration { get; set; }
        //public bool SendNotification { get; set; }
        public string Message { get; set; }
    }
}