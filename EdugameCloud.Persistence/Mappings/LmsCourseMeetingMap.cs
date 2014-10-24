namespace EdugameCloud.Persistence.Mappings
{
    using EdugameCloud.Core.Domain.Entities;

    /// <summary>
    /// The canvas ac meeting map
    /// </summary>
    public class LmsCourseMeetingMap : BaseClassMap<LmsCourseMeeting>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LmsCourseMeetingMap"/> class.
        /// </summary>
        public LmsCourseMeetingMap()
        {
            this.Map(x => x.CourseId).Not.Nullable();
            this.Map(x => x.ScoId).Not.Nullable();
            this.Map(x => x.CompanyLmsId).Nullable();
        }

        #endregion
    }
}
