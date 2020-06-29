using System;
using System.Collections.Generic;

namespace EdugameCloud.Lti.SearchApi.Host.Models
{
    public partial class LmsProvider
    {
        public LmsProvider()
        {
            CompanyLms = new HashSet<CompanyLms>();
            Distractor = new HashSet<Distractor>();
            LmsQuestionType = new HashSet<LmsQuestionType>();
            Question = new HashSet<Question>();
            Quiz = new HashSet<Quiz>();
            SubModuleCategory = new HashSet<SubModuleCategory>();
            Survey = new HashSet<Survey>();
        }

        public int LmsProviderId { get; set; }
        public string LmsProvider1 { get; set; }
        public string ShortName { get; set; }
        public string ConfigurationUrl { get; set; }
        public string UserGuideFileUrl { get; set; }

        public virtual ICollection<CompanyLms> CompanyLms { get; set; }
        public virtual ICollection<Distractor> Distractor { get; set; }
        public virtual ICollection<LmsQuestionType> LmsQuestionType { get; set; }
        public virtual ICollection<Question> Question { get; set; }
        public virtual ICollection<Quiz> Quiz { get; set; }
        public virtual ICollection<SubModuleCategory> SubModuleCategory { get; set; }
        public virtual ICollection<Survey> Survey { get; set; }
    }
}
