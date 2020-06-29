using Esynctraining.Lti.Lms.Common.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Esynctraining.Lti.Lms.Common.API
{
    public abstract class LmsCalendarEventServiceBase
    {
        public abstract Task<LmsCalendarEventDTO> CreateEvent(string courseId, Dictionary<string, object> licenseSettings, LmsCalendarEventDTO lmsEvent);

        public abstract Task<LmsCalendarEventDTO> UpdateEvent(string courseId, Dictionary<string, object> licenseSettings, LmsCalendarEventDTO lmsEvent);

        public abstract Task<IEnumerable<LmsCalendarEventDTO>> GetUserCalendarEvents(string lmsUserId, Dictionary<string, object> licenseSettings);

        public abstract Task DeleteCalendarEvent(string eventId, Dictionary<string, object> licenseSettings);
    }
}
