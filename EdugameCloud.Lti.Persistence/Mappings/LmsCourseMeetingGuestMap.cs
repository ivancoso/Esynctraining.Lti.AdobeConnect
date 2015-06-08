using Esynctraining.Persistence.Mappings;
using EdugameCloud.Lti.Domain.Entities;

namespace EdugameCloud.Lti.Persistence.Mappings
{
    public sealed class LmsCourseMeetingGuestMap : BaseClassMap<LmsCourseMeetingGuest>
    {
        public LmsCourseMeetingGuestMap()
        {
            this.References(x => x.LmsCourseMeeting).Column("lmsCourseMeetingId");
            this.Map(x => x.PrincipalId).Not.Nullable();
        }

    }

}
