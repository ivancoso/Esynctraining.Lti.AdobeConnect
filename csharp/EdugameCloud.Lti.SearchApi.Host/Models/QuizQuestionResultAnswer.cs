using System;
using System.Collections.Generic;

namespace EdugameCloud.Lti.SearchApi.Host.Models
{
    public partial class QuizQuestionResultAnswer
    {
        public int QuizQuestionResultAnswerId { get; set; }
        public int QuizQuestionResultId { get; set; }
        public string Value { get; set; }
        public int? QuizDistractorAnswerId { get; set; }

        public virtual Distractor QuizDistractorAnswer { get; set; }
        public virtual QuizQuestionResult QuizQuestionResult { get; set; }
    }
}
