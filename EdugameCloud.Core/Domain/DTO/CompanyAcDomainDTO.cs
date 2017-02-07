using System.Runtime.Serialization;

namespace EdugameCloud.Core.Domain.DTO
{
    [DataContract]
    public class CompanyAcDomainDTO
    {
        public string AcServer { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool IsDefault { get; set; }
    }
}