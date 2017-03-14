using System.Runtime.Serialization;

namespace EdugameCloud.Core.Domain.DTO.OfflineQuiz
{
    [DataContract]
    public class OfflineQuizDTO
    {
        [DataMember]
        public string errorMessage { get; set; }

        [DataMember]
        public string quizName { get; set; }

        [DataMember]
        public string description { get; set; }

        [DataMember]
        public ParticipantDTO participant { get; set; }

        [DataMember]
        public OfflineQuestionDTO[] questions { get; set; }
    }
}

