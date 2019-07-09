using System;
using System.Collections.Generic;

namespace EdugameClaud.Lti.SearchApi.Host.Models
{
    public partial class LmsUserSession
    {
        public Guid LmsUserSessionId { get; set; }
        public string SessionData { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateModified { get; set; }
        public int CompanyLmsId { get; set; }
        public int? LmsUserId { get; set; }
        public int LmsCourseId { get; set; }
        public string ZoomAccessToken { get; set; }
        public string ZoomRefreshToken { get; set; }

        public virtual CompanyLms CompanyLms { get; set; }
        public virtual LmsUser LmsUser { get; set; }
    }
}
