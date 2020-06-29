using EdugameCloud.Lti.Domain.Entities;
using Esynctraining.Persistence.Mappings;

namespace EdugameCloud.Lti.Persistence.Mappings
{
    public sealed class LmsCourseSectionMap : BaseClassMap<LmsCourseSection>
    {
        #region Constructors and Destructors

        public LmsCourseSectionMap()
        {
            this.Map(x => x.LmsId).Column("lmsId").Length(50);
            this.Map(x => x.Name).Column("name").Length(256);

            this.References(x => x.Meeting).Column("lmsCourseMeetingId");
        }

        #endregion
    }
}