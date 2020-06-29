using System;

namespace Esynctraining.Lti.Zoom.Common.Dto.OfficeHours
{
    public class RescheduleDateDto
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public long FirstSlotTimeshift { get; set; } //miliseconds
        public bool KeepRegistration { get; set; }
        //public bool SendNotification { get; set; }
        public string Message { get; set; }
    }
}