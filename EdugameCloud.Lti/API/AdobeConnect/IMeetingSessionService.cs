using System.Collections.Generic;
using EdugameCloud.Lti.Core.DTO;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.DTO;

namespace EdugameCloud.Lti.API.AdobeConnect
{
    public interface IMeetingSessionService
    {
        IEnumerable<MeetingSessionDTO> CreateBatch(CreateMeetingSessionsBatchDto dto, LtiParamDTO param);
        IEnumerable<MeetingSessionDTO> GetSessions(int meetingId);
        MeetingSessionDTO CreateSession(int meetingId, LtiParamDTO param);
        MeetingSessionDTO SaveSession(int meetingId, MeetingSessionDTO ev, LtiParamDTO param);
        void DeleteSession(int meetingId, int id, LtiParamDTO param);
        void DeleteMeetingSessions(LmsCourseMeeting meeting, LtiParamDTO param);
    }
}