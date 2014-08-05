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
        [DataMember]
        public string course { get; set; }
        [DataMember]
        public string courseName { get; set; }
        [DataMember]
        public int lastModifiedMoodle { get; set; }
        [DataMember]
        public int lastModifiedEGC { get; set; }
    }
}
