using System;

namespace Esynctraining.Lti.Zoom.Api.Dto.OfficeHours
{
    public class RescheduleSlotDto
    {
        public DateTime Start { get; set; }
        //public bool SendNotification { get; set; }
        public string Message { get; set; }
    }
}