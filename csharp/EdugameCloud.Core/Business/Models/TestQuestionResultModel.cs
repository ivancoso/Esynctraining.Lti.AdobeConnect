namespace EdugameCloud.Core.Business.Models
{
    using EdugameCloud.Core.Domain.Entities;

    using Esynctraining.NHibernate;

    public class TestQuestionResultModel : BaseModel<TestQuestionResult, int>
    {
        public TestQuestionResultModel(IRepository<TestQuestionResult, int> repository)
            : base(repository)
        {
        }

    }

}