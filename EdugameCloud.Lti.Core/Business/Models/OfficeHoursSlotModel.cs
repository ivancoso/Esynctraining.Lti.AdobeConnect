using EdugameCloud.Lti.Domain.Entities;
using Esynctraining.NHibernate;

namespace EdugameCloud.Lti.Core.Business.Models
{
    public sealed class OfficeHoursSlotModel : BaseModel<OfficeHoursSlot, int>
    {
        public OfficeHoursSlotModel(IRepository<OfficeHoursSlot, int> repository) : base(repository)
        {
        }
    }
}