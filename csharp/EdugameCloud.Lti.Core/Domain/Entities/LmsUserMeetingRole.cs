using EdugameCloud.Lti.Domain.Entities;
using Esynctraining.Core.Domain.Entities;

namespace EdugameCloud.Lti.Core.Domain.Entities
{
    public class LmsUserMeetingRole : Entity
    {
        public virtual LmsUser User { get; set; }
        public virtual LmsCourseMeeting Meeting { get; set; }
        public virtual string LmsRole { get; set; }
    }
}