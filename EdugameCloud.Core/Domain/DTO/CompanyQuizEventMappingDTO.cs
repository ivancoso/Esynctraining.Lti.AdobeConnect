using System.Runtime.Serialization;
using EdugameCloud.Core.Domain.Entities;

namespace EdugameCloud.Core.Domain.DTO
{
    [DataContract]
    public class CompanyQuizEventMappingDTO : CompanyQuizEventMappingSaveDTO
    {
        public CompanyQuizEventMappingDTO(CompanyEventQuizMapping entity, CompanyEventDTO eventDto = null)
        {
            acEventScoId = entity.AcEventScoId;
            preQuizId = entity.PreQuiz?.Id ?? 0;
            postQuizId = entity.PostQuiz?.Id ?? 0;
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
            if (eventDto != null)
            {
                acEventInfo = new CompanyEventDTO()
                {
                    companyId = eventDto.companyId,
                    dateEnd = eventDto.dateEnd,
                    desc = eventDto.desc,
                    scoId = eventDto.scoId,
                    name = eventDto.name,
                    dateCreated = eventDto.dateCreated,
                    dateBegin = eventDto.dateBegin,
                    dateModified = eventDto.dateModified,
                    urlPath = eventDto.urlPath,
                    isMappedToQuizzes = eventDto.isMappedToQuizzes,
                    isSeminar = eventDto.isSeminar,
                    meetingUrl = eventDto.meetingUrl
                };
            }

            if (eventQuizMappingId != 0)
            {
                registrationUrl = "http://dev.esynctraining.com:8066?eventQuizMappingId=" + eventQuizMappingId;
            }
        }

        [DataMember]
        public string registrationUrl { get; set; }

        [DataMember]
        public CompanyEventDTO acEventInfo { get; set; }
        //[DataMember]
        //public ACDomainDTO companyAcDomain { get; set; } 

    }
}