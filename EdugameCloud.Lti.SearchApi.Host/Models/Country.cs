using System;
using System.Collections.Generic;

namespace EdugameCloud.Lti.SearchApi.Host.Models
{
    public partial class Country
    {
        public Country()
        {
            Address = new HashSet<Address>();
            SnmapSettings = new HashSet<SnmapSettings>();
        }

        public int CountryId { get; set; }
        public string CountryCode { get; set; }
        public string CountryCode3 { get; set; }
        public string Country1 { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public int ZoomLevel { get; set; }

        public virtual ICollection<Address> Address { get; set; }
        public virtual ICollection<SnmapSettings> SnmapSettings { get; set; }
    }
}
