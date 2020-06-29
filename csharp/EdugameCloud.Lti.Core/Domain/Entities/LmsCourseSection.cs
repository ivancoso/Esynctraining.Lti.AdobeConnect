using Esynctraining.Core.Domain.Entities;

namespace EdugameCloud.Lti.Domain.Entities
{
    public class LmsCourseSection : Entity
    {
        public virtual string LmsId { get; set; }
        public virtual string Name { get; set; }
        public virtual LmsCourseMeeting Meeting { get; set; }
    }
}