using System;
using System.Collections.Generic;

namespace EdugameCloud.Lti.SearchApi.Host.Models
{
    public partial class BuildVersionType
    {
        public BuildVersionType()
        {
            BuildVersion = new HashSet<BuildVersion>();
        }

        public int BuildVersionTypeId { get; set; }
        public string BuildVersionType1 { get; set; }

        public virtual ICollection<BuildVersion> BuildVersion { get; set; }
    }
}
