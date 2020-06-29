namespace Esynctraining.Lti.Zoom.Common.Dto
{
    public class CreateMeetingRecurrenceViewModel
    {
        // TRICK: to support JIL instead of DayOfWeek
        public int[] DaysOfWeek { get; set; }

        public int Weeks { get; set; }

    }
}