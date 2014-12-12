namespace EdugameCloud.Lti.Domain.Entities
{
    using Esynctraining.Core.Domain.Entities;

    /// <summary>
    /// The LMS AC meeting
    /// </summary>
    public class LmsCourseMeeting : Entity
    {
        /// <summary>
        /// Gets or sets the course id.
        /// </summary>
        public virtual int CourseId { get; set; }

        /// <summary>
        /// Gets or sets the SCO id.
        /// </summary>
        public virtual string ScoId { get; set; }

        /// <summary>
        /// Gets or sets the company LMS.
        /// </summary>
        public virtual CompanyLms CompanyLms { get; set; }
    }
}
