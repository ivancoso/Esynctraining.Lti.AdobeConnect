using System;
using System.Collections.Generic;

namespace EdugameCloud.Lti.SearchApi.Host.Models
{
    public partial class AppletItem
    {
        public AppletItem()
        {
            AppletResult = new HashSet<AppletResult>();
        }

        public int AppletItemId { get; set; }
        public int? SubModuleItemId { get; set; }
        public string AppletName { get; set; }
        public string DocumentXml { get; set; }

        public virtual SubModuleItem SubModuleItem { get; set; }
        public virtual ICollection<AppletResult> AppletResult { get; set; }
    }
}
