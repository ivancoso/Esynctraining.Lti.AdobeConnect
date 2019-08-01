using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Esynctraining.Lti.Zoom.Domain
{
    [Table("LmsCourseMeeting")]
    public class LmsCourseMeeting : BaseEntity
    {
        public Guid LicenseKey { get; set; }
        public int Type { get; set; }
        public string CourseId { get; set; }
        public string ProviderMeetingId { get; set; }
        public string ProviderHostId { get; set; }
        public bool Reused { get; set; }
        public string Details { get; set; }
        [MaxLength(50)]
        public string LmsCalendarEventId { get; set; }

        [MaxLength(50)]
        public string SubAccountId { get; set; }

        public virtual List<LmsMeetingSession> MeetingSessions { get; protected set; }

    }
}