using System;
using System.Collections.Generic;

namespace EdugameClaud.Lti.SearchApi.Host.Models
{
    public partial class Company
    {
        public Company()
        {
            CompanyAcDomains = new HashSet<CompanyAcDomains>();
            CompanyLicense = new HashSet<CompanyLicense>();
            CompanyLms = new HashSet<CompanyLms>();
            User = new HashSet<User>();
        }

        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public int? AddressId { get; set; }
        public int Status { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public int? PrimaryContactId { get; set; }
        public Guid? CompanyThemeId { get; set; }
        public bool? UseEventMapping { get; set; }

        public virtual Address Address { get; set; }
        public virtual CompanyTheme CompanyTheme { get; set; }
        public virtual User PrimaryContact { get; set; }
        public virtual ICollection<CompanyAcDomains> CompanyAcDomains { get; set; }
        public virtual ICollection<CompanyLicense> CompanyLicense { get; set; }
        public virtual ICollection<CompanyLms> CompanyLms { get; set; }
        public virtual ICollection<User> User { get; set; }
    }
}
