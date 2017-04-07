namespace EdugameCloud.Core.Business.Models
{
    using EdugameCloud.Core.Domain.Entities;
    using Esynctraining.NHibernate;

    public class SNLinkModel : BaseModel<SNLink, int>
    {
        public SNLinkModel(IRepository<SNLink, int> repository)
            : base(repository)
        {
        }

    }

}