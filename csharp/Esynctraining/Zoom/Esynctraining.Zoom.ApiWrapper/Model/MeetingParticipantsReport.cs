using System.Collections.Generic;

namespace Esynctraining.Zoom.ApiWrapper.Model
{
    public class MeetingParticipantsReport : PageTokenList
    {
        public List<MeetingParticipant> Participants { get; set; }
    }

    public class ListMeetingParticipantsDetails : PageTokenList
    {
        public List<ZoomMeetingParticipantDetails> Participants { get; set; }
    }
}