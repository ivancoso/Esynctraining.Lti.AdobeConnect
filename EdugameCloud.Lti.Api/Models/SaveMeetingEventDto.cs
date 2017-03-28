using EdugameCloud.Lti.DTO;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;

namespace EdugameCloud.Lti.Api.Models
{
    [DataContract]
    public class SaveMeetingEventDto : MeetingSessionDTO
    {
        [Required]
        [DataMember]
        public int MeetingId { get; set; }

    }

}
