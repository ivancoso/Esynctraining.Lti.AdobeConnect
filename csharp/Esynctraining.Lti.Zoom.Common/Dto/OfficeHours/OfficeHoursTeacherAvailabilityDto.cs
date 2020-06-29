using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Esynctraining.Lti.Zoom.Common.Dto.OfficeHours
{
    public class OfficeHoursTeacherAvailabilityDto
    {

        public List<AvailabilityInterval> Intervals { get; set; }
        public int Duration { get; set; }

        // TRICK: to support JIL instead of DayOfWeek
        [MinLength(1, ErrorMessage = "At least one day of week must be checked")]
        public int[] DaysOfWeek { get; set; }

        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }
    }
}