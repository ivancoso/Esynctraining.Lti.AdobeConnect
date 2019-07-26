using System;
using System.Collections.Generic;

namespace EdugameCloud.Lti.SearchApi.Host.Models
{
    public partial class LmsCourseMeeting
    {
        public LmsCourseMeeting()
        {
            LmsCourseMeetingGuest = new HashSet<LmsCourseMeetingGuest>();
            LmsCourseMeetingRecording = new HashSet<LmsCourseMeetingRecording>();
            LmsCourseSection = new HashSet<LmsCourseSection>();
            LmsMeetingSession = new HashSet<LmsMeetingSession>();
            LmsUserMeetingRole = new HashSet<LmsUserMeetingRole>();
        }

        public int LmsCourseMeetingId { get; set; }
        public string CourseId { get; set; }
        public string ScoId { get; set; }
        public int CompanyLmsId { get; set; }
        public int? OfficeHoursId { get; set; }
        public int? OwnerId { get; set; }
        public int LmsMeetingTypeId { get; set; }
        public string MeetingNameJson { get; set; }
        public bool? Reused { get; set; }
        public int? SourceCourseMeetingId { get; set; }
        public string AudioProfileId { get; set; }
        public bool EnableDynamicProvisioning { get; set; }
        public string AudioProfileProvider { get; set; }
        public string LmsCalendarEventId { get; set; }

        public virtual CompanyLms CompanyLms { get; set; }
        public virtual LmsMeetingType LmsMeetingType { get; set; }
        public virtual OfficeHours OfficeHours { get; set; }
        public virtual LmsUser Owner { get; set; }
        public virtual ICollection<LmsCourseMeetingGuest> LmsCourseMeetingGuest { get; set; }
        public virtual ICollection<LmsCourseMeetingRecording> LmsCourseMeetingRecording { get; set; }
        public virtual ICollection<LmsCourseSection> LmsCourseSection { get; set; }
        public virtual ICollection<LmsMeetingSession> LmsMeetingSession { get; set; }
        public virtual ICollection<LmsUserMeetingRole> LmsUserMeetingRole { get; set; }
    }
}
