namespace EdugameClaud.Lti.SearchApi.Host.DTOs
{
    public class LmsCourseMeetingDto
    {
        public int LmsCourseMeetingId { get; set; }
        public string CourseId { get; set; }
        public string ScoId { get; set; }
        public int CompanyLmsId { get; set; }
        public int LmsMeetingTypeId { get; set; }
        public int? OfficeHoursId { get; set; }
        public MeetingNameInfoDto MeetingNameInfo { get; set; }
    }
}
