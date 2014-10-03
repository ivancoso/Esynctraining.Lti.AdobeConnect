namespace EdugameCloud.Persistence.Mappings
{
    using EdugameCloud.Core.Domain.Entities;

    /// <summary>
    /// The canvas ac meeting map
    /// </summary>
    public class CanvasCourseMeetingMap : BaseClassMap<CanvasCourseMeeting>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CanvasCourseMeetingMap"/> class.
        /// </summary>
        public CanvasCourseMeetingMap()
        {
            this.Map(x => x.CanvasConnectCredentialsId).Not.Nullable();
            this.Map(x => x.CourseId).Not.Nullable();
            this.Map(x => x.ScoId).Not.Nullable();
        }

        #endregion
    }
}
