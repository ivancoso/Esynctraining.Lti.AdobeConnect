using System;
using EdugameCloud.Lti.Domain.Entities;
using Esynctraining.Core.Domain.Entities;

namespace EdugameCloud.Lti.Core.Domain.Entities
{
    public class LmsMeetingSession : Entity
    {
        public virtual string EventId { get; set; }

        public virtual string Name { get; set; }

        public virtual DateTime StartDate { get; set; }

        public virtual DateTime EndDate { get; set; }

        public virtual LmsCourseMeeting LmsCourseMeeting { get; set; }

        public virtual string Summary { get; set; }

        public virtual int? LmsCalendarEventId { get; set; }

    }
}