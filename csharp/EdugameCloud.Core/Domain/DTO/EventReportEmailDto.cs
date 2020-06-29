using System.Runtime.Serialization;

namespace EdugameCloud.Core.Domain.DTO
{
    [DataContract]
    public class EventReportEmailDto
    {
        [DataMember(Name = "quizResultIds")]
        public int[] QuizResultIds { get; set; }

//        [DataMember(Name = "eventQuizMappingId")]
//        public int EventQuizMappingId { get; set; }
    }
}