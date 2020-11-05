using Esynctraining.NHibernate;

namespace PDFAnnotation.Core.Business.Models
{
    using PDFAnnotation.Core.Domain.Entities;

    public class StateModel : BaseModel<State, int>
    {
        public StateModel(IRepository<State, int> repository)
            : base(repository)
        {
        }

    }

}