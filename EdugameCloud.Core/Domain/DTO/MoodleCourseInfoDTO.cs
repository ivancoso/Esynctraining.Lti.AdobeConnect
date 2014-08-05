namespace EdugameCloud.Core.Domain.DTO
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    [DataContract]
    public class MoodleCourseInfoDTO
    {
        [DataMember]
        public string id { get; set; }
        [DataMember]
        public string fullname { get; set; }
        [DataMember]
        public List<MoodleQuizInfoDTO> quizzes { get; set; }
    }
}
