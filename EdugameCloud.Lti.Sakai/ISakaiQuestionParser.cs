using EdugameCloud.Lti.DTO;
using Esynctraining.Lti.Lms.Common.Dto;

namespace EdugameCloud.Lti.Sakai
{
    internal interface ISakaiQuestionParser
    {
        LmsQuestionDTO ParseQuestion(BBQuestionDTO dto);
    }
}