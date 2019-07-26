using System;
using System.Collections.Generic;

namespace EdugameCloud.Lti.SearchApi.Host.Models
{
    public partial class Module
    {
        public Module()
        {
            SubModule = new HashSet<SubModule>();
        }

        public int ModuleId { get; set; }
        public string ModuleName { get; set; }
        public DateTime DateCreated { get; set; }
        public bool? IsActive { get; set; }

        public virtual ICollection<SubModule> SubModule { get; set; }
    }
}
