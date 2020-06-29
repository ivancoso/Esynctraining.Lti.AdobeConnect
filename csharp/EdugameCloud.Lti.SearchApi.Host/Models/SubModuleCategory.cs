using System;
using System.Collections.Generic;

namespace EdugameCloud.Lti.SearchApi.Host.Models
{
    public partial class SubModuleCategory
    {
        public SubModuleCategory()
        {
            SubModuleItem = new HashSet<SubModuleItem>();
        }

        public int SubModuleCategoryId { get; set; }
        public int UserId { get; set; }
        public int SubModuleId { get; set; }
        public string CategoryName { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime DateModified { get; set; }
        public bool? IsActive { get; set; }
        public string LmsCourseId { get; set; }
        public int? LmsProviderId { get; set; }
        public int? CompanyLmsId { get; set; }

        public virtual CompanyLms CompanyLms { get; set; }
        public virtual LmsProvider LmsProvider { get; set; }
        public virtual User ModifiedByNavigation { get; set; }
        public virtual SubModule SubModule { get; set; }
        public virtual User User { get; set; }
        public virtual ICollection<SubModuleItem> SubModuleItem { get; set; }
    }
}
