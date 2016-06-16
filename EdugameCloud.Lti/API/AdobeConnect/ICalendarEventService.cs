using System.Collections.Generic;
using EdugameCloud.Lti.Core.DTO;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.DTO;

namespace EdugameCloud.Lti.API.AdobeConnect
{
    public interface ICalendarEventService
    {
        IEnumerable<CalendarEventDTO> CreateBatch(CreateCalendarEventsBatchDto dto, LtiParamDTO param);
        IEnumerable<CalendarEventDTO> GetEvents(int meetingId);
        CalendarEventDTO CreateEvent(int meetingId, LtiParamDTO param);
        CalendarEventDTO SaveEvent(int meetingId, CalendarEventDTO ev, LtiParamDTO param);
        void DeleteEvent(int meetingId, string eventId, LtiParamDTO param);
        void DeleteMeetingEvents(LmsCourseMeeting meeting, LtiParamDTO param);
    }
}