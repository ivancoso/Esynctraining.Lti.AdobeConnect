namespace EdugameCloud.Lti.DTO
{
    using System.Collections.Generic;

    /// <summary>
    /// The quiz submission result DTO.
    /// </summary>
    public class QuizSubmissionResultDTO
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="QuizSubmissionResultDTO"/> class.
        /// </summary>
        public QuizSubmissionResultDTO()
        {
            this.quiz_submissions = new List<QuizSubmissionDTO>();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the quiz_submissions.
        /// </summary>
        public List<QuizSubmissionDTO> quiz_submissions { get; set; }

        #endregion
    }
}