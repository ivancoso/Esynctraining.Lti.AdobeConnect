using System;
using System.Collections.Generic;

namespace EdugameClaud.Lti.SearchApi.Host.Models
{
    public partial class TimeZone
    {
        public TimeZone()
        {
            User = new HashSet<User>();
        }

        public int TimeZoneId { get; set; }
        public string TimeZone1 { get; set; }
        public double TimeZoneGmtdiff { get; set; }

        public virtual ICollection<User> User { get; set; }
    }
}
