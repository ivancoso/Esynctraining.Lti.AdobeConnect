namespace EdugameCloud.Core.Business.Models
{
    using EdugameCloud.Core.Domain.Entities;

    using Esynctraining.NHibernate;

    public class SurveyQuestionResultAnswerModel : BaseModel<SurveyQuestionResultAnswer, int>
    {
        public SurveyQuestionResultAnswerModel(IRepository<SurveyQuestionResultAnswer, int> repository)
            : base(repository)
        {
        }

    }

}