namespace EdugameCloud.Persistence.Mappings
{
    using EdugameCloud.Core.Domain.Entities;
    using EdugameCloud.Lti.Domain.Entities;

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
            this.Map(x => x.CourseId).Not.Nullable();
            this.Map(x => x.ScoId).Nullable();

            this.References(x => x.CompanyLms).Nullable();
        }

        #endregion
    }
}
