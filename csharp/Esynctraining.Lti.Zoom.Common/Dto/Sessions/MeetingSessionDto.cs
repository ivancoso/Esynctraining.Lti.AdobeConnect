using System.ComponentModel.DataAnnotations;

namespace Esynctraining.Lti.Zoom.Common.Dto.Sessions
{
    public class MeetingSessionDto : MeetingSessionUpdateDto
    {
        [Required]
        public int Id { get; set; }
    }
}
