using System.Runtime.Serialization;

namespace EdugameCloud.Core.Domain.DTO
{
    [DataContract]
    public class CompanyAdvancedFilterDTO
    {
        public CompanyAdvancedFilterDTO()
        {
            
        }

        [DataMember(IsRequired = false)]
        public string firstName { get; set; }

        [DataMember(IsRequired = false)]
        public string lastName { get; set; }

        [DataMember(IsRequired = false)]
        public string email { get; set; }

        [DataMember(IsRequired = false)]
        public string companyName { get; set; }

        [DataMember(IsRequired = false)]
        public string acServer { get; set; }
    }
}
