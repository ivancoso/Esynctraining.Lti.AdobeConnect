using System.Runtime.Serialization;

namespace Esynctraining.Mp4Service.Tasks.Client.Dto
{
    [DataContract]
    public sealed class FileDto
    {
        [DataMember(Name = "id")]
        public string Id { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "size")]
        public int Size { get; set; }

    }

}
