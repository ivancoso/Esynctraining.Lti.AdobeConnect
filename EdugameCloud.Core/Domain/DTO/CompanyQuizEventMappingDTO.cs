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
        }

        public int EventQuizMappingId { get; set; }
        public int PreQuizId { get; set; }
        public int PostQuizId { get; set; }
        public int CompanyAcDomainId { get; set; }
        public string AcEventScoId { get; set; }

    }
}