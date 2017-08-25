using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace EdugameCloud.Lti.Api.Models
{
    [DataContract]
    public class ShareRecordingRequestDto : RequestDto
    {
        [Required]
        [DataMember]
        public string RecordingId { get; set; }

        [Required]
        [DataMember]
        public bool IsPublic { get; set; }

        // TODO: is it used??
        [DataMember]
        public string Password { get; set; }

    }
}
