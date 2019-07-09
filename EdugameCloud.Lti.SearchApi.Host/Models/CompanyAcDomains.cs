using System;
using System.Collections.Generic;

namespace EdugameClaud.Lti.SearchApi.Host.Models
{
    public partial class CompanyAcDomains
    {
        public CompanyAcDomains()
        {
            CompanyEventQuizMapping = new HashSet<CompanyEventQuizMapping>();
        }

        public int CompanyAcServerId { get; set; }
        public string AcServer { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool? IsDefault { get; set; }
        public int CompanyId { get; set; }

        public virtual Company Company { get; set; }
        public virtual ICollection<CompanyEventQuizMapping> CompanyEventQuizMapping { get; set; }
    }
}
