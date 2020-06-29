using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Esynctraining.Lti.Zoom.Domain
{
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