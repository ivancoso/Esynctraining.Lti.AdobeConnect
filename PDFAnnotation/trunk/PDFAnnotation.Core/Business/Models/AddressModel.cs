using Esynctraining.NHibernate;

namespace PDFAnnotation.Core.Business.Models
{
    using PDFAnnotation.Core.Domain.Entities;

    public class AddressModel : BaseModel<Address, int>
    {
        public AddressModel(IRepository<Address, int> repository)
            : base(repository)
        {
        }

    }

}