using System;
using System.Collections.Generic;

namespace EdugameCloud.Lti.SearchApi.Host.Models
{
    public partial class SnprofileSnservice
    {
        public int SnProfileSnserviceId { get; set; }
        public int SnProfileId { get; set; }
        public int SnServiceId { get; set; }
        public bool IsEnabled { get; set; }
        public string ServiceUrl { get; set; }

        public virtual Snprofile SnProfile { get; set; }
        public virtual Snservice SnService { get; set; }
    }
}
