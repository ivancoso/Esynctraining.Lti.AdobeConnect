namespace EdugameCloud.Lti.DTO
{
    public class QuizDTO
    {
        public int id { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public QuizQuestionDTO[] questions { get; set; }
    }
}
