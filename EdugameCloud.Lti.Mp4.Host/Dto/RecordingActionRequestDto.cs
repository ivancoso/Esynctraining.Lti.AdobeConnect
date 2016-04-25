using System.Runtime.Serialization;

namespace EdugameCloud.Lti.Mp4.Host.Dto
{
    [DataContract]
    public sealed class RecordingActionRequestDto
    {
        [DataMember(Name = "lmsProviderName")]
        public string LmsProviderName { get; set; }

        // TODO: make string
        [DataMember(Name = "recordingId")]
        public long RecordingId { get; set; }

    }

}