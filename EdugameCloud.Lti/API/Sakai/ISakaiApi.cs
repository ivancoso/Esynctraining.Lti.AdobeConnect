using System.Collections.Generic;
using EdugameCloud.Lti.DTO;

namespace EdugameCloud.Lti.API.Sakai
{
    public interface ISakaiApi
    {
        IEnumerable<string> DeleteEvents(IEnumerable<string> eventIds, LtiParamDTO param);
        IEnumerable<SakaiEventDto> SaveEvents(int meetingId, IEnumerable<SakaiEventDto> eventDtos, LtiParamDTO param);
    }
}
