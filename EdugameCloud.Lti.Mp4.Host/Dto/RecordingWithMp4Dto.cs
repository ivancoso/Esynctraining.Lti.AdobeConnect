using System.Runtime.Serialization;
using Esynctraining.AdobeConnect.Api.MeetingRecording.Dto;
using Esynctraining.Mp4Service.Tasks.Client.Dto;

namespace EdugameCloud.Lti.Mp4.Host.Dto
{
    [DataContract]
    public class RecordingWithMp4Dto : RecordingDto, IMp4StatusContainer
    {
        [DataMember]
        public Mp4TaskStatusDto Mp4 { get; set; }
        
    }

    [DataContract]
    public sealed class SeminarRecordingWithMp4Dto : RecordingWithMp4Dto
    {
        [DataMember]
        public string SeminarSessionId { get; set; }

        [DataMember]
        public string SeminarSessionName { get; set; }

    }

}