using System;
using System.Collections.Generic;

namespace EdugameClaud.Lti.SearchApi.Host.Models
{
    public partial class CompanyLicenseHistory
    {
        public int CompanyLicenseHistoryId { get; set; }
        public int CompanyLicenseId { get; set; }
        public int? CreatedBy { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }

        public virtual User ModifiedByNavigation { get; set; }
    }
}
