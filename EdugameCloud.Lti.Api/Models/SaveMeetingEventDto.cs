using EdugameCloud.Lti.DTO;
using System.Runtime.Serialization;

namespace EdugameCloud.Lti.Api.Models
{
    [DataContract]
    public class SaveMeetingEventDto : MeetingSessionDTO
    {
        [DataMember]
        public int meetingId { get; set; }
    }
}
