using Esynctraining.Lti.Lms.Common.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;
using Esynctraining.Lti.Lms.Common.Dto.Canvas;

namespace Esynctraining.Lti.Lms.Common.API.Canvas
{
    public interface IEGCEnabledCanvasAPI : IEGCEnabledLmsAPI, ICanvasAPI
    {
        Task<LmsUserDTO> GetCourseUser(string userId, Dictionary<string, object> licenseSettings, string courseId);
        Task<List<LmsUserDTO>> GetUsersForCourse(string domain, string courseid, Dictionary<string, object> licenseSettings);

        //sections
        Task<List<LmsCourseSectionDTO>> GetCourseSections(string domain, string userToken, string courseId);

        //quizzes
        Task<CanvasQuizSubmissionDTO> CreateQuizSubmission(
            string api,
            string userToken,
            string courseid,
            int quizid);

        Task AnswerQuestionsForQuiz(string api, string userToken, CanvasQuizSubmissionDTO submission);

        Task CompleteQuizSubmission(
            string api,
            string userToken,
            string courseid,
            CanvasQuizSubmissionDTO submission);

        //calendar
        Task<LmsCalendarEventDTO> CreateCalendarEvent(string courseId, Dictionary<string, object> licenseSettings, LmsCalendarEventDTO lmsEvent);

        Task<LmsCalendarEventDTO> UpdateCalendarEvent(string courseId, Dictionary<string, object> licenseSettings, LmsCalendarEventDTO lmsEvent);

        Task<IEnumerable<LmsCalendarEventDTO>> GetUserCalendarEvents(string userId, Dictionary<string, object> licenseSettings);

        Task DeleteCalendarEvents(int eventId, Dictionary<string, object> licenseSettings);
    }

}
