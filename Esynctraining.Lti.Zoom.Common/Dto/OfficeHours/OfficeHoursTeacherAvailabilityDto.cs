using System;
using System.Collections.Generic;

namespace Esynctraining.Lti.Zoom.Common.Dto.OfficeHours
{
    public class OfficeHoursTeacherAvailabilityDto
    {

        public List<AvailabilityInterval> Intervals { get; set; }
        public int Duration { get; set; }

        // TRICK: to support JIL instead of DayOfWeek        
        public int[] DaysOfWeek { get; set; }

        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }
    }
}