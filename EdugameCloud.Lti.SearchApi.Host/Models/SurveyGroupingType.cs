using System;
using System.Collections.Generic;

namespace EdugameClaud.Lti.SearchApi.Host.Models
{
    public partial class SurveyGroupingType
    {
        public SurveyGroupingType()
        {
            Survey = new HashSet<Survey>();
        }

        public int SurveyGroupingTypeId { get; set; }
        public string SurveyGroupingType1 { get; set; }

        public virtual ICollection<Survey> Survey { get; set; }
    }
}
