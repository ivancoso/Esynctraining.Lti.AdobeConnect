using System.Runtime.Serialization;

namespace EdugameCloud.Core.Domain.DTO
{
    [DataContract]
    public class ACDomainDTO
    {
        [DataMember]
        public int domainId { get; set; }

        [DataMember]
        public string path { get; set; }

        [DataMember]
        public string user { get; set; }

        [DataMember]
        public string password { get; set; }

        [DataMember]
        public bool isDefault { get; set; }

        [DataMember]
        public int companyId { get; set; }
    }
}