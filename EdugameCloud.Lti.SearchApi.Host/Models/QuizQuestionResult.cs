using System;
using System.Collections.Generic;

namespace EdugameClaud.Lti.SearchApi.Host.Models
{
    public partial class QuizQuestionResult
    {
        public QuizQuestionResult()
        {
            QuizQuestionResultAnswer = new HashSet<QuizQuestionResultAnswer>();
        }

        public int QuizQuestionResultId { get; set; }
        public int QuizResultId { get; set; }
        public int QuestionId { get; set; }
        public string Question { get; set; }
        public int QuestionTypeId { get; set; }
        public bool IsCorrect { get; set; }

        public virtual Question QuestionNavigation { get; set; }
        public virtual QuestionType QuestionType { get; set; }
        public virtual QuizResult QuizResult { get; set; }
        public virtual ICollection<QuizQuestionResultAnswer> QuizQuestionResultAnswer { get; set; }
    }
}
