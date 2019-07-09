using System;
using System.Collections.Generic;

namespace EdugameClaud.Lti.SearchApi.Host.Models
{
    public partial class SurveyQuestionResult
    {
        public SurveyQuestionResult()
        {
            SurveyQuestionResultAnswer = new HashSet<SurveyQuestionResultAnswer>();
        }

        public int SurveyQuestionResultId { get; set; }
        public int SurveyResultId { get; set; }
        public int QuestionId { get; set; }
        public string Question { get; set; }
        public int QuestionTypeId { get; set; }
        public bool IsCorrect { get; set; }

        public virtual Question QuestionNavigation { get; set; }
        public virtual QuestionType QuestionType { get; set; }
        public virtual SurveyResult SurveyResult { get; set; }
        public virtual ICollection<SurveyQuestionResultAnswer> SurveyQuestionResultAnswer { get; set; }
    }
}
