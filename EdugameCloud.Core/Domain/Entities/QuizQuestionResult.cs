using System.Collections.Generic;

namespace EdugameCloud.Core.Domain.Entities
{
    using Esynctraining.Core.Domain.Entities;

    public class QuizQuestionResult : Entity
    {
        public QuizQuestionResult()
        {
            Answers = new List<QuizQuestionResultAnswer>();
        }

        public virtual bool IsCorrect { get; set; }

        public virtual Question QuestionRef { get; set; }

        public virtual QuestionType QuestionType { get; set; }

        public virtual string Question { get; set; }

        public virtual QuizResult QuizResult { get; set; }

        public virtual IList<QuizQuestionResultAnswer> Answers { get; protected set; }

    }

}