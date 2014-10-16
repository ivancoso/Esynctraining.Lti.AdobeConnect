namespace EdugameCloud.Lti.DTO
{
    using System.Collections.Generic;

    public class QuizSubmissionResultDTO
    {
        public QuizSubmissionResultDTO()
        {
            quiz_submissions = new List<QuizSubmissionDTO>();
        }
        public List<QuizSubmissionDTO> quiz_submissions { get; set; }
    }
}
