using System.Collections.Generic;

namespace Esynctraining.Lti.Lms.Common.Dto.Canvas
{
    public class CanvasQuizSubmissionResultDTO
    {
        public CanvasQuizSubmissionResultDTO()
        {
            this.quiz_submissions = new List<CanvasQuizSubmissionDTO>();
        }

        public List<CanvasQuizSubmissionDTO> quiz_submissions { get; set; }

    }
}