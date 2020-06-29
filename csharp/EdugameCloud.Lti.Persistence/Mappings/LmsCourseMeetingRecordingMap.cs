using Esynctraining.Persistence.Mappings;
using EdugameCloud.Lti.Domain.Entities;

namespace EdugameCloud.Lti.Persistence.Mappings
{
    public sealed class LmsCourseMeetingRecordingMap : BaseClassMap<LmsCourseMeetingRecording>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LmsCourseMeetingRecordingMap"/> class.
        /// </summary>
        public LmsCourseMeetingRecordingMap()
        {
            this.References(x => x.LmsCourseMeeting).Column("lmsCourseMeetingId");
            this.Map(x => x.ScoId).Not.Nullable();
        }

        #endregion
    }

}
