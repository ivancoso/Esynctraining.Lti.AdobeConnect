using System.Runtime.Serialization;
using EdugameCloud.Lti.Domain.Entities;

namespace EdugameCloud.Lti.Mp4.Host.Dto
{
    [DataContract]
    public sealed class RecordingsRequestDto
    {
        [DataMember(Name = "lmsProviderName")]
        public string LmsProviderName { get; set; }

        [DataMember(Name = "meetingId")]
        public int MeetingId { get; set; }

        [DataMember(Name = "type")]
        public LmsMeetingType LmsMeetingType { get; set; }

    }

}