namespace EdugameCloud.Lti.Domain.Entities
{
    using Esynctraining.Core.Domain.Entities;

    public class LmsCourseMeetingRecording : Entity
    {
        public virtual LmsCourseMeeting LmsCourseMeeting { get; set; }

        /// <summary>
        /// Gets or sets sco-id of the recording.
        /// </summary>
        public virtual string ScoId { get; set; }

    }

}