using EdugameCloud.Lti.DTO;

namespace EdugameCloud.Lti.Sakai
{
    internal interface ISakaiQuestionParser
    {
        LmsQuestionDTO ParseQuestion(BBQuestionDTO dto);
    }
}