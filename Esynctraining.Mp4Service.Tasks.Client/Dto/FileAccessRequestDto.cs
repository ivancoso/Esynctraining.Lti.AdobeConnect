using System.Runtime.Serialization;

namespace Esynctraining.Mp4Service.Tasks.Client.Dto
{
    [DataContract]
    public sealed class FileAccessRequestDto
    {
        [DataMember(Name = "lmsProviderName")]
        public string LmsProviderName { get; set; }
        
    }

}