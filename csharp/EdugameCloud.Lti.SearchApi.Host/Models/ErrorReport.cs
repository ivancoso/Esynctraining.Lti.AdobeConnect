using System;
using System.Collections.Generic;

namespace EdugameCloud.Lti.SearchApi.Host.Models
{
    public partial class ErrorReport
    {
        public int UserId { get; set; }
        public string Os { get; set; }
        public string FlashVersion { get; set; }
        public string Message { get; set; }
        public string ApplicationVersion { get; set; }
        public DateTime? DateCreated { get; set; }
    }
}
