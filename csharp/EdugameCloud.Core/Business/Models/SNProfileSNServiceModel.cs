namespace EdugameCloud.Core.Business.Models
{
    using EdugameCloud.Core.Domain.Entities;
    using Esynctraining.NHibernate;

    public class SNProfileSNServiceModel : BaseModel<SNProfileSNService, int>
    {
        public SNProfileSNServiceModel(IRepository<SNProfileSNService, int> repository)
            : base(repository)
        {
        }

    }

}