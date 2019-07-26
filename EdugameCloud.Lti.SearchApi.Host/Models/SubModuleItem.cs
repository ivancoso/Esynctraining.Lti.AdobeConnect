using System;
using System.Collections.Generic;

namespace EdugameCloud.Lti.SearchApi.Host.Models
{
    public partial class SubModuleItem
    {
        public SubModuleItem()
        {
            Acsession = new HashSet<Acsession>();
            AppletItem = new HashSet<AppletItem>();
            Question = new HashSet<Question>();
            Quiz = new HashSet<Quiz>();
            Snprofile = new HashSet<Snprofile>();
            SubModuleItemTheme = new HashSet<SubModuleItemTheme>();
            Survey = new HashSet<Survey>();
            Test = new HashSet<Test>();
        }

        public int SubModuleItemId { get; set; }
        public int SubModuleCategoryId { get; set; }
        public int? CreatedBy { get; set; }
        public bool? IsShared { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public bool? IsActive { get; set; }

        public virtual SubModuleCategory SubModuleCategory { get; set; }
        public virtual ICollection<Acsession> Acsession { get; set; }
        public virtual ICollection<AppletItem> AppletItem { get; set; }
        public virtual ICollection<Question> Question { get; set; }
        public virtual ICollection<Quiz> Quiz { get; set; }
        public virtual ICollection<Snprofile> Snprofile { get; set; }
        public virtual ICollection<SubModuleItemTheme> SubModuleItemTheme { get; set; }
        public virtual ICollection<Survey> Survey { get; set; }
        public virtual ICollection<Test> Test { get; set; }
    }
}
