namespace EdugameCloud.Lti.DTO
{
    /// <summary>
    /// The canvas quiz dto.
    /// </summary>
    public class CanvasQuizDTO : LmsQuizDTO
    {
        /// <summary>
        /// Gets or sets the questions.
        /// </summary>
        public CanvasQuestionDTO[] questions { get; set; }

        /// <summary>
        /// Gets the question_list.
        /// </summary>
        public override LmsQuestionDTO[] question_list
        {
            get
            {
                return this.questions;
            }
        }
    }
}
