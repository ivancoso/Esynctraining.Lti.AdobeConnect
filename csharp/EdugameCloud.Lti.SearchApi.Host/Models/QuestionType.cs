using System;
using System.Collections.Generic;

namespace EdugameCloud.Lti.SearchApi.Host.Models
{
    public partial class QuestionType
    {
        public QuestionType()
        {
            LmsQuestionType = new HashSet<LmsQuestionType>();
            Question = new HashSet<Question>();
            QuizQuestionResult = new HashSet<QuizQuestionResult>();
            SurveyQuestionResult = new HashSet<SurveyQuestionResult>();
            TestQuestionResult = new HashSet<TestQuestionResult>();
        }

        public int QuestionTypeId { get; set; }
        public string Type { get; set; }
        public int? QuestionTypeOrder { get; set; }
        public string QuestionTypeDescription { get; set; }
        public string Instruction { get; set; }
        public string CorrectText { get; set; }
        public string IncorrectMessage { get; set; }
        public bool? IsActive { get; set; }
        public string IconSource { get; set; }

        public virtual ICollection<LmsQuestionType> LmsQuestionType { get; set; }
        public virtual ICollection<Question> Question { get; set; }
        public virtual ICollection<QuizQuestionResult> QuizQuestionResult { get; set; }
        public virtual ICollection<SurveyQuestionResult> SurveyQuestionResult { get; set; }
        public virtual ICollection<TestQuestionResult> TestQuestionResult { get; set; }
    }
}
