using System.Runtime.Serialization;

namespace EdugameCloud.Lti.Mp4.Host.Dto
{
    [DataContract]
    public sealed class RecordingsRequestDto
    {
        [DataMember(Name = "lmsProviderName")]
        public string LmsProviderName { get; set; }

        [DataMember(Name = "meetingId")]
        public int MeetingId { get; set; }

    }

}