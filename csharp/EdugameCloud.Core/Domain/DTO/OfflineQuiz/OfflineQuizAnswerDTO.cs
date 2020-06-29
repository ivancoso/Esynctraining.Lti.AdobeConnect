using System;
using System.Runtime.Serialization;

namespace EdugameCloud.Core.Domain.DTO.OfflineQuiz
{
    [DataContract]
    public class OfflineQuizAnswerContainerDTO
    {
        [DataMember]
        public Guid quizResultGuid { get; set; }

        [DataMember]
        public string startTime { get; set; }

        [DataMember]
        public string endTime { get; set; }

        [DataMember]
        public OfflineQuizAnswerDTO[] answers { get; set; }
    }

    [DataContract]
    public class OfflineQuizAnswerDTO
    {

        [DataMember]
        public int questionId { get; set; }

        [DataMember]
        public OfflineTrueFalseAnswerDTO trueFalseAnswer { get; set; }

        [DataMember]
        public OfflineMultiChoiceAnswerDTO multiChoiceAnswer { get; set; }

        [DataMember]
        public OfflineSingleChoiceAnswerDTO singleChoiceAnswer { get; set; }
    }

    [DataContract]
    public class OfflineTrueFalseAnswerDTO
    {
        [DataMember]
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