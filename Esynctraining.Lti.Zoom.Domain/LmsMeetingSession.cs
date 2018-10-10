using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Esynctraining.Lti.Zoom.Domain
{
    [Table("LmsMeetingSession")]
    public class LmsMeetingSession : BaseEntity
    {
        //public virtual string ExternalId { get; set; }

        public string Name { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public virtual LmsCourseMeeting Meeting { get; set; }

        public string Summary { get; set; }

    }
}