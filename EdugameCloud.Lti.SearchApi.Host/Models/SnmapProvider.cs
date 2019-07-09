using System;
using System.Collections.Generic;

namespace EdugameClaud.Lti.SearchApi.Host.Models
{
    public partial class SnmapProvider
    {
        public SnmapProvider()
        {
            SnmapSettings = new HashSet<SnmapSettings>();
        }

        public int SnMapProviderId { get; set; }
        public string MapProvider { get; set; }

        public virtual ICollection<SnmapSettings> SnmapSettings { get; set; }
    }
}
