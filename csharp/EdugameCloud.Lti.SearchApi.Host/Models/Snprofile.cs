using System;
using System.Collections.Generic;

namespace EdugameCloud.Lti.SearchApi.Host.Models
{
    public partial class Snprofile
    {
        public Snprofile()
        {
            Snlink = new HashSet<Snlink>();
            SnprofileSnservice = new HashSet<SnprofileSnservice>();
        }

        public int SnProfileId { get; set; }
        public string ProfileName { get; set; }
        public string UserName { get; set; }
        public string JobTitle { get; set; }
        public int? AddressId { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string About { get; set; }
        public int? SnMapSettingsId { get; set; }
        public int SubModuleItemId { get; set; }

        public virtual Address Address { get; set; }
        public virtual SnmapSettings SnMapSettings { get; set; }
        public virtual SubModuleItem SubModuleItem { get; set; }
        public virtual ICollection<Snlink> Snlink { get; set; }
        public virtual ICollection<SnprofileSnservice> SnprofileSnservice { get; set; }
    }
}
