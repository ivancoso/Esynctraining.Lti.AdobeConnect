using System;
using System.Collections.Generic;

namespace EdugameCloud.Lti.SearchApi.Host.Models
{
    public partial class OfficeHoursTeacherAvailability
    {
        public OfficeHoursTeacherAvailability()
        {
            OfficeHoursSlot = new HashSet<OfficeHoursSlot>();
        }

        public int OfficeHoursTeacherAvailabilityId { get; set; }
        public int Duration { get; set; }
        public string Intervals { get; set; }
        public string DaysOfWeek { get; set; }
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }
        public int LmsUserId { get; set; }
        public int OfficeHoursId { get; set; }

        public virtual LmsUser LmsUser { get; set; }
        public virtual OfficeHours OfficeHours { get; set; }
        public virtual ICollection<OfficeHoursSlot> OfficeHoursSlot { get; set; }
    }
}
