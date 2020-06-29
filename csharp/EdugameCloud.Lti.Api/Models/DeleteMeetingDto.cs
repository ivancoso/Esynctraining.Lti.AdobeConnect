using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace EdugameCloud.Lti.Api.Models
{
    [DataContract]
    public class DeleteMeetingDto
    {
        [Required]
        [DataMember]
        public int MeetingId { get; set; }

        [DataMember]
        public bool? Remove { get; set; } = false;

    }

}
