namespace Esynctraining.Zoom.ApiWrapper.Model
{
    public class MeetingRecurrence
    {
        //[JsonProperty("weekly_days")]
        //private string WeeklyDays { get; set; }

        //public MeetingRecurrenceTypes Type { get; set; }

        //public int RepeatInterval { get; set; }

        //public int MonthlyDay { get; set; }

        //public MeetingRecurrenceWeeks MonthlyWeek { get; set; }

        //public MeetingRecurrenceWeekDays MonthlyWeekDay { get; set; }

        //public int EndTimes { get; set; }

        //public DateTimeOffset? EndDateTime { get; set; }

        //public List<MeetingRecurrenceWeekDays> WeeklyDaysList
        //{
        //    get
        //    {
        //        if (string.IsNullOrWhiteSpace(this.WeeklyDays))
        //            return (List<MeetingRecurrenceWeekDays>)null;
        //        IEnumerable<MeetingRecurrenceWeekDays> source = ((IEnumerable<string>)this.WeeklyDays.Split(',')).Select<string, MeetingRecurrenceWeekDays>((Func<string, MeetingRecurrenceWeekDays>)(e => (MeetingRecurrenceWeekDays)Enum.Parse(typeof(MeetingRecurrenceWeekDays), e)));
        //        if (source == null)
        //            return (List<MeetingRecurrenceWeekDays>)null;
        //        return source.ToList<MeetingRecurrenceWeekDays>();
        //    }
        //    set
        //    {
        //        this.WeeklyDays = string.Join<MeetingRecurrenceWeekDays>(",", (IEnumerable<MeetingRecurrenceWeekDays>)value);
        //    }
        //}
    }
}