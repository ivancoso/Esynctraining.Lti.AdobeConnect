using System;

namespace Esynctraining.Zoom.ApiWrapper.Model
{
    public class ZoomRecordingFile
    {
        public string Id { get; set; }
        public string MeetingId { get; set; }
        public DateTimeOffset RecordingStart { get; set; }
        public DateTimeOffset? RecordingEnd { get; set; }//"" for processing
        public string FileType { get; set; } //"" for processing
        public long FileSize { get; set; } // 0 fro processing
        public string PlayUrl { get; set; }
        public string DownloadUrl { get; set; }
        public string Status { get; set; }//processing, completed
        public string RecordingType { get; set; } //active_speaker or gallery_view
        public DateTimeOffset? DeletedTime { get; set; }
    }
}