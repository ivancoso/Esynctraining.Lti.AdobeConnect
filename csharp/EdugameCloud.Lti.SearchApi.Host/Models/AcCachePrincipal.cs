using System;
using System.Collections.Generic;

namespace EdugameCloud.Lti.SearchApi.Host.Models
{
    public partial class AcCachePrincipal
    {
        public int AcCachePrincipalId { get; set; }
        public int LmsCompanyId { get; set; }
        public string AccountId { get; set; }
        public string DisplayId { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public bool? HasChildren { get; set; }
        public bool? IsHidden { get; set; }
        public bool? IsPrimary { get; set; }
        public string LastName { get; set; }
        public string Login { get; set; }
        public string Name { get; set; }
        public string PrincipalId { get; set; }
        public string Type { get; set; }

        public virtual CompanyLms LmsCompany { get; set; }
    }
}
