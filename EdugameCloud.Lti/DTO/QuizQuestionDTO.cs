namespace EdugameCloud.Lti.DTO
{
    using System.Collections.Generic;

    public class QuizQuestionDTO
    {
        public QuizQuestionDTO()
        {
            answers = new List<AnswerDTO>();
        }
        public int id { get; set; }
        public int quiz_id { get; set; }
        public string question_name { get; set; }
        public string question_type { get; set; }
        public string question_text { get; set; }
        public List<AnswerDTO> answers { get; set; }
    }
}
