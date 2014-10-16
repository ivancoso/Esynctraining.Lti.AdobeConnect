namespace EdugameCloud.Lti.DTO
{
    using System.Collections.Generic;

    using NHibernate.Mapping;

    public class QuizSubmissionDTO
    {
        public QuizSubmissionDTO()
        {
            quiz_questions = new List<QuizSubmissionQuestionDTO>();
        }

        public int id { get; set; }
        public int quiz_id { get; set; }
        public string access_code { get; set; }
        public string validation_token { get; set; }
        public int user_id { get; set; }
        public int submission_id { get; set; }
        public string started_at { get; set; }
        public string finished_at { get; set; }
        public int attempt { get; set; }
        public int time_spent { get; set; }
        public int score { get; set; }
        public List<QuizSubmissionQuestionDTO> quiz_questions { get; set; }
    }
}
