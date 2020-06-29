using System;
using System.Collections.Generic;

namespace EdugameCloud.Lti.SearchApi.Host.Models
{
    public partial class QuestionForSingleMultipleChoice
    {
        public int QuestionForSingleMultipleChoiceId { get; set; }
        public int QuestionId { get; set; }
        public bool? AllowOther { get; set; }
        public int? PageNumber { get; set; }
        public bool IsMandatory { get; set; }
        public string Restrictions { get; set; }

        public virtual Question Question { get; set; }
    }
}
