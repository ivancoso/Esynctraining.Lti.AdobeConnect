namespace EdugameCloud.Core.Domain.DTO
{
    using System.Runtime.Serialization;

    [DataContract]
    public class QuizDataDTO
    {
        [DataMember]
        public DistractorFromStoredProcedureDTO[] distractors { get; set; }

        [DataMember]
        public QuestionFromStoredProcedureDTO[] questions { get; set; }

        [DataMember]
        public QuizDTO quizVO { get; set; }

    }

}