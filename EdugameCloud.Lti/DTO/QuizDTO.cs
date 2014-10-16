namespace EdugameCloud.Lti.DTO
{
    /// <summary>
    /// The quiz DTO.
    /// </summary>
    public class QuizDTO
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

        #endregion
    }
}