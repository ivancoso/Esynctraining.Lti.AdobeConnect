using System;
using System.Collections.Generic;

namespace EdugameCloud.Lti.SearchApi.Host.Models
{
    public partial class Webinar
    {
        public int WebinarId { get; set; }
        public DateTime? WebinarDate { get; set; }
        public string WebinarHost { get; set; }
        public string WebinarDescription { get; set; }
    }
}
