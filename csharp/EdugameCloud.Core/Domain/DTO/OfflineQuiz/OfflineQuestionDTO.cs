using System;
using System.Runtime.Serialization;

namespace EdugameCloud.Core.Domain.DTO.OfflineQuiz
{
    [DataContract]
    public class OfflineQuestionDTO
    {
        [DataMember]
        public int questionId { get; set; }

        [DataMember]
        public string question { get; set; }

        [DataMember]
        public int questionOrder { get; set; }

        [DataMember]
        public int questionTypeId { get; set; }

        [DataMember]
        public string instruction { get; set; }

        [DataMember]
        public Guid? imageId { get; set; }

        [DataMember]
        public FileDTO imageVO { get; set; }

        [DataMember]
        public bool? randomizeAnswers { get; set; }

        [DataMember]
        public string restrictions { get; set; }

        [DataMember]
        public bool isMultipleChoice { get; set; }

        [DataMember]
        public OfflineDistractorDTO[] distractors { get; set; }

    }
}