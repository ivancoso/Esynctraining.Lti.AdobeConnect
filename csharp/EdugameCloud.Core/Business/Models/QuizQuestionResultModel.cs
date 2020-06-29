namespace EdugameCloud.Core.Business.Models
{
    using EdugameCloud.Core.Domain.Entities;
    using Esynctraining.NHibernate;

    public class QuizQuestionResultModel : BaseModel<QuizQuestionResult, int>
    {
        public QuizQuestionResultModel(IRepository<QuizQuestionResult, int> repository)
            : base(repository)
        {
        }

    }

}