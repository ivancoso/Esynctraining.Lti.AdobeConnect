namespace EdugameCloud.Core.Domain.Entities
{
    using Esynctraining.Core.Domain.Entities;

    /// <summary>
    /// The lms ac meeting
    /// </summary>
    public class LmsCourseMeeting : Entity
    {
        /// <summary>
        /// Gets or sets the course id.
        /// </summary>
        public virtual int CourseId { get; set; }

        /// <summary>
        /// Gets or sets the sco id.
        /// </summary>
        public virtual string ScoId { get; set; }

        /// <summary>
        /// Gets or sets the company lms id.
        /// </summary>
        public virtual int CompanyLmsId { get; set; }
    }
}
