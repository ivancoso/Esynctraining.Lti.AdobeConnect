using System;
using System.ComponentModel.DataAnnotations;

namespace Esynctraining.Lti.Zoom.Api.Dto.Sessions
{
    public class MeetingSessionDto
    {
        [Required]
        public int Id { get; set; }

        //public string EventId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        public string Summary { get; set; }

    }

    public class CreateMeetingSessionsBatchDto
    {
        public int MeetingId { get; set; }

        public string Duration { get; set; }

        // TRICK: to support JIL instead of DayOfWeek
        public int[] DaysOfWeek { get; set; }

        public int Weeks { get; set; }

        public DateTime StartDate { get; set; }
    }
}
