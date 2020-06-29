using System;
using System.Collections.Generic;

namespace EdugameCloud.Lti.SearchApi.Host.Models
{
    public partial class OfficeHours
    {
        public OfficeHours()
        {
            LmsCourseMeeting = new HashSet<LmsCourseMeeting>();
            OfficeHoursTeacherAvailability = new HashSet<OfficeHoursTeacherAvailability>();
        }

        public int OfficeHoursId { get; set; }
        public string Hours { get; set; }
        public string ScoId { get; set; }
        public int LmsUserId { get; set; }

        public virtual LmsUser LmsUser { get; set; }
        public virtual ICollection<LmsCourseMeeting> LmsCourseMeeting { get; set; }
        public virtual ICollection<OfficeHoursTeacherAvailability> OfficeHoursTeacherAvailability { get; set; }
    }
}
