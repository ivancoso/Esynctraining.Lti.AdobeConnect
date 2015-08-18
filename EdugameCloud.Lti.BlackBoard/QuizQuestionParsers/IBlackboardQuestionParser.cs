using EdugameCloud.Lti.DTO;

namespace EdugameCloud.Lti.BlackBoard.QuizQuestionParsers
{
    internal interface IBlackboardQuestionParser
    {
        LmsQuestionDTO ParseQuestion(BBQuestionDTO dto);
    }
}