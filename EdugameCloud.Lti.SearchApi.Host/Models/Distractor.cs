using System;
using System.Collections.Generic;

namespace EdugameClaud.Lti.SearchApi.Host.Models
{
    public partial class Distractor
    {
        public Distractor()
        {
            DistractorHistory = new HashSet<DistractorHistory>();
            QuizQuestionResultAnswer = new HashSet<QuizQuestionResultAnswer>();
            SurveyQuestionResultAnswerSurveyDistractor = new HashSet<SurveyQuestionResultAnswer>();
            SurveyQuestionResultAnswerSurveyDistractorAnswer = new HashSet<SurveyQuestionResultAnswer>();
        }

        public int DistractorId { get; set; }
        public int? QuestionId { get; set; }
        public string Distractor1 { get; set; }
        public int DistractorOrder { get; set; }
        public string Score { get; set; }
        public bool? IsCorrect { get; set; }
        public int? CreatedBy { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public bool? IsActive { get; set; }
        public int? DistractorType { get; set; }
        public Guid? ImageId { get; set; }
        public string LmsAnswer { get; set; }
        public int? LmsProviderId { get; set; }
        public int? LmsAnswerId { get; set; }
        public Guid? LeftImageId { get; set; }
        public Guid? RightImageId { get; set; }

        public virtual User CreatedByNavigation { get; set; }
        public virtual File Image { get; set; }
        public virtual File LeftImage { get; set; }
        public virtual LmsProvider LmsProvider { get; set; }
        public virtual User ModifiedByNavigation { get; set; }
        public virtual Question Question { get; set; }
        public virtual File RightImage { get; set; }
        public virtual ICollection<DistractorHistory> DistractorHistory { get; set; }
        public virtual ICollection<QuizQuestionResultAnswer> QuizQuestionResultAnswer { get; set; }
        public virtual ICollection<SurveyQuestionResultAnswer> SurveyQuestionResultAnswerSurveyDistractor { get; set; }
        public virtual ICollection<SurveyQuestionResultAnswer> SurveyQuestionResultAnswerSurveyDistractorAnswer { get; set; }
    }
}
