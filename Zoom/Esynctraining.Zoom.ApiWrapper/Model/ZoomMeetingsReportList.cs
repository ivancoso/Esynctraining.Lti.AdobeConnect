using System.Collections.Generic;

namespace Esynctraining.Zoom.ApiWrapper.Model
{
    public class ZoomMeetingsReportList : PageTokenList
    {
        public List<ZoomMeetingReportItem> Meetings { get; set; }
    }
}