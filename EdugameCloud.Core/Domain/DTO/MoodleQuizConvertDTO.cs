namespace EdugameCloud.Core.Domain.DTO
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    [DataContract]
    public class MoodleQuizConvertDTO
    {
        [DataMember]
        public virtual int userId { get; set; }
        [DataMember]
        public virtual List<int> quizIds { get; set; }
    }
}
