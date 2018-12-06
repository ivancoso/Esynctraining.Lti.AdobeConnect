using System.Linq;
using EdugameCloud.Lti.DTO;
using Esynctraining.Lti.Lms.Common.Dto;

namespace EdugameCloud.Lti.BlackBoard.QuizQuestionParsers
{
    internal sealed class BlackboardMultipleChoiceParser : BlackboardCommonQuestionParser
    {
        public override LmsQuestionDTO ParseQuestion(BBQuestionDTO dto)
        {
            var lmsQuestion = base.ParseQuestion(dto);

            if (!string.IsNullOrEmpty(dto.questionImageLink) && !string.IsNullOrEmpty(dto.questionImageBinary))
            {
                var fileDto = new LmsQuestionFileDTO
                {
                    fileName = dto.questionImageLink.Split('/').Last(),
                    fileUrl = dto.questionImageLink,
                    base64Content = dto.questionImageBinary
                };
                lmsQuestion.files.Add(0, fileDto);
            }
            return lmsQuestion;
        }

        public BlackboardMultipleChoiceParser(BBAssessmentDTO td) : base(td)
        {
        }
    }
}