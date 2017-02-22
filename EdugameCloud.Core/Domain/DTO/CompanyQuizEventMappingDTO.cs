using System.Runtime.Serialization;
using EdugameCloud.Core.Domain.Entities;

namespace EdugameCloud.Core.Domain.DTO
{
    [DataContract]
    public class CompanyQuizEventMappingDTO : CompanyQuizEventMappingSaveDTO
    {
        public CompanyQuizEventMappingDTO(CompanyEventQuizMapping entity)
        {
            acEventScoId = entity.AcEventScoId;
            preQuizId = entity.PreQuiz.Id;
            postQuizId = entity.PostQuiz.Id;
            eventQuizMappingId = entity.Id;
            companyAcDomainId = entity.CompanyAcDomain.Id;
            //companyAcDomain = new ACDomainDTO()
            //{
            //    companyId = entity.CompanyAcDomain.Company.Id,
            //    password = entity.CompanyAcDomain.Password,
            //    domainId = entity.CompanyAcDomain.Id,
            //    isDefault = entity.CompanyAcDomain.IsDefault,
            //    path = entity.CompanyAcDomain.AcServer,
            //    user = entity.CompanyAcDomain.Username
            //};
        }

       


        //[DataMember]
        //public ACDomainDTO companyAcDomain { get; set; } 

    }
}