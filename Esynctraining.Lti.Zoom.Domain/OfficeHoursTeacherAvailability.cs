using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Esynctraining.Lti.Zoom.Domain
{
    [Table("OfficeHoursTeacherAvailability")]
    public class OfficeHoursTeacherAvailability : BaseEntity
    {
        public string LmsUserId { get; set; } // LMS Id of user
        public int Duration { get; set; } // time slot, in minutes

        public string Intervals { get; set; } //UTC, in minutes from the beginning of day. Format - comma-ceparated array of intervals: "60-120,540-660"

        public string DaysOfWeek { get; set; } //comma-separated array of days: "1,2,5"
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }

        public virtual LmsCourseMeeting Meeting { get; set; }

        public virtual List<OfficeHoursSlot> Slots {get;set;}
    }

    [Table("OfficeHoursSlot")]
    public class OfficeHoursSlot : BaseEntity
    {
        public virtual OfficeHoursTeacherAvailability Availability { get; set; }
        public int Status { get; set; } // 0 - Free, 1 - Booked, 2 - NotAvailable
        public string LmsUserId { get; set; } // LMS Id of user
        public string RequesterName { get; set; } //student's full name
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string Subject { get; set; }
        public string Questions { get; set; }
    }
}