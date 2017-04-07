namespace EdugameCloud.Core.Business.Models
{
    using EdugameCloud.Core.Domain.Entities;
    using Esynctraining.NHibernate;

    public class SNMapSettingsModel : BaseModel<SNMapSettings, int>
    {
        public SNMapSettingsModel(IRepository<SNMapSettings, int> repository)
            : base(repository)
        {
        }

    }

}