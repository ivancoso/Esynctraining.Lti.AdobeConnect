using System.Collections.Generic;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.DTO;

namespace EdugameCloud.Lti.API.Canvas
{
    public interface IEGCEnabledCanvasAPI : IEGCEnabledLmsAPI, ICanvasAPI
    {
        LmsUserDTO GetCourseUser(string userId, LmsCompany lmsCompany, string userToken, int courseid);

        List<LmsUserDTO> GetUsersForCourse(string domain, string userToken, int courseid);

        List<CanvasQuizSubmissionDTO> GetSubmissionForQuiz(
            string api,
            string userToken,
            int courseid,
            int quizid);

        void ReturnSubmissionForQuiz(
            string api,
            string userToken,
            int courseid,
            CanvasQuizSubmissionDTO submission);
    }

}
