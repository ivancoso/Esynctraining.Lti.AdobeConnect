using System.Runtime.Serialization;
using EdugameCloud.Lti.Core.DTO;
using Esynctraining.Mp4Service.Tasks.Client.Dto;

namespace EdugameCloud.Lti.Mp4.Host.Dto
{
    [DataContract]
    public class RecordingWithMp4Dto : RecordingDTO, IMp4StatusContainer
    {
        [DataMember(Name = "mp4")]
        public Mp4TaskStatusDto Mp4 { get; set; }
        
    }

    [DataContract]
    public sealed class SeminarRecordingWithMp4Dto : RecordingWithMp4Dto
    {
        [DataMember(Name = "seminarSessionId")]
        public string SeminarSessionId { get; set; }

        [DataMember(Name = "seminarSessionName")]
        public string SeminarSessionName { get; set; }
    }

}