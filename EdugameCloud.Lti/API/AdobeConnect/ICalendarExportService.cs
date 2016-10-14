using System.Collections.Generic;
using EdugameCloud.Lti.DTO;

namespace EdugameCloud.Lti.API.AdobeConnect
{
    public interface ICalendarExportService
    {
        IEnumerable<string> DeleteEvents(IEnumerable<string> eventIds, LtiParamDTO param);
        IEnumerable<MeetingSessionDTO> SaveEvents(int meetingId, IEnumerable<MeetingSessionDTO> eventDtos, LtiParamDTO param);
    }
}