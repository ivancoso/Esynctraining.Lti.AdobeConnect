using System.Runtime.Serialization;

namespace EdugameCloud.Lti.DTO
{
    [DataContract]
    public class MeetingSessionDTO
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string EventId { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string StartDate { get; set; }

        [DataMember]
        public string EndDate { get; set; }

        [DataMember]
        public string Summary { get; set; }

    }

}