using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace EdugameCloud.Lti.Mp4.Host.Dto
{
    [DataContract]
    public sealed class RecordingActionRequestDto
    {
        [Required]
        [DataMember]
        public string RecordingId { get; set; }

    }

}