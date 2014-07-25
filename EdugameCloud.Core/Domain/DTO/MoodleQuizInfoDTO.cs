namespace EdugameCloud.Core.Domain.DTO
{
    using System.Runtime.Serialization;

    [DataContract]
    public class MoodleQuizInfoDTO
    {
        [DataMember]
        public string id { get; set; }
        [DataMember]
        public string name { get; set; }
    }
}
