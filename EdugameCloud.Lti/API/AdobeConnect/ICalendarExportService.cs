using System.Collections.Generic;
using System.Threading.Tasks;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.DTO;

namespace EdugameCloud.Lti.API.AdobeConnect
{
    public interface ICalendarExportService
    {
        Task<IEnumerable<string>> DeleteEventsAsync(IEnumerable<string> eventIds, LtiParamDTO param, ILmsLicense license);

        Task<IEnumerable<MeetingSessionDTO>> SaveEventsAsync(int meetingId, IEnumerable<MeetingSessionDTO> eventDtos, LtiParamDTO param, ILmsLicense license);

    }

}