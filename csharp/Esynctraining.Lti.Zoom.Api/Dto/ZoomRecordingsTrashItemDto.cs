using System;

namespace Esynctraining.Lti.Zoom.Api.Dto
{
    public class ZoomRecordingsTrashItemDto
    {
        public string Topic { get; set; }
        public string RecordingFileId { get; set; }
        public string MeetingId { get; set; }
        public string RecordingId { get; set; } //guid, like "meeting session" id (i.e. if you launch the same meeting multiple times)
        public DateTime StartTime { get; set; }
        public string FileType { get; set; }  //null if >1 files
        public long FileSize { get; set; }
        public DateTime? DeletedTime { get; set; }
        public int RecordingCount { get; set; }
    }
}