using Esynctraining.Core.Domain.Entities;
using System.Collections.Generic;

namespace EdugameCloud.Lti.Domain.Entities
{
    public class OfficeHours : Entity
    {
        public virtual string ScoId { get; set; }

        public virtual string Hours { get; set; }

        public virtual LmsUser LmsUser { get; set; }

        public virtual IList<OfficeHoursTeacherAvailability> Availabilities { get; protected set; }
    }
}
