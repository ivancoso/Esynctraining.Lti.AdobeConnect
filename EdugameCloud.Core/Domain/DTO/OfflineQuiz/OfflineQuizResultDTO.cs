using System.Runtime.Serialization;

namespace EdugameCloud.Core.Domain.DTO.OfflineQuiz
{
    [DataContract]
    public class OfflineQuizResultDTO
    {
        [DataMember]
        public int score { get; set; }

        [DataMember]
        public bool isSuccess { get; set; }

        [DataMember]
        public string certificateDownloadUrl { get; set; }

        [DataMember]
        public string certificatePreviewUrl { get; set; }

        [DataMember]
        public string errorMessage { get; set; }
    }
}