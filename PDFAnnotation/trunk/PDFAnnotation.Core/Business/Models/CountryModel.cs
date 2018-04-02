using Esynctraining.NHibernate;

namespace PDFAnnotation.Core.Business.Models
{
    using PDFAnnotation.Core.Domain.Entities;

    public class CountryModel : BaseModel<Country, int>
    {
        public CountryModel(IRepository<Country, int> repository)
            : base(repository)
        {
        }

    }

}