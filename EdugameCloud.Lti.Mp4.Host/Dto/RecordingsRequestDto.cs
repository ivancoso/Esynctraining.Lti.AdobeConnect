using System.Runtime.Serialization;

namespace EdugameCloud.Lti.Mp4.Host.Dto
{
    [DataContract]
    public sealed class RecordingsRequestDto
    {
        [DataMember]
        public string LmsProviderName { get; set; }

        [DataMember(Name = "type")]
        public int LmsMeetingType { get; set; }

        [DataMember]
        public int MeetingId { get; set; }
        
    }

}