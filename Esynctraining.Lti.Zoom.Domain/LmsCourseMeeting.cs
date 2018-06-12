namespace Esynctraining.Lti.Zoom.Domain
{
    public class LmsCourseMeeting : BaseEntity
    {

        public int LicenseId { get; set; }
        public int Type { get; set; }
        public string CourseId { get; set; }
        public string ProviderMeetingId { get; set; }
        public string ProviderHostId { get; set; }
        public bool Reused { get; set; }
        public string Details { get; set; }
    }
}