using System.Collections.Generic;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.DTO;

namespace EdugameCloud.Lti.API.Canvas
{
    public interface IEGCEnabledCanvasAPI : IEGCEnabledLmsAPI, ICanvasAPI
    {
        LmsUserDTO GetCourseUser(string userId, ILmsLicense lmsCompany, string userToken, int courseid);

        List<LmsUserDTO> GetUsersForCourse(string domain, string userToken, int courseid);

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

        List<LmsCourseSectionDTO> GetCourseSections(string domain, string userToken, int courseId);
    }

}
