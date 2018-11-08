namespace EdugameCloud.Lti.Core.DTO.OfficeHours
{
    public class GetSlotsDto
    {
        public int MeetingId { get; set; }
        public long Start { get; set; }
        public long? End { get; set; }
        public int? Status { get; set; }
    }
}