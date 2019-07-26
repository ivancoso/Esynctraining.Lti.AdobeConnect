using System;
using System.Collections.Generic;

namespace EdugameCloud.Lti.SearchApi.Host.Models
{
    public partial class Snlink
    {
        public int SnLinkId { get; set; }
        public int SnProfileId { get; set; }
        public string LinkName { get; set; }
        public string LinkValue { get; set; }

        public virtual Snprofile SnProfile { get; set; }
    }
}
