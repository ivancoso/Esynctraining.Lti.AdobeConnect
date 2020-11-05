using System.Collections.Generic;

namespace Esynctraining.Zoom.ApiWrapper.Model
{
    public class ListMeetings : PageList
    {
        public List<Meeting> Meetings { get; set; }
    }
}