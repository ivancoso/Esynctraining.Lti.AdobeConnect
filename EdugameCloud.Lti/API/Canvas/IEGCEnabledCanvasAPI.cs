using System.Collections.Generic;
using System.Threading.Tasks;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.DTO;

namespace EdugameCloud.Lti.API.Canvas
{
    public interface IEGCEnabledCanvasAPI : IEGCEnabledLmsAPI, ICanvasAPI
    {

        Task<LmsUserDTO> GetCourseUser(string userId, ILmsLicense lmsCompany, string userToken, int courseid);
        Task<List<LmsUserDTO>> GetUsersForCourse(string domain, string userToken, int courseid);
        Task<List<LmsCourseSectionDTO>> GetCourseSections(string domain, string userToken, int courseId);

        //todo: async

        CanvasQuizSubmissionDTO CreateQuizSubmission(
            string api,
            string userToken,
            int courseid,
            int quizid);

        void AnswerQuestionsForQuiz(string api, string userToken, CanvasQuizSubmissionDTO submission);

        void CompleteQuizSubmission(
            string api,
            string userToken,
            int courseid,
            CanvasQuizSubmissionDTO submission);

    }

}
