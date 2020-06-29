using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace EdugameCloud.Lti.Api.Models
{
    [DataContract]
    public class DeleteMeetingEventDto
    {
        [Required]
        [DataMember]
        public int MeetingId { get; set; }

        [DataMember]
        public int? Id { get; set; }

    }

}
