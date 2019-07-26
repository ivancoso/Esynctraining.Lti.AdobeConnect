using System;
using System.Collections.Generic;

namespace EdugameCloud.Lti.SearchApi.Host.Models
{
    public partial class LmsCourseMeetingRecording
    {
        public int LmsCourseMeetingRecordingId { get; set; }
        public int LmsCourseMeetingId { get; set; }
        public string ScoId { get; set; }

        public virtual LmsCourseMeeting LmsCourseMeeting { get; set; }
    }
}
