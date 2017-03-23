using System.Runtime.Serialization;

namespace EdugameCloud.Lti.Api.Models
{
    [DataContract]
    public class DeleteMeetingEventDto
    {
        [DataMember]
        public int meetingId { get; set; }

        [DataMember]
        public int? id { get; set; }
    }
}
