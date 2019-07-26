using System;
using System.Collections.Generic;

namespace EdugameCloud.Lti.SearchApi.Host.Models
{
    public partial class Question
    {
        public Question()
        {
            Distractor = new HashSet<Distractor>();
            QuestionForLikert = new HashSet<QuestionForLikert>();
            QuestionForOpenAnswer = new HashSet<QuestionForOpenAnswer>();
            QuestionForRate = new HashSet<QuestionForRate>();
            QuestionForSingleMultipleChoice = new HashSet<QuestionForSingleMultipleChoice>();
            QuestionForTrueFalse = new HashSet<QuestionForTrueFalse>();
            QuestionForWeightBucket = new HashSet<QuestionForWeightBucket>();
            QuestionHistory = new HashSet<QuestionHistory>();
            QuizQuestionResult = new HashSet<QuizQuestionResult>();
            SurveyQuestionResult = new HashSet<SurveyQuestionResult>();
            TestQuestionResult = new HashSet<TestQuestionResult>();
        }

        public int QuestionId { get; set; }
        public int QuestionTypeId { get; set; }
        public int? SubModuleItemId { get; set; }
        public string Question1 { get; set; }
        public int QuestionOrder { get; set; }
        public string Instruction { get; set; }
        public string CorrectMessage { get; set; }
        public string CorrectReference { get; set; }
        public string IncorrectMessage { get; set; }
        public string Hint { get; set; }
        public int? CreatedBy { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public bool? IsActive { get; set; }
        public int ScoreValue { get; set; }
        public Guid? ImageId { get; set; }
        public int? LmsQuestionId { get; set; }
        public bool? IsMoodleSingle { get; set; }
        public int? LmsProviderId { get; set; }
        public bool? RandomizeAnswers { get; set; }
        public int? Rows { get; set; }
        public string HtmlText { get; set; }

        public virtual File Image { get; set; }
        public virtual LmsProvider LmsProvider { get; set; }
        public virtual QuestionType QuestionType { get; set; }
        public virtual SubModuleItem SubModuleItem { get; set; }
        public virtual ICollection<Distractor> Distractor { get; set; }
        public virtual ICollection<QuestionForLikert> QuestionForLikert { get; set; }
        public virtual ICollection<QuestionForOpenAnswer> QuestionForOpenAnswer { get; set; }
        public virtual ICollection<QuestionForRate> QuestionForRate { get; set; }
        public virtual ICollection<QuestionForSingleMultipleChoice> QuestionForSingleMultipleChoice { get; set; }
        public virtual ICollection<QuestionForTrueFalse> QuestionForTrueFalse { get; set; }
        public virtual ICollection<QuestionForWeightBucket> QuestionForWeightBucket { get; set; }
        public virtual ICollection<QuestionHistory> QuestionHistory { get; set; }
        public virtual ICollection<QuizQuestionResult> QuizQuestionResult { get; set; }
        public virtual ICollection<SurveyQuestionResult> SurveyQuestionResult { get; set; }
        public virtual ICollection<TestQuestionResult> TestQuestionResult { get; set; }
    }
}
