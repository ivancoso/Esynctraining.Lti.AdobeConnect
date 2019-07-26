using System;
using System.Collections.Generic;

namespace EdugameCloud.Lti.SearchApi.Host.Models
{
    public partial class SubModule
    {
        public SubModule()
        {
            SubModuleCategory = new HashSet<SubModuleCategory>();
        }

        public int SubModuleId { get; set; }
        public int ModuleId { get; set; }
        public string SubModuleName { get; set; }
        public DateTime DateCreated { get; set; }
        public bool? IsActive { get; set; }

        public virtual Module Module { get; set; }
        public virtual ICollection<SubModuleCategory> SubModuleCategory { get; set; }
    }
}
