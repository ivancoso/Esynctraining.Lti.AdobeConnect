using System;

namespace Esynctraining.Lti.Zoom.Common.Dto.Sessions
{
    public class CreateMeetingSessionsBatchDto
    {
        public int Duration { get; set; } //minutes

        // TRICK: to support JIL instead of DayOfWeek
        public int[] DaysOfWeek { get; set; }

        public int Weeks { get; set; }

        public DateTime StartDate { get; set; }
    }
}