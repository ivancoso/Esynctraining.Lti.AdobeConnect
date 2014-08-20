namespace EdugameCloud.Persistence.Mappings
{
    using EdugameCloud.Core.Domain.Entities;

    /// <summary>
    /// The canvas ac meeting map
    /// </summary>
    public class CanvasACMeetingMap : BaseClassMap<CanvasACMeeting>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CanvasACMeetingMap"/> class.
        /// </summary>
        public CanvasACMeetingMap()
        {
            this.Map(x => x.ContextId).Not.Nullable();
            this.Map(x => x.ScoId).Not.Nullable();
        }

        #endregion
    }
}
