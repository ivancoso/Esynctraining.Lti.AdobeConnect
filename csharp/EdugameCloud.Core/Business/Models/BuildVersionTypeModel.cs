namespace EdugameCloud.Core.Business.Models
{
    using EdugameCloud.Core.Domain.Entities;
    using Esynctraining.NHibernate;

    public class BuildVersionTypeModel : BaseModel<BuildVersionType, int>
    {
        public BuildVersionTypeModel(IRepository<BuildVersionType, int> repository)
            : base(repository)
        {
        }

    }

}