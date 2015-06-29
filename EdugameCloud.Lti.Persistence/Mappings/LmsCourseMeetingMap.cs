using Esynctraining.Persistence.Mappings;
using EdugameCloud.Lti.Domain.Entities;

namespace EdugameCloud.Lti.Persistence.Mappings
{
    /// <summary>
    /// The canvas AC meeting map
    /// </summary>
    public sealed class LmsCourseMeetingMap : BaseClassMap<LmsCourseMeeting>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LmsCourseMeetingMap"/> class.
        /// </summary>
        public LmsCourseMeetingMap()
        {
            //this.OptimisticLock.Dirty();
            //this.DynamicUpdate();
            this.Map(x => x.CourseId).Not.Nullable();
            this.Map(x => x.ScoId).Not.Nullable();
            this.Map(x => x.CachedUsers).CustomType("StringClob").CustomSqlType("nvarchar(max)").Nullable();
            this.Map(x => x.MeetingNameJson).Length(4000).Nullable();
            this.Map(x => x.AddedToCache).Nullable();
            this.Map(x => x.LmsMeetingType).Column("lmsMeetingTypeId").Not.Nullable();

            this.References(x => x.LmsCompany).Column("companyLmsId").Not.Nullable();
            this.References(x => x.OfficeHours).Nullable();
            this.References(x => x.Owner).Column("ownerId").Nullable();
            HasMany(x => x.MeetingRoles).KeyColumn("lmsCourseMeetingId").Cascade.AllDeleteOrphan().Inverse();
            HasMany(x => x.MeetingGuests).KeyColumn("lmsCourseMeetingId").Cascade.AllDeleteOrphan().Inverse();
        }

        #endregion
    }
}
