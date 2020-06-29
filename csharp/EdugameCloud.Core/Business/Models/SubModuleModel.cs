namespace EdugameCloud.Core.Business.Models
{
    using EdugameCloud.Core.Domain.Entities;

    using Esynctraining.NHibernate;

    public class SubModuleModel : BaseModel<SubModule, int>
    {
        public SubModuleModel(IRepository<SubModule, int> repository)
            : base(repository)
        {
        }

    }

}