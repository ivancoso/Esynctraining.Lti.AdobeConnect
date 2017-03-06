using System.Runtime.Serialization;

namespace EdugameCloud.Core.Domain.DTO.OfflineQuiz
{
    [DataContract]
    public class OfflineQuizAnswerDTO
    {
        [DataMember]
        public int eventQuizMappingId { get; set; }

        [DataMember]
        public int questionId { get; set; }

        [DataMember]
        public OfflineTrueFalseAnswerDTO trueFalseAnswer { get; set; }

        [DataMember]
        public OfflineMultiChoiceAnswerDTO multiChoiceAnswer { get; set; }

        [DataMember]
        public OfflineMultiChoiceAnswerDTO singleChoiceAnswer { get; set; }
    }

    [DataContract]
    public class OfflineTrueFalseAnswerDTO
    {
        public bool answer { get; set; }
    }

    [DataContract]
    public class OfflineMultiChoiceAnswerDTO
    {
        [DataMember]
        public int[] answeredDistractorIds { get; set; }
    }

    [DataContract]
    public class OfflineSingleChoiceAnswerDTO
    {
        [DataMember]
        public int answeredDistractorId { get; set; }
    }
}