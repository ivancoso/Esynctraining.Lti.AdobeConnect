using System;
using System.Collections.Generic;

namespace EdugameCloud.Lti.SearchApi.Host.Models
{
    public partial class SngroupDiscussion
    {
        public int SnGroupDiscussionId { get; set; }
        public int AcSessionId { get; set; }
        public string GroupDiscussionData { get; set; }
        public string GroupDiscussionTitle { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateModified { get; set; }
        public bool IsActive { get; set; }

        public virtual Acsession AcSession { get; set; }
    }
}
