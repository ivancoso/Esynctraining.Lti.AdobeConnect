namespace EdugameCloud.Lti.DTO
{
    using System.Collections.Generic;

    /// <summary>
    /// The quiz submission DTO.
    /// </summary>
    public class CanvasQuizSubmissionDTO
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CanvasQuizSubmissionDTO"/> class.
        /// </summary>
        public CanvasQuizSubmissionDTO()
        {
            this.quiz_questions = new List<CanvasQuizSubmissionQuestionDTO>();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the access code.
        /// </summary>
        public string access_code { get; set; }

        /// <summary>
        /// Gets or sets the attempt.
        /// </summary>
        public int attempt { get; set; }

        /// <summary>
        /// Gets or sets the finished at.
        /// </summary>
        public string finished_at { get; set; }

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// Gets or sets the quiz id.
        /// </summary>
        public int quiz_id { get; set; }

        /// <summary>
        /// Gets or sets the quiz questions.
        /// </summary>
        public List<CanvasQuizSubmissionQuestionDTO> quiz_questions { get; set; }

        /// <summary>
        /// Gets or sets the score.
        /// </summary>
        public int score { get; set; }

        /// <summary>
        /// Gets or sets the started at.
        /// </summary>
        public string started_at { get; set; }

        /// <summary>
        /// Gets or sets the submission id.
        /// </summary>
        public int submission_id { get; set; }

        /// <summary>
        /// Gets or sets the time spent.
        /// </summary>
        public int time_spent { get; set; }

        /// <summary>
        /// Gets or sets the user id.
        /// </summary>
        public int user_id { get; set; }

        /// <summary>
        /// Gets or sets the validation token.
        /// </summary>
        public string validation_token { get; set; }

        #endregion
    }
}