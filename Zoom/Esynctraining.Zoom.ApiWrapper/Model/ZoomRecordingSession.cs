using System;
using System.Collections.Generic;

namespace Esynctraining.Zoom.ApiWrapper.Model
{
    public class ZoomRecordingSession
    {
        public string Id { get; set; } //meetingId
        public string Uuid { get; set; }
        public string AccountId { get; set; }
        public string HostId { get; set; }
        public string Topic { get; set; }
        public DateTimeOffset StartTime { get; set; }
        public string Timezone { get; set; }
        public int Duration { get; set; }
        public long TotalSize { get; set; }
        public int RecordingCount { get; set; }
        public List<ZoomRecordingFile> RecordingFiles { get; set; }

        /*
                "": "vCn5dP1oSYSGLaNr5a8eHg",
                "": "hxYospYTROeo73rtvVq-OA",
                "": "Study Meeting A",
                "": "2018-05-18T13:17:06Z",
                "": "Europe/London",
                "duration": 1,
                "": 32807,
                "": 6,
                "recording_files": [
                    {
                        "id": "a9bbcdba-dd8d-4628-900a-ae358a519249",
                        "meeting_id": "3DUEKglsTD+LtHH7nh4niw==",
                        "recording_start": "2018-05-18T13:17:15Z",
                        "recording_end": "2018-05-18T13:17:19Z",
                        "file_type": "MP4",
                        "file_size": 9489,
                        "play_url": "https://api.zoom.us/recording/play/DDhIv2VFUEnbeksOA-g3DGlRW7JjGQ9KtF114kJmTrs_KnqBy1L2Dr3K_DcPxy-e",
                        "download_url": "https://api.zoom.us/recording/download/DDhIv2VFUEnbeksOA-g3DGlRW7JjGQ9KtF114kJmTrs_KnqBy1L2Dr3K_DcPxy-e",
                        "status": "completed",
                        "recording_type": "active_speaker"
         */
    }
}