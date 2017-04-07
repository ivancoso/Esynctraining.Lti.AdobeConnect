namespace EdugameCloud.Core.Business.Models
{
    using EdugameCloud.Core.Domain.Entities;

    using Esynctraining.NHibernate;

    public class SurveyQuestionResultModel : BaseModel<SurveyQuestionResult, int>
    {
        public SurveyQuestionResultModel(IRepository<SurveyQuestionResult, int> repository)
            : base(repository)
        {
        }

    }

}