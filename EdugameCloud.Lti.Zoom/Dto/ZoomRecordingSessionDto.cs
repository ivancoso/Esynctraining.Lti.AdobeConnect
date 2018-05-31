using System;
using System.Collections.Generic;

namespace Edugamecloud.Lti.Zoom.Dto
{
    public class ZoomRecordingSessionDto
    {
        public string Id { get; set; }
        public string Topic { get; set; }
        public DateTime StartTime { get; set; }
        public int Duration { get; set; }
        public long TotalSize { get; set; }
        public int RecordingCount { get; set; }
        public List<ZoomRecordingDto> Recordings { get; set; }
    }
}