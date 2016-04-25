using Esynctraining.Persistence.Mappings;
using EdugameCloud.Lti.Domain.Entities;

namespace EdugameCloud.Lti.Persistence.Mappings
{
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
            this.Map(x => x.ScoId).Nullable();
            this.Map(x => x.MeetingNameJson).Length(4000).Nullable();
            this.Map(x => x.LmsMeetingType).Column("lmsMeetingTypeId").Not.Nullable();
            this.Map(x => x.Reused).Nullable();
            this.Map(x => x.SourceCourseMeetingId).Nullable();
            this.Map(x => x.AudioProfileId).Nullable();

            this.Map(x => x.LmsCompanyId).Column("companyLmsId").Not.Nullable();
            this.Map(x => x.EnableDynamicProvisioning).Not.Nullable();
            
            this.References(x => x.OfficeHours).Nullable();
            this.References(x => x.Owner).Column("ownerId").Nullable();
            HasMany(x => x.MeetingRoles).KeyColumn("lmsCourseMeetingId").Cascade.AllDeleteOrphan().Inverse();
            HasMany(x => x.MeetingGuests).KeyColumn("lmsCourseMeetingId").Cascade.AllDeleteOrphan().Inverse();
            HasMany(x => x.MeetingRecordings).KeyColumn("lmsCourseMeetingId").Cascade.AllDeleteOrphan().Inverse();
        }

        #endregion
    }
}
