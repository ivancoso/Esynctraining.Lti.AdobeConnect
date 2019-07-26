using System;
using System.Collections.Generic;

namespace EdugameCloud.Lti.SearchApi.Host.Models
{
    public partial class LmsMeetingType
    {
        public LmsMeetingType()
        {
            LmsCourseMeeting = new HashSet<LmsCourseMeeting>();
        }

        public int LmsMeetingTypeId { get; set; }
        public string LmsMeetingTypeName { get; set; }

        public virtual ICollection<LmsCourseMeeting> LmsCourseMeeting { get; set; }
    }
}
