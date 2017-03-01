using System.Runtime.Serialization;

namespace EdugameCloud.Core.Domain.DTO.OfflineQuiz
{
    [DataContract]
    public class ParticipantDTO
    {
        [DataMember]
        public string email { get; set; }

        [DataMember]
        public string firstName { get; set; }

        [DataMember]
        public string lastName { get; set; }

        [DataMember]
        public string userName { get; set; }
    }

    [DataContract]
    public class OfflineQuizResultDTO
    {
        [DataMember]
        public int score { get; set; }
    }

    [DataContract]
    public class OfflineQuizAnswerDTO
    {
        [DataMember]
        public int questionId { get; set; }

        [DataMember]
        public int[] answeredDistractorIds { get; set;  }
    }
}