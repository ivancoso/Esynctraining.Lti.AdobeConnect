using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace EdugameCloud.Lti.DTO
{
    // TODO: move
    [DataContract]
    public class RequestDto
    {
        [Required]
        [DataMember]
        public string lmsProviderName { get; set; }

    }

    [DataContract]
    public class MeetingRequestDto : RequestDto
    {
        [Required]
        [DataMember]
        public int meetingId { get; set; }

    }

    [DataContract]
    public sealed class SearchRequestDto : RequestDto
    {
        [DataMember]
        public string searchTerm { get; set; }

    }

}
