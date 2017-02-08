using System.Runtime.Serialization;

namespace EdugameCloud.Core.Domain.DTO
{
    [DataContract]
    public class CompanyAcDomainDTO
    {
        [DataMember]
        public string path { get; set; }

        [DataMember]
        public string user { get; set; }

        [DataMember]
        public string password { get; set; }

        [DataMember]
        public bool IsDefault { get; set; }

        [DataMember]
        public int companyId { get; set; }
    }
}