using System;
using System.Collections.Generic;

namespace EdugameCloud.Lti.SearchApi.Host.Models
{
    public partial class CompanyLicense
    {
        public int CompanyLicenseId { get; set; }
        public int CompanyId { get; set; }
        public string LicenseNumber { get; set; }
        public string Domain { get; set; }
        public DateTime ExpiryDate { get; set; }
        public int CreatedBy { get; set; }
        public int ModifiedBy { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public int TotalLicensesCount { get; set; }
        public int LicenseStatus { get; set; }
        public DateTime DateStart { get; set; }
        public int TotalParticipantsCount { get; set; }
        public bool HasApi { get; set; }

        public virtual Company Company { get; set; }
        public virtual User CreatedByNavigation { get; set; }
        public virtual User ModifiedByNavigation { get; set; }
    }
}
