namespace EdugameCloud.Core.Business.Models
{
    using EdugameCloud.Core.Domain.Entities;
    using Esynctraining.NHibernate;

    public class ModuleModel : BaseModel<Module, int>
    {
        public ModuleModel(IRepository<Module, int> repository)
            : base(repository)
        {
        }

    }

}