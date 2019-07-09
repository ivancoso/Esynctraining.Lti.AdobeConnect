using System;
using System.Collections.Generic;

namespace EdugameClaud.Lti.SearchApi.Host.Models
{
    public partial class LmsCourseMeetingGuest
    {
        public int LmsCourseMeetingGuestId { get; set; }
        public int LmsCourseMeetingId { get; set; }
        public string PrincipalId { get; set; }

        public virtual LmsCourseMeeting LmsCourseMeeting { get; set; }
    }
}
