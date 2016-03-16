using System.Runtime.Serialization;
using EdugameCloud.Lti.Core.DTO;
using Esynctraining.Mp4Service.Tasks.Client.Dto;

namespace EdugameCloud.Lti.Mp4.Host.Dto
{
    [DataContract]
    public sealed class RecordingWithMp4Dto : RecordingDTO
    {
        [DataMember(Name = "mp4")]
        public Mp4TaskStatusDto Mp4 { get; set; }
        

    }
}