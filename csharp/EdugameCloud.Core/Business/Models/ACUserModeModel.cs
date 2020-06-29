namespace EdugameCloud.Core.Business.Models
{
    using EdugameCloud.Core.Domain.Entities;
    using Esynctraining.NHibernate;

    public class ACUserModeModel : BaseModel<ACUserMode, int>
    {
        public ACUserModeModel(IRepository<ACUserMode, int> repository)
            : base(repository)
        {
        }

    }

}