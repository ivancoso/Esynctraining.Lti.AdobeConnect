using System.ComponentModel.DataAnnotations;

namespace Esynctraining.Lti.Zoom.Api.Dto.Sessions
{
    public class MeetingSessionDto : MeetingSessionUpdateDto
    {
        [Required]
        public int Id { get; set; }
    }
}
