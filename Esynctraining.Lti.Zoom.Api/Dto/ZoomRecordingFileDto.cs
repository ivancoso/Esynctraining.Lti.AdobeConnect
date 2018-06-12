using System;

namespace Esynctraining.Lti.Zoom.Api.Dto
{
    public class ZoomRecordingFileDto
    {
        public string Id { get; set; }
        //public string Topic { get; set; }
        public string ViewUrl { get; set; }
        public string DownloadUrl { get; set; }
        public string Status { get; set; }

        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string FileType { get; set; }
        public long FileSize { get; set; }

    }
}