using System;
using System.Collections.Generic;

namespace EdugameCloud.Lti.SearchApi.Host.Models
{
    public partial class LmsMeetingSession
    {
        public int LmsMeetingSessionId { get; set; }
        public string LmsCalendarEventId { get; set; }
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int LmsCourseMeetingId { get; set; }
        public string Summary { get; set; }

        public virtual LmsCourseMeeting LmsCourseMeeting { get; set; }
    }
}
