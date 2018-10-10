using System;

namespace Esynctraining.Lti.Zoom.Api.Dto.Sessions
{
    public class CreateMeetingSessionsBatchDto
    {
        public string Duration { get; set; }

        // TRICK: to support JIL instead of DayOfWeek
        public int[] DaysOfWeek { get; set; }

        public int Weeks { get; set; }

        public DateTime StartDate { get; set; }
    }
}