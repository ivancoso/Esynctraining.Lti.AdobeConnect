namespace Esynctraining.Lti.Zoom.Api.Dto
{
    public class MeetingDetailsDto : CreateZoomMeetingDto
    {
        public string Id { get; set; }

        public string HostId { get; set; }

        public string JoinUrl { get; set; }
    }
}