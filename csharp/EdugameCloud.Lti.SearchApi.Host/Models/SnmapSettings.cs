using System;
using System.Collections.Generic;

namespace EdugameCloud.Lti.SearchApi.Host.Models
{
    public partial class SnmapSettings
    {
        public SnmapSettings()
        {
            Snprofile = new HashSet<Snprofile>();
        }

        public int SnMapSettingsId { get; set; }
        public int? SnMapProviderId { get; set; }
        public int? ZoomLevel { get; set; }
        public int? CountryId { get; set; }

        public virtual Country Country { get; set; }
        public virtual SnmapProvider SnMapProvider { get; set; }
        public virtual ICollection<Snprofile> Snprofile { get; set; }
    }
}
