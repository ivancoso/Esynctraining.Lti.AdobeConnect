using System;
using System.Runtime.Serialization;
using EdugameCloud.Core.Domain.Entities;
using Esynctraining.Core.Providers;

namespace EdugameCloud.Core.Domain.DTO
{
    [DataContract]
    public class CompanyQuizEventMappingDTO : CompanyQuizEventMappingSaveDTO
    {
        public CompanyQuizEventMappingDTO(CompanyEventQuizMapping entity, dynamic settings, CompanyEventDTO eventDto = null, CompanyAcServer acServer = null)
        {
            if (entity == null)
                throw new InvalidOperationException($"Entity should not be null");

            if (settings == null)
                throw new InvalidOperationException($"Settings should not be null");
            acEventScoId = entity.AcEventScoId;
            guid = entity.Guid;
            preQuizId = entity.PreQuiz?.Id ?? 0;
            postQuizId = entity.PostQuiz?.Id ?? 0;
            eventQuizMappingId = entity.Id;
            if (entity.CompanyAcDomain == null)
                throw new InvalidOperationException($"entity.CompanyAcDomain should not be null");
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
            if (acServer != null)
            {
                companyAcDomain = new ACDomainDTO()
                {
                    path = acServer.AcServer,
                    isDefault = acServer.IsDefault,
                    domainId = acServer.Id,
                    user = acServer.Username
                };
            }
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
                registrationUrl = settings.CertificatesUrl + "/" + entity.Guid;
            }
        }

        [DataMember]
        public string registrationUrl { get; set; }

        [DataMember]
        public Guid guid { get; set; }

        [DataMember]
        public CompanyEventDTO acEventInfo { get; set; }
        [DataMember]
        public ACDomainDTO companyAcDomain { get; set; }

    }
}