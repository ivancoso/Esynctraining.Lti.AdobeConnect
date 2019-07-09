using System;
using System.Collections.Generic;

namespace EdugameClaud.Lti.SearchApi.Host.Models
{
    public partial class LmsCompanySetting
    {
        public int LmsCompanySettingId { get; set; }
        public int LmsCompanyId { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }

        public virtual CompanyLms LmsCompany { get; set; }
    }
}
