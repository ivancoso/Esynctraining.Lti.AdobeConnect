using System;
using System.ComponentModel.DataAnnotations;

namespace Esynctraining.Lti.Zoom.Api.Dto.Sessions
{
    public class MeetingSessionUpdateDto
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [MaxLength(2000)]
        public string Summary { get; set; }
    }
}