using System.Runtime.Serialization;

namespace EdugameCloud.Core.Domain.DTO
{

    [DataContract]
    public class CompanyFlatDTO
    {
        [DataMember]
        public int id { get; set; }

        [DataMember]
        public string name { get; set; }

        [DataMember]
        public bool isActive { get; set; }

        [DataMember]
        public bool isActiveTrial { get; set; }

        [DataMember]
        public bool isExpiredTrial { get; set; }

    }

}
