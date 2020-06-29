using System;
using System.Collections.Generic;

namespace EdugameCloud.Lti.SearchApi.Host.Models
{
    public partial class Language
    {
        public Language()
        {
            Acsession = new HashSet<Acsession>();
            User = new HashSet<User>();
        }

        public int LanguageId { get; set; }
        public string Language1 { get; set; }
        public string TwoLetterCode { get; set; }

        public virtual ICollection<Acsession> Acsession { get; set; }
        public virtual ICollection<User> User { get; set; }
    }
}
