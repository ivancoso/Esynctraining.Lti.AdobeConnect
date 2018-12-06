using EdugameCloud.Lti.DTO;
using Esynctraining.Lti.Lms.Common.Dto;

namespace EdugameCloud.Lti.BlackBoard.QuizQuestionParsers
{
    internal interface IBlackboardQuestionParser
    {
        LmsQuestionDTO ParseQuestion(BBQuestionDTO dto);
    }
}