using System.Collections.Generic;
using EdugameCloud.Lti.DTO;

namespace EdugameCloud.Lti.API.Canvas
{
    public interface IEGCEnabledCanvasAPI : IEGCEnabledLmsAPI, ICanvasAPI
    {
        LmsUserDTO GetCourseUser(string userId, string domain, string usertoken, int courseid);
        List<LmsUserDTO> GetUsersForCourse(string domain, string usertoken, int courseid);

        List<CanvasQuizSubmissionDTO> GetSubmissionForQuiz(
            string api,
            string usertoken,
            int courseid,
            int quizid);

        void ReturnSubmissionForQuiz(
            string api,
            string usertoken,
            int courseid,
            CanvasQuizSubmissionDTO submission);
    }

}
