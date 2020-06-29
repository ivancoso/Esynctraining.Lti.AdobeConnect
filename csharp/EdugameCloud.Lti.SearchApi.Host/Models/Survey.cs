using System;
using System.Collections.Generic;

namespace EdugameCloud.Lti.SearchApi.Host.Models
{
    public partial class Survey
    {
        public int SurveyId { get; set; }
        public int? SubModuleItemId { get; set; }
        public string SurveyName { get; set; }
        public string Description { get; set; }
        public int SurveyGroupingTypeId { get; set; }
        public int? LmsSurveyId { get; set; }
        public int? LmsProviderId { get; set; }

        public virtual LmsProvider LmsProvider { get; set; }
        public virtual SubModuleItem SubModuleItem { get; set; }
        public virtual SurveyGroupingType SurveyGroupingType { get; set; }
    }
}
