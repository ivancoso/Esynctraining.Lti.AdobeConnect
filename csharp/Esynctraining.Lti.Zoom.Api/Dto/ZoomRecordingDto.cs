using System;
using System.Collections.Generic;

namespace Esynctraining.Lti.Zoom.Api.Dto
{
    public class ZoomRecordingDto
    {
        public DateTime StartTime { get; set; }
        public List<ZoomRecordingFileDto> Files { get; set; }
    }
}