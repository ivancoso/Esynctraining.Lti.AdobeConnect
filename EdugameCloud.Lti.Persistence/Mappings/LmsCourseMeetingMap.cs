namespace EdugameCloud.Lti.Persistence.Mappings
{
    using EdugameCloud.Lti.Domain.Entities;

    using Esynctraining.Persistence.Mappings;

    /// <summary>
    /// The canvas AC meeting map
    /// </summary>
    public class LmsCourseMeetingMap : BaseClassMap<LmsCourseMeeting>
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
            this.Map(x => x.CachedUsers).CustomType("StringClob").CustomSqlType("nvarchar(max)").Nullable();
            this.Map(x => x.AddedToCache).Nullable();
            this.Map(x => x.LmsMeetingType).Column("lmsMeetingTypeId").Nullable();
            
            this.References(x => x.LmsCompany).Column("companyLmsId").Nullable();
            this.References(x => x.OfficeHours).Nullable();
            this.References(x => x.Owner).Column("ownerId").Nullable();
        }

        #endregion
    }
}
