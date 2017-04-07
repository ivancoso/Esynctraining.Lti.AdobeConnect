namespace EdugameCloud.Core.Business.Models
{
    using EdugameCloud.Core.Domain.Entities;
    using Esynctraining.NHibernate;

    public class AddressModel : BaseModel<Address, int>
    {
        public AddressModel(IRepository<Address, int> repository)
            : base(repository)
        {
        }

    }

}