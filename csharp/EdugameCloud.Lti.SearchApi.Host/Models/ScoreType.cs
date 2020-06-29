using System;
using System.Collections.Generic;

namespace EdugameCloud.Lti.SearchApi.Host.Models
{
    public partial class ScoreType
    {
        public ScoreType()
        {
            Quiz = new HashSet<Quiz>();
            Test = new HashSet<Test>();
        }

        public int ScoreTypeId { get; set; }
        public string ScoreType1 { get; set; }
        public DateTime DateCreated { get; set; }
        public bool? IsActive { get; set; }
        public string Prefix { get; set; }
        public int DefaultValue { get; set; }

        public virtual ICollection<Quiz> Quiz { get; set; }
        public virtual ICollection<Test> Test { get; set; }
    }
}
