using System;

namespace Edugamecloud.Lti.Zoom.Dto
{
    public class MeetingDto
    {
        public string Id { get; set; }
        public string HostId { get; set; }
        public string Topic { get; set; }
        public DateTime? StartTime { get; set; }
        public int Duration { get; set; }
        public string Timezone { get; set; }

        public bool CanEdit { get; set; }
        public bool CanJoin { get; set; }
        public bool HasSessions { get; set; }

    }
}
