using System.Runtime.Serialization;

namespace EdugameCloud.Core.Domain.DTO.OfflineQuiz
{
    [DataContract]
    public class OfflineQuizResultDTO
    {
        [DataMember]
        public int score { get; set; }

        [DataMember]
        public string certificateUrl { get; set; }
    }
}