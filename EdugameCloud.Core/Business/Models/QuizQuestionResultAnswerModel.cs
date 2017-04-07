using EdugameCloud.Core.Domain.Entities;
using Esynctraining.NHibernate;

namespace EdugameCloud.Core.Business.Models
{
    public class QuizQuestionResultAnswerModel : BaseModel<QuizQuestionResultAnswer, int>
    {
        public QuizQuestionResultAnswerModel(IRepository<QuizQuestionResultAnswer, int> repository)
            : base(repository)
        {
        }

    }

}