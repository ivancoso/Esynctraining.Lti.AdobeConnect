using System.Runtime.Serialization;

namespace EdugameCloud.Lti.Mp4.Host.Dto
{
    [DataContract]
    public sealed class RecordingActionRequestDto
    {
        [DataMember]
        public string LmsProviderName { get; set; }

        // TODO: make string
        [DataMember]
        public long RecordingId { get; set; }

    }

}