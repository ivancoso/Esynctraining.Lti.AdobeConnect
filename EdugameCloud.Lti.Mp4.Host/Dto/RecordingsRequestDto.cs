using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace EdugameCloud.Lti.Mp4.Host.Dto
{
    [DataContract]
    public sealed class RecordingsRequestDto
    {
        [Required]
        [DataMember(Name = "type")]
        public int LmsMeetingType { get; set; }

        [Required]
        [DataMember]
        public int MeetingId { get; set; }

    }

}