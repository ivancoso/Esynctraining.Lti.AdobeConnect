namespace Esynctraining.Lti.Zoom.Api.Dto.Email
{
    public class OfficeHoursCancellationEmailModel
    {
        public string Date { get; set; }
        public string MeetingName { get; set; }
        public string Message { get; set; }
    }

    public class OfficeHoursBookSlotEmailModel
    {
        public string UserName { get; set; }
        public string Date { get; set; }
        public string MeetingName { get; set; }
        public string Subject { get; set; }
        public string Questions { get; set; }
    }
}