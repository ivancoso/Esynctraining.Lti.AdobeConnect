using EdugameCloud.Lti.Core.Domain.Entities;
using EdugameCloud.Lti.Domain.Entities;
using Esynctraining.Persistence.Mappings;

namespace EdugameCloud.Lti.Persistence.Mappings
{
    public sealed class LmsCalendarEventMap : BaseClassMap<LmsCalendarEvent>
    {
        public LmsCalendarEventMap()
        {
            this.Map(x => x.EventId).Not.Nullable().Length(50);
            this.Map(x => x.Name).Not.Nullable().Length(200); ;
            this.Map(x => x.StartDate).Not.Nullable();
            this.Map(x => x.EndDate).Not.Nullable();
            this.References(x => x.LmsCourseMeeting).Column("lmsCourseMeetingId");
        }
    }
}