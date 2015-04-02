namespace EdugameCloud.Lti.DTO
{
    using System.Collections.Generic;

    /// <summary>
    /// The quiz submission result DTO.
    /// </summary>
    public class CanvasQuizSubmissionResultDTO
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CanvasQuizSubmissionResultDTO"/> class.
        /// </summary>
        public CanvasQuizSubmissionResultDTO()
        {
            this.quiz_submissions = new List<CanvasQuizSubmissionDTO>();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the quiz_submissions.
        /// </summary>
        public List<CanvasQuizSubmissionDTO> quiz_submissions { get; set; }

        #endregion
    }
}