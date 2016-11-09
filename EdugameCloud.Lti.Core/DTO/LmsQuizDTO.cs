using System.Collections.Generic;

namespace EdugameCloud.Lti.DTO
{
    /// <summary>
    /// The quiz DTO.
    /// </summary>
    public class LmsQuizDTO : LmsQuizInfoDTO
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        public string description { get; set; }

        /// <summary>
        /// Gets or sets the questions.
        /// </summary>
        public virtual LmsQuestionDTO[] question_list { get; set; }

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
        /// All images for a quiz
        /// </summary>
        public Dictionary<string, byte[]> Images { get; set; }

        #endregion
    }
}