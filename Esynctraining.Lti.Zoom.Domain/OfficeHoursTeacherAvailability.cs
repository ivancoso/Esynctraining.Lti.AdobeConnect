using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Esynctraining.Lti.Zoom.Domain
{
    [Table("OfficeHoursTeacherAvailability")]
    public class OfficeHoursTeacherAvailability : BaseEntity
    {
        public string LmsId { get; set; } // LMS Id of user
        public int Duration { get; set; } // time slot, in minutes
        public string Intervals { get; set; } //UTC, in minutes from the beginning of day. Format - comma-ceparated array of intervals: "60-120,540-660"
        public string DaysOfWeek { get; set; } //comma-separated array of days: "1,2,5"
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }
    }
}