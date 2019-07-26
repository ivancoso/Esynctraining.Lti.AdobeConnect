using System;
using System.Collections.Generic;

namespace EdugameCloud.Lti.SearchApi.Host.Models
{
    public partial class QuestionForTrueFalse
    {
        public int QuestionForTrueFalseId { get; set; }
        public int QuestionId { get; set; }
        public int? PageNumber { get; set; }
        public bool IsMandatory { get; set; }

        public virtual Question Question { get; set; }
    }
}
