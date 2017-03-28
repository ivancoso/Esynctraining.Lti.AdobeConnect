using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace EdugameCloud.Lti.Api.Models
{
    [DataContract]
    public class CreateEventDto
    {
        [Required]
        [DataMember]
        public int MeetingId { get; set; }

    }

}
