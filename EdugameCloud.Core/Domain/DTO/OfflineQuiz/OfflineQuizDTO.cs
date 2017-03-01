using System.Runtime.Serialization;

namespace EdugameCloud.Core.Domain.DTO.OfflineQuiz
{
    [DataContract]
    public class OfflineQuizDTO
    {
        [DataMember]
        public string quizName { get; set; }

        [DataMember]
        public string description { get; set; }

        public ParticipantDTO participant { get; set; }

        public OfflineQuestionDTO[] questions { get; set; }
    }
}

