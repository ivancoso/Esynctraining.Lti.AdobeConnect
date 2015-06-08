namespace EdugameCloud.Lti.Domain.Entities
{
    using Esynctraining.Core.Domain.Entities;

    public class LmsCourseMeetingGuest : Entity
    {
        public virtual LmsCourseMeeting LmsCourseMeeting { get; set; }

        public virtual string PrincipalId { get; set; }

    }

}