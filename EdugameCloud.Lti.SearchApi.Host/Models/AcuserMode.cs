using System;
using System.Collections.Generic;

namespace EdugameCloud.Lti.SearchApi.Host.Models
{
    public partial class AcuserMode
    {
        public AcuserMode()
        {
            Acsession = new HashSet<Acsession>();
        }

        public int AcUserModeId { get; set; }
        public string UserMode { get; set; }
        public Guid? ImageId { get; set; }

        public virtual File Image { get; set; }
        public virtual ICollection<Acsession> Acsession { get; set; }
    }
}
