using System;

namespace Esynctraining.Zoom.ApiWrapper.Model
{
    public class ZoomMeetingReportItem
    {
        public string Uuid { get; set; }
        public string Id { get; set; }
        public MeetingTypes Type { get; set; }
        public string Topic { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public DateTimeOffset StartTime { get; set; }
        public DateTimeOffset EndTime { get; set; }
        public int Duration { get; set; }
        public int TotalMinutes { get; set; }
        public int ParticipantsCount { get; set; }

    }
}