using System.Collections.Generic;
using System.Threading.Tasks;
using Esynctraining.Core.Domain;
using Esynctraining.Lti.Lms.Common.Dto;

namespace Esynctraining.Lti.Lms.Common.API
{
    public interface IEGCEnabledLmsAPI : ILmsAPI
    {
        Task<OperationResultWithData<IEnumerable<LmsQuizInfoDTO>>> GetItemsInfoForUserAsync(Dictionary<string, object> licenseSettings, bool isSurvey);
        Task<OperationResultWithData<IEnumerable<LmsQuizDTO>>> GetItemsForUserAsync(Dictionary<string, object> licenseSettings, bool isSurvey, IEnumerable<int> quizIds);
        Task SendAnswersAsync(Dictionary<string, object> licenseSettings, string json, bool isSurvey, string[] answers = null);
        Task PublishQuiz(Dictionary<string, object> licenseSettings, string courseId, int quizId);
    }
}
