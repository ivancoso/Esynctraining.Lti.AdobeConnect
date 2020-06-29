using System;
using System.Collections.Generic;

namespace EdugameCloud.Lti.SearchApi.Host.Models
{
    public partial class LmsCourseSection
    {
        public int LmsCourseSectionId { get; set; }
        public int LmsCourseMeetingId { get; set; }
        public string LmsId { get; set; }
        public string Name { get; set; }

        public virtual LmsCourseMeeting LmsCourseMeeting { get; set; }
    }
}
