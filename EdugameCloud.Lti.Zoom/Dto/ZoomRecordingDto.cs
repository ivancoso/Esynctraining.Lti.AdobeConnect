using System;
using System.Collections.Generic;

namespace Edugamecloud.Lti.Zoom.Dto
{
    public class ZoomRecordingDto
    {
        public DateTime StartTime { get; set; }
        public List<ZoomRecordingFileDto> Files { get; set; }
    }
}