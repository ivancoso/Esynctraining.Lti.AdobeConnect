using System.Runtime.Serialization;
using EdugameCloud.Core.Domain.Entities;

namespace EdugameCloud.Core.Domain.DTO
{
    [DataContract]
    public class CompanyQuizEventMappingDTO
    {
        public CompanyQuizEventMappingDTO(CompanyEventQuizMapping entity)
        {
            AcEventScoId = entity.AcEventScoId;
            PreQuizId = entity.PreQuiz.Id;
            PostQuizId = entity.PostQuiz.Id;
            EventQuizMappingId = entity.Id;
            CompanyAcDomainId = entity.CompanyAcDomain.Id;
            CompanyAcDomain = new ACDomainDTO()
            {
                companyId = entity.CompanyAcDomain.Company.Id,
                password = entity.CompanyAcDomain.Password,
                domainId = entity.CompanyAcDomain.Id,
                isDefault = entity.CompanyAcDomain.IsDefault,
                path = entity.CompanyAcDomain.AcServer,
                user = entity.CompanyAcDomain.Username
            };
        }

        [DataMember]
        public int EventQuizMappingId { get; set; }
        [DataMember]
        public int PreQuizId { get; set; }
        [DataMember]
        public int PostQuizId { get; set; }
        [DataMember]
        public int CompanyAcDomainId { get; set; }
        [DataMember]
        public string AcEventScoId { get; set; }

        [DataMember]
        public ACDomainDTO CompanyAcDomain { get; set; } 

    }
}