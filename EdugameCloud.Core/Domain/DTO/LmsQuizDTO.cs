namespace EdugameCloud.Core.Domain.DTO
{
    /// <summary>
    /// The quiz DTO.
    /// </summary>
    public class LmsQuizDTO
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        public string description { get; set; }

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// Gets or sets the questions.
        /// </summary>
        public QuizQuestionDTO[] questions { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        public string title { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether published.
        /// </summary>
        public bool published { get; set; }

        /// <summary>
        /// Gets or sets the quiz_type.
        /// </summary>
        public string quiz_type { get; set; }

        /// <summary>
        /// Gets or sets the course.
        /// </summary>
        public int course { get; set; }

        /// <summary>
        /// Gets or sets the course name.
        /// </summary>
        public string courseName { get; set; }

        /// <summary>
        /// Gets or sets the last modified lms.
        /// </summary>
        public int lastModifiedLMS { get; set; }

        #endregion
    }
}