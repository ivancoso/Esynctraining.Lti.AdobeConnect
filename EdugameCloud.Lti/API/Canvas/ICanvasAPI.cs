using EdugameCloud.Lti.DTO;

namespace EdugameCloud.Lti.API.Canvas
{
    public interface ICanvasAPI
    {
        AnnouncementDTO CreateAnnouncement(
            string api,
            string usertoken,
            int courseid,
            string title,
            string message);

        bool IsTokenExpired(string api, string usertoken);

        void AddMoreDetailsForUser(string api, string usertoken, LmsUserDTO user);

        void AnswerQuestionsForQuiz(string api, string usertoken, CanvasQuizSubmissionDTO submission);

    }

}
