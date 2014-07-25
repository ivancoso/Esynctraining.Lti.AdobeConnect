namespace EdugameCloud.Core.Domain.DTO
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    using NHibernate.Mapping;

    [DataContract]
    public class MoodleQuizResultDTO
    {
        [DataMember]
        public virtual int quizId { get; set; }
        [DataMember]
        public virtual int questionId { get; set; }
        [DataMember]
        public virtual int userId { get; set; }
        [DataMember]
        public virtual int startTime { get; set; }
        [DataMember]
        public virtual List<string> answers { get; set; }
        [DataMember]
        public virtual bool? isSingle { get; set; }
        [DataMember]
        public virtual string questionType { get; set; }
    }
}
