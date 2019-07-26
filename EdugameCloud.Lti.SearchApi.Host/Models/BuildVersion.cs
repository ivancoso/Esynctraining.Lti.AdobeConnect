using System;
using System.Collections.Generic;

namespace EdugameCloud.Lti.SearchApi.Host.Models
{
    public partial class BuildVersion
    {
        public int BuildVersionId { get; set; }
        public string BuildNumber { get; set; }
        public int BuildVersionTypeId { get; set; }
        public bool IsActive { get; set; }
        public string DescriptionSmall { get; set; }
        public string DescriptionHtml { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public Guid? FileId { get; set; }

        public virtual BuildVersionType BuildVersionType { get; set; }
        public virtual File File { get; set; }
    }
}
