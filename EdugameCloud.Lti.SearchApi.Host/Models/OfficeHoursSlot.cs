using System;
using System.Collections.Generic;

namespace EdugameClaud.Lti.SearchApi.Host.Models
{
    public partial class OfficeHoursSlot
    {
        public int OfficeHoursSlotId { get; set; }
        public int Status { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string Subject { get; set; }
        public string Questions { get; set; }
        public int? LmsUserId { get; set; }
        public int AvailabilityId { get; set; }

        public virtual OfficeHoursTeacherAvailability Availability { get; set; }
        public virtual LmsUser LmsUser { get; set; }
    }
}
