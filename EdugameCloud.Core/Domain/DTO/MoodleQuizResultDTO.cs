namespace EdugameCloud.Core.Domain.DTO
{
    using System.Runtime.Serialization;

    [DataContract]
    public class MoodleQuizResultDTO
    {
        [DataMember]
        public virtual int QuizId { get; set; }
        [DataMember]
        public virtual int QuestionId { get; set; }
        [DataMember]
        public virtual bool Answer { get; set; }
        [DataMember]
        public virtual int UserId { get; set; }
        [DataMember]
        public virtual int StartTime { get; set; }
    }
}
