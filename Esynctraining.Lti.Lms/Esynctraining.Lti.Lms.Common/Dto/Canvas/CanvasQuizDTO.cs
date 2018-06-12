namespace Esynctraining.Lti.Lms.Common.Dto.Canvas
{
    public class CanvasQuizDTO : LmsQuizDTO
    {
        public CanvasQuestionDTO[] questions { get; set; }

        public override LmsQuestionDTO[] question_list
        {
            get
            {
                return this.questions;
            }
        }
    }
}
