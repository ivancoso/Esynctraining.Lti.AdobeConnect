using System.Runtime.Serialization;

namespace EdugameCloud.Lti.Core.DTO
{
    [DataContract]
    [KnownType(typeof(PrincipalDto))]
    [KnownType(typeof(TransactionInfoDto))]
    public sealed class PrincipalReportDto
    {
        [DataMember(Name = "principal")]
        public PrincipalDto Principal { get; set; }

        [DataMember(Name = "lastTransaction")]
        public TransactionInfoDto LastTransaction { get; set; }

    }

}
