namespace EdugameCloud.Lti.DTO
{
    public class AnswerDTO
    {
        public int id { get; set; }
        public string answer_text { get; set; }
        public string answer_match_left { get; set; }
        public string answer_match_right { get; set; }
        public string exact { get; set; }
    }
}
