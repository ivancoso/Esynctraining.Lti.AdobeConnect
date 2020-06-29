using System;
using System.Collections.Generic;

namespace Esynctraining.Zoom.ApiWrapper.Model
{
    public class ZoomApiRecordingList
    {
        public string Id { get; set; }
        public string Uuid { get; set; }
        public string AccountId { get; set; }
        public string HostId { get; set; }
        public string Topic { get; set; }

        public DateTimeOffset StartTime { get; set; }
        public int Duration { get; set; }
        public long TotalSize { get; set; }
        public int RecordingCount { get; set; }
        public List<ZoomRecordingFile> RecordingFiles { get; set; }

    }
}