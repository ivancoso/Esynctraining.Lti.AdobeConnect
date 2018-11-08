using System;
using System.Collections.Generic;
using Esynctraining.Core.Domain.Entities;

namespace EdugameCloud.Lti.Domain.Entities
{
    public class OfficeHoursTeacherAvailability : Entity
    {
        public virtual LmsUser User { get; set; } // Teacher

        public virtual int Duration { get; set; } // time slot, in minutes

        public virtual string Intervals { get; set; } //UTC, in minutes from the beginning of day. Format - comma-ceparated array of intervals: "60-120,540-660"

        public virtual string DaysOfWeek { get; set; } //comma-separated array of days: "1,2,5"
        public virtual DateTime PeriodStart { get; set; }
        public virtual DateTime PeriodEnd { get; set; }

        public virtual OfficeHours Meeting { get; set; }

        public virtual IList<OfficeHoursSlot> Slots { get; set; }
    }
}