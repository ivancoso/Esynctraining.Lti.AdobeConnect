using System;
using System.Collections.Generic;

namespace EdugameCloud.Lti.SearchApi.Host.Models
{
    public partial class Address
    {
        public Address()
        {
            Company = new HashSet<Company>();
            Snprofile = new HashSet<Snprofile>();
        }

        public int AddressId { get; set; }
        public int? CountryId { get; set; }
        public int? StateId { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string Zip { get; set; }

        public virtual Country Country { get; set; }
        public virtual State State { get; set; }
        public virtual ICollection<Company> Company { get; set; }
        public virtual ICollection<Snprofile> Snprofile { get; set; }
    }
}
