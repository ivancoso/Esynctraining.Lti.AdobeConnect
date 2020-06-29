using System.Runtime.Serialization;

namespace EdugameCloud.Core.Domain.DTO
{

    [DataContract]
    public class QuizSummaryResultDTO
    {
        [DataMember(IsRequired = true)]
        public int acSessionId { get; set; }

        [DataMember(IsRequired = true)]
        public int quizId { get; set; }

        [DataMember]
        public int? eventQuizMappingId { get; set; }

        [DataMember]
        public int companyId { get; set; }

        [DataMember]
        public QuizResultDTO[] quizResults { get; set; }
    }
}
