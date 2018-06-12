using System.Collections.Generic;
using System.Threading.Tasks;
using Esynctraining.Lti.Lms.Common.Dto;
using Esynctraining.Lti.Lms.Common.Dto.Canvas;

namespace Esynctraining.Lti.Lms.Common.API.Canvas
{
    public interface IEGCEnabledCanvasAPI : IEGCEnabledLmsAPI, ICanvasAPI
    {

        Task<LmsUserDTO> GetCourseUser(string userId, Dictionary<string, object> licenseSettings, string userToken, string courseid);
        Task<List<LmsUserDTO>> GetUsersForCourse(string domain, string userToken, string courseid);
        Task<List<LmsCourseSectionDTO>> GetCourseSections(string domain, string userToken, string courseId);

        //todo: async

        CanvasQuizSubmissionDTO CreateQuizSubmission(
            string api,
            string userToken,
            string courseId,
            int quizid);

        void AnswerQuestionsForQuiz(string api, string userToken, CanvasQuizSubmissionDTO submission);

        void CompleteQuizSubmission(
            string api,
            string userToken,
            string courseId,
            CanvasQuizSubmissionDTO submission);

    }

}
