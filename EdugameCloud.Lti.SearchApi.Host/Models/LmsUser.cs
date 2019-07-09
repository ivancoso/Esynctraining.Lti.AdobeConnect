using System;
using System.Collections.Generic;

namespace EdugameClaud.Lti.SearchApi.Host.Models
{
    public partial class LmsUser
    {
        public LmsUser()
        {
            CompanyLms = new HashSet<CompanyLms>();
            LmsCourseMeeting = new HashSet<LmsCourseMeeting>();
            LmsUserMeetingRole = new HashSet<LmsUserMeetingRole>();
            LmsUserParameters = new HashSet<LmsUserParameters>();
            LmsUserSession = new HashSet<LmsUserSession>();
            OfficeHoursSlot = new HashSet<OfficeHoursSlot>();
            OfficeHoursTeacherAvailability = new HashSet<OfficeHoursTeacherAvailability>();
        }

        public int LmsUserId { get; set; }
        public int CompanyLmsId { get; set; }
        public string UserId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Token { get; set; }
        public int? AcConnectionMode { get; set; }
        public string PrimaryColor { get; set; }
        public string PrincipalId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string UserIdExtended { get; set; }
        public string SharedKey { get; set; }
        public string AcPasswordData { get; set; }

        public virtual CompanyLms CompanyLmsNavigation { get; set; }
        public virtual OfficeHours OfficeHours { get; set; }
        public virtual ICollection<CompanyLms> CompanyLms { get; set; }
        public virtual ICollection<LmsCourseMeeting> LmsCourseMeeting { get; set; }
        public virtual ICollection<LmsUserMeetingRole> LmsUserMeetingRole { get; set; }
        public virtual ICollection<LmsUserParameters> LmsUserParameters { get; set; }
        public virtual ICollection<LmsUserSession> LmsUserSession { get; set; }
        public virtual ICollection<OfficeHoursSlot> OfficeHoursSlot { get; set; }
        public virtual ICollection<OfficeHoursTeacherAvailability> OfficeHoursTeacherAvailability { get; set; }
    }
}
