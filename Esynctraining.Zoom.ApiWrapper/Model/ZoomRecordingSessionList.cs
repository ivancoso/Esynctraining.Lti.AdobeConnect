using System.Collections.Generic;

namespace Esynctraining.Zoom.ApiWrapper.Model
{
    public class ZoomRecordingSessionList : PageTokenList
    {
        public List<ZoomRecordingSession> Meetings { get; set; }
    }
}