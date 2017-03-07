using System.Runtime.Serialization;

namespace EdugameCloud.Core.Domain.DTO.OfflineQuiz
{
    [DataContract]
    public class ParticipantDTO
    {
        [DataMember]
        public string email { get; set; }

        [DataMember]
        public string participantName { get; set; }
    }
}