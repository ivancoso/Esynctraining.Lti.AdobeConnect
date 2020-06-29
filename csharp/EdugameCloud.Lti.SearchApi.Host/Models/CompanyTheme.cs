using System;
using System.Collections.Generic;

namespace EdugameCloud.Lti.SearchApi.Host.Models
{
    public partial class CompanyTheme
    {
        public CompanyTheme()
        {
            Company = new HashSet<Company>();
        }

        public Guid CompanyThemeId { get; set; }
        public string HeaderBackgroundColor { get; set; }
        public string ButtonColor { get; set; }
        public string ButtonTextColor { get; set; }
        public string GridHeaderTextColor { get; set; }
        public string GridHeaderBackgroundColor { get; set; }
        public string GridRolloverColor { get; set; }
        public Guid? LogoId { get; set; }
        public string LoginHeaderTextColor { get; set; }
        public string PopupHeaderBackgroundColor { get; set; }
        public string PopupHeaderTextColor { get; set; }
        public string QuestionColor { get; set; }
        public string QuestionHeaderColor { get; set; }
        public string WelcomeTextColor { get; set; }

        public virtual File Logo { get; set; }
        public virtual ICollection<Company> Company { get; set; }
    }
}
