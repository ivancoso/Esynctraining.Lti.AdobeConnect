using System;
using System.Collections.Generic;

namespace EdugameClaud.Lti.SearchApi.Host.Models
{
    public partial class TestQuestionResult
    {
        public int TestQuestionResultId { get; set; }
        public int TestResultId { get; set; }
        public int QuestionId { get; set; }
        public string Question { get; set; }
        public int QuestionTypeId { get; set; }
        public bool IsCorrect { get; set; }

        public virtual Question QuestionNavigation { get; set; }
        public virtual QuestionType QuestionType { get; set; }
        public virtual TestResult TestResult { get; set; }
    }
}
