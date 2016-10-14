using System.Runtime.Serialization;

namespace EdugameCloud.Lti.DTO
{
    /// <summary>
    /// The calendar event DTO.
    /// </summary>
    [DataContract]
    public class MeetingSessionDTO
    {
        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember(Name = "eventId")]
        public string EventId { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "startDate")]
        public string StartDate { get; set; }

        [DataMember(Name = "endDate")]
        public string EndDate { get; set; }

        [DataMember(Name = "summary")]
        public string Summary { get; set; }
    }
}