using System;
using System.Collections.Generic;

namespace EdugameClaud.Lti.SearchApi.Host.Models
{
    public partial class LmsQuestionType
    {
        public int LmsQuestionTypeId { get; set; }
        public int LmsProviderId { get; set; }
        public int QuestionTypeId { get; set; }
        public string LmsQuestionType1 { get; set; }
        public int? SubModuleId { get; set; }

        public virtual LmsProvider LmsProvider { get; set; }
        public virtual QuestionType QuestionType { get; set; }
    }
}
