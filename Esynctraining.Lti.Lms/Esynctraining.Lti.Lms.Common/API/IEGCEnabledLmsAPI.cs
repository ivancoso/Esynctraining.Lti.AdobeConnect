using System.Collections.Generic;
using System.Threading.Tasks;
using Esynctraining.Lti.Lms.Common.Dto;

namespace Esynctraining.Lti.Lms.Common.API
{
    public interface IEGCEnabledLmsAPI : ILmsAPI
    {
        Task<(IEnumerable<LmsQuizInfoDTO> Data, string Error)> GetItemsInfoForUserAsync(LmsUserParametersDto lmsUserParameters, bool isSurvey);

        Task<(IEnumerable<LmsQuizDTO> Data, string Error)> GetItemsForUserAsync(LmsUserParametersDto lmsUserParameters, bool isSurvey, IEnumerable<int> quizIds);

        Task SendAnswersAsync(LmsUserParametersDto lmsUserParameters, string json, bool isSurvey, string[] answers = null);

        void PublishQuiz(LmsUserParametersDto lmsUserParameters, int courseId, int quizId);
    }
}
