using System;
using System.Collections.Generic;

namespace EdugameClaud.Lti.SearchApi.Host.Models
{
    public partial class LmsCompanyRoleMapping
    {
        public int LmsCompanyRoleMappingId { get; set; }
        public int LmsCompanyId { get; set; }
        public string LmsRoleName { get; set; }
        public int AcRole { get; set; }
        public bool IsDefaultLmsRole { get; set; }
        public bool IsTeacherRole { get; set; }

        public virtual CompanyLms LmsCompany { get; set; }
    }
}
