using System.Runtime.Serialization;

namespace EdugameCloud.Lti.Api.Models
{
    [DataContract]
    public class DeleteMeetingDto
    {
        [DataMember]
        public int meetingId { get; set; }

        [DataMember]
        public bool? remove { get; set; } = false;
    }
}
