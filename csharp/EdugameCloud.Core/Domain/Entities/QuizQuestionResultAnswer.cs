using Esynctraining.Core.Domain.Entities;

namespace EdugameCloud.Core.Domain.Entities
{
    public class QuizQuestionResultAnswer : Entity
    {
        public virtual string Value { get; set; }
        public virtual QuizQuestionResult QuizQuestionResult { get; set; }
        public virtual Distractor QuizDistractorAnswer { get; set; }
    }
}