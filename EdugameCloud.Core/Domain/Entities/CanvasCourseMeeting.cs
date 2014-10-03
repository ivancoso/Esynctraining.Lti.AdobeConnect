namespace EdugameCloud.Core.Domain.Entities
{
    using Esynctraining.Core.Domain.Entities;

    /// <summary>
    /// The canvas ac meeting
    /// </summary>
    public class CanvasCourseMeeting : Entity
    {
        /// <summary>
        /// The canvas connect credentials
        /// </summary>
        public virtual int CanvasConnectCredentialsId { get; set; }
        /// <summary>
        /// The course id
        /// </summary>
        public virtual int CourseId { get; set; }
        /// <summary>
        /// The sco id
        /// </summary>
        public virtual string ScoId { get; set; }
    }
}
