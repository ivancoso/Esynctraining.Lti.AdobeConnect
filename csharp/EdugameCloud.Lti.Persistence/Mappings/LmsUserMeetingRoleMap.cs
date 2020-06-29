using EdugameCloud.Lti.Core.Domain.Entities;
using Esynctraining.Persistence.Mappings;

namespace EdugameCloud.Lti.Persistence.Mappings
{
    public sealed class LmsUserMeetingRoleMap : BaseClassMap<LmsUserMeetingRole>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LmsUserParametersMap"/> class.
        /// </summary>
        public LmsUserMeetingRoleMap()
        {
            this.Map(x => x.LmsRole).Column("lmsRole").Length(50);

            this.References(x => x.User).Column("lmsUserId");
            this.References(x => x.Meeting).Column("lmsCourseMeetingId");
        }

        #endregion
    }
}