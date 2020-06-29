using System.Runtime.Serialization;

namespace EdugameCloud.Core.Domain.DTO
{
    [DataContract]
    public class CompanyQuizEventMappingSaveDTO
    {
        [DataMember]
        public int eventQuizMappingId { get; set; }
        [DataMember]
        public int preQuizId { get; set; }
        [DataMember]
        public int postQuizId { get; set; }
        [DataMember]
        public int companyAcDomainId { get; set; }
        [DataMember]
        public string acEventScoId { get; set; }
    }
}