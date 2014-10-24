namespace EdugameCloud.Core.Domain.DTO
{
    using System.Collections.Generic;

    /// <summary>
    /// The quiz question DTO.
    /// </summary>
    public class QuizQuestionDTO
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="QuizQuestionDTO"/> class.
        /// </summary>
        public QuizQuestionDTO()
        {
            this.answers = new List<AnswerDTO>();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the answers.
        /// </summary>
        public List<AnswerDTO> answers { get; set; }

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// Gets or sets the question name.
        /// </summary>
        public string question_name { get; set; }

        /// <summary>
        /// Gets or sets the question text.
        /// </summary>
        public string question_text { get; set; }

        /// <summary>
        /// Gets or sets the question type.
        /// </summary>
        public string question_type { get; set; }

        /// <summary>
        /// Gets or sets the quiz id.
        /// </summary>
        public int quiz_id { get; set; }

        #endregion
    }
}