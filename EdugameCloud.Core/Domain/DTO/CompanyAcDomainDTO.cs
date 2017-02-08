using System.Runtime.Serialization;

namespace EdugameCloud.Core.Domain.DTO
{
    [DataContract]
    public class CompanyAcDomainDTO
    {
        [DataMember(Name = "path")]
        public string AcServer { get; set; }

        [DataMember(Name = "user")]
        public string Username { get; set; }

        [DataMember(Name = "password")]
        public string Password { get; set; }

        [DataMember(Name = "isDefault")]
        public bool IsDefault { get; set; }

        [DataMember(Name = "companyId")]
        public int CompanyId { get; set; }
    }
}