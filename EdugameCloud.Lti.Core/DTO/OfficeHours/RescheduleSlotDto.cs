using System;

namespace EdugameCloud.Lti.Core.DTO.OfficeHours
{
    public class RescheduleSlotDto
    {
        public int SlotId { get; set; }
        public DateTime Start { get; set; }
        //public bool SendNotification { get; set; }
        public string Message { get; set; }
    }
}