using System;
using System.Collections.Generic;

namespace EdugameClaud.Lti.SearchApi.Host.Models
{
    public partial class SurveyQuestionResultAnswer
    {
        public int SurveyQuestionResultAnswerId { get; set; }
        public int SurveyQuestionResultId { get; set; }
        public int? SurveyDistractorId { get; set; }
        public string Value { get; set; }
        public int? SurveyDistractorAnswerId { get; set; }

        public virtual Distractor SurveyDistractor { get; set; }
        public virtual Distractor SurveyDistractorAnswer { get; set; }
        public virtual SurveyQuestionResult SurveyQuestionResult { get; set; }
    }
}
