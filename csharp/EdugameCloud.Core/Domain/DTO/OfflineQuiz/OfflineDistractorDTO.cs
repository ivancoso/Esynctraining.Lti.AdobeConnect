using System.Runtime.Serialization;

namespace EdugameCloud.Core.Domain.DTO.OfflineQuiz
{
    [DataContract]
    public class OfflineDistractorDTO
    {
        [DataMember]
        public string distractor { get; set; }

        [DataMember]
        public int distractorId { get; set; }

        [DataMember]
        public int distractorOrder { get; set; }

        [DataMember]
        public int? distractorType { get; set; }

        [DataMember]
        public int? questionId { get; set; }
        
    }
}