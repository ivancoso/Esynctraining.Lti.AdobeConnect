using EdugameCloud.Lti.Domain.Entities;
using Esynctraining.Persistence.Mappings;

namespace EdugameCloud.Lti.Persistence.Mappings
{
    public sealed class OfficeHoursTeacherAvailabilityMap : BaseClassMap<OfficeHoursTeacherAvailability>
    {
        public OfficeHoursTeacherAvailabilityMap()
        {
            Map(x => x.Duration).Not.Nullable();
            Map(x => x.Intervals).Length(1000).Not.Nullable();
            Map(x => x.DaysOfWeek).Length(20).Not.Nullable();
            Map(x => x.PeriodStart).Not.Nullable();
            Map(x => x.PeriodEnd).Not.Nullable();
            References(x => x.User).Column("lmsUserId").Not.Nullable();
            References(x => x.Meeting).Column("officeHoursId").Not.Nullable();
            HasMany(x => x.Slots).KeyColumn("availabilityId").Cascade.AllDeleteOrphan().Inverse();
        }
    }
}