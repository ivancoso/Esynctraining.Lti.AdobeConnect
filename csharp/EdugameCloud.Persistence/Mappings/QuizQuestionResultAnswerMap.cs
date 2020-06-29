using EdugameCloud.Core.Domain.Entities;

namespace EdugameCloud.Persistence.Mappings
{
    public class QuizQuestionResultAnswerMap : BaseClassMap<QuizQuestionResultAnswer>
    {
        public QuizQuestionResultAnswerMap()
        {
            this.Map(x => x.Value).Not.Nullable();
            this.References(x => x.QuizDistractorAnswer).Column("quizDistractorAnswerId").Nullable();
            this.References(x => x.QuizQuestionResult).Not.Nullable();
        }
    }
}