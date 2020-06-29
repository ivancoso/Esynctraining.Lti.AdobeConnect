using System;
using Esynctraining.Core.Domain.Entities;

namespace EdugameCloud.Lti.Domain.Entities
{
    public class OfficeHoursSlot : Entity
    {
        public virtual OfficeHoursTeacherAvailability Availability { get; set; }
        public virtual int Status { get; set; } // 0 - Free, 1 - Booked, 2 - NotAvailable
        public virtual LmsUser User { get; set; }
        //public string RequesterName { get; set; } //student's full name
        public virtual DateTime Start { get; set; }
        public virtual DateTime End { get; set; }
        public virtual string Subject { get; set; }
        public virtual string Questions { get; set; }
    }

    public enum OfficeHoursSlotStatus
    {
        Free = 0,
        Booked = 1,
        Cancelled = 2 //means that slot is cancelled and not available for selection
    }
}