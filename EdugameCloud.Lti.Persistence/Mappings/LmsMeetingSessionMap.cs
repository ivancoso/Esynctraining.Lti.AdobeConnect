using EdugameCloud.Lti.Core.Domain.Entities;
using Esynctraining.Persistence.Mappings;

namespace EdugameCloud.Lti.Persistence.Mappings
{
    public sealed class LmsMeetingSessionMap : BaseClassMap<LmsMeetingSession>
    {
        public LmsMeetingSessionMap()
        {
            this.Table("LmsCalendarEvent");
            this.Map(x => x.EventId).Not.Nullable().Length(50);
            this.Map(x => x.Name).Not.Nullable().Length(200);
            this.Map(x => x.StartDate).Not.Nullable();
            this.Map(x => x.EndDate).Not.Nullable();
            this.Map(x => x.Summary).Nullable().Length(2000);
            this.References(x => x.LmsCourseMeeting).Column("lmsCourseMeetingId");
        }
    }
}