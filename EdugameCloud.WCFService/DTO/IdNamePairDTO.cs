using System.Runtime.Serialization;

namespace EdugameCloud.WCFService.DTO
{
    [DataContract]
    public class IdNamePairDTO
    {
        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

    }

}