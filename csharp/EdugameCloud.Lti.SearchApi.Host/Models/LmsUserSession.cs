using System;
using System.Collections.Generic;

namespace EdugameCloud.Lti.SearchApi.Host.Models
{
    public partial class LmsUserSession
    {
        public Guid LmsUserSessionId { get; set; }
        public string SessionData { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateModified { get; set; }
        public int CompanyLmsId { get; set; }
        public int? LmsUserId { get; set; }
        public string LmsCourseId { get; set; }

        public virtual CompanyLms CompanyLms { get; set; }
        public virtual LmsUser LmsUser { get; set; }
    }
}
