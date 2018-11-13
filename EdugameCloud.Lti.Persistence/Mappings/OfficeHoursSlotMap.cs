using EdugameCloud.Lti.Domain.Entities;
using Esynctraining.Persistence.Mappings;

namespace EdugameCloud.Lti.Persistence.Mappings
{
    public sealed class OfficeHoursSlotMap : BaseClassMap<OfficeHoursSlot>
    {
        public OfficeHoursSlotMap()
        {
            Map(x => x.Status).Not.Nullable();
            Map(x => x.Start).Not.Nullable();
            Map(x => x.End).Not.Nullable();
            Map(x => x.Subject).Length(200).Nullable();
            Map(x => x.Questions).Length(2000).Nullable();
            References(x => x.User).Column("lmsUserId").Not.Nullable();
            References(x => x.Availability).Column("availabilityId").Not.Nullable();
        }
    }
}