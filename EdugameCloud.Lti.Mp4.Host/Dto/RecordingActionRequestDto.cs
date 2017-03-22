using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace EdugameCloud.Lti.Mp4.Host.Dto
{
    [DataContract]
    public sealed class RecordingActionRequestDto
    {
        // TODO: make string
        [Required]
        [DataMember]
        public long RecordingId { get; set; }

    }

}