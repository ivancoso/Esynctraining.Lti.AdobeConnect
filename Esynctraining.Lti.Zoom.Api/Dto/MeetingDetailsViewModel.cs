namespace Esynctraining.Lti.Zoom.Api.Dto
{
    public class MeetingDetailsViewModel : CreateMeetingViewModel
    {
        public int Id { get; set; }
        public string ConferenceId { get; set; }
        public string JoinUrl { get; set; }
    }
}