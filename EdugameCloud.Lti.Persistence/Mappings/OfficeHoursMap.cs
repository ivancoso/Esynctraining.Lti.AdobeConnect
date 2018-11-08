using Esynctraining.Persistence.Mappings;
using EdugameCloud.Lti.Domain.Entities;

namespace EdugameCloud.Lti.Persistence.Mappings
{
    public sealed class OfficeHoursMap : BaseClassMap<OfficeHours>
    {
        public OfficeHoursMap()
        {
            Map(x => x.Hours).Nullable();
            Map(x => x.ScoId).Not.Nullable();

            References(x => x.LmsUser).Not.Nullable();
            HasMany(x => x.Availabilities).KeyColumn("officeHoursId").Cascade.AllDeleteOrphan().Inverse();
        }
    }
}
