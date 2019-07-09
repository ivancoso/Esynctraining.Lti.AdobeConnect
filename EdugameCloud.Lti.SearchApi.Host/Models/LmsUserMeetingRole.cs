using System;
using System.Collections.Generic;

namespace EdugameClaud.Lti.SearchApi.Host.Models
{
    public partial class LmsUserMeetingRole
    {
        public int LmsUserMeetingRoleId { get; set; }
        public int LmsUserId { get; set; }
        public int LmsCourseMeetingId { get; set; }
        public string LmsRole { get; set; }

        public virtual LmsCourseMeeting LmsCourseMeeting { get; set; }
        public virtual LmsUser LmsUser { get; set; }
    }
}
