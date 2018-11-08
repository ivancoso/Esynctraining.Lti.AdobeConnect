using System;
using System.Collections.Generic;

namespace EdugameCloud.Lti.Core.DTO.OfficeHours
{
    public class OfficeHoursTeacherAvailabilityDto
    {
        public int MeetingId { get; set; }
        public List<AvailabilityInterval> Intervals { get; set; }
        public int Duration { get; set; }

        // TRICK: to support JIL instead of DayOfWeek        
        public int[] DaysOfWeek { get; set; }

        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }
    }
}