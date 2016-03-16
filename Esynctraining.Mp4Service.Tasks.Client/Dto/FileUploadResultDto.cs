using System.Runtime.Serialization;

namespace Esynctraining.Mp4Service.Tasks.Client.Dto
{
    [DataContract]
    public class FileDescription
    {
        [DataMember(Name = "id")]
        public string FileId { get; set; }

        [DataMember(Name = "fileName")]
        public string FileName { get; set; }

    }

    [DataContract]
    [KnownType(typeof(FileDescription))]
    public class FileUploadResultDto
    {
        [DataMember(Name = "isSuccess")]
        public bool IsSuccess { get; set; }

        [DataMember(Name = "message")]
        public string Message { get; set; }

        [DataMember(Name = "result")]
        public FileDescription Result { get; set; }

    }

}
