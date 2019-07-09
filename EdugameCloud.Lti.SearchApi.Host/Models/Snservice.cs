using System;
using System.Collections.Generic;

namespace EdugameClaud.Lti.SearchApi.Host.Models
{
    public partial class Snservice
    {
        public Snservice()
        {
            SnprofileSnservice = new HashSet<SnprofileSnservice>();
        }

        public int SnServiceId { get; set; }
        public string SocialService { get; set; }

        public virtual ICollection<SnprofileSnservice> SnprofileSnservice { get; set; }
    }
}
