using System;
using Esynctraining.Lti.Lms.Common.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Esynctraining.Lti.Lms.Common.API.Canvas
{
    public interface IEGCEnabledCanvasAPI : IEGCEnabledLmsAPI, ICanvasAPI
    {
        Task<LmsUserDTO> GetCourseUser(string userId, Dictionary<string, object> licenseSettings, string courseId);
        Task<List<LmsUserDTO>> GetUsersForCourse(string domain, string courseid, Dictionary<string, object> licenseSettings);

        Task<LmsCalendarEventDTO> CreateCalendarEvent(string courseId, Dictionary<string, object> licenseSettings, LmsCalendarEventDTO lmsEvent);

        Task<LmsCalendarEventDTO> UpdateCalendarEvent(string courseId, Dictionary<string, object> licenseSettings, LmsCalendarEventDTO lmsEvent);

        Task UpdateCalendarEvent(string domain, string courseId, Dictionary<string, object> licenseSettings, DateTime startTime, string endTime, string title);

        Task<IEnumerable<LmsCalendarEventDTO>> GetUserCalendarEvents(string userId, Dictionary<string, object> licenseSettings);

        Task DeleteCalendarEvents(int eventId, Dictionary<string, object> licenseSettings);
    }

}
