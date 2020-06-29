namespace EdugameCloud.Core.Business.Models
{
    using EdugameCloud.Core.Domain.Entities;
    using Esynctraining.NHibernate;

    public class BuildVersionModel : BaseModel<BuildVersion, int>
    {
        public BuildVersionModel(IRepository<BuildVersion, int> repository)
            : base(repository)
        {
        }

    }

}