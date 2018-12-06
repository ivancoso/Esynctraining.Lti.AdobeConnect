using System.Collections.Generic;
using System.Threading.Tasks;
using Esynctraining.Lti.Lms.Common.Dto;

namespace Esynctraining.Lti.Lms.Common.API
{
    public interface IEGCEnabledLmsAPI : ILmsAPI
    {
        Task<(IEnumerable<LmsQuizInfoDTO> Data, string Error)> GetItemsInfoForUserAsync(Dictionary<string, object> licenseSettings, bool isSurvey);
        Task<(IEnumerable<LmsQuizDTO> Data, string Error)> GetItemsForUserAsync(Dictionary<string, object> licenseSettings, bool isSurvey, IEnumerable<int> quizIds);
        Task SendAnswersAsync(Dictionary<string, object> licenseSettings, string json, bool isSurvey, string[] answers = null);
        Task PublishQuiz(Dictionary<string, object> licenseSettings, string courseId, int quizId);
    }
}
