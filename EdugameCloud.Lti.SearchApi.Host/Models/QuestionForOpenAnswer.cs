using System;
using System.Collections.Generic;

namespace EdugameClaud.Lti.SearchApi.Host.Models
{
    public partial class QuestionForOpenAnswer
    {
        public int QuestionForOpenAnswerId { get; set; }
        public int QuestionId { get; set; }
        public string Restrictions { get; set; }
        public int? PageNumber { get; set; }
        public bool IsMandatory { get; set; }

        public virtual Question Question { get; set; }
    }
}
