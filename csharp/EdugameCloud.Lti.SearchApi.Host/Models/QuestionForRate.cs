using System;
using System.Collections.Generic;

namespace EdugameCloud.Lti.SearchApi.Host.Models
{
    public partial class QuestionForRate
    {
        public int QuestionForRateId { get; set; }
        public int QuestionId { get; set; }
        public string Restrictions { get; set; }
        public bool? AllowOther { get; set; }
        public int? PageNumber { get; set; }
        public bool IsMandatory { get; set; }
        public bool? IsAlwaysRateDropdown { get; set; }

        public virtual Question Question { get; set; }
    }
}
