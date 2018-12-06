using System.Collections.Generic;
using System.Threading.Tasks;
using EdugameCloud.Lti.Core.DTO;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.DTO;
using Esynctraining.Lti.Lms.Common.Dto;

namespace EdugameCloud.Lti.API.AdobeConnect
{
    public interface IMeetingSessionService
    {
        Task<IEnumerable<MeetingSessionDTO>> CreateBatchAsync(CreateMeetingSessionsBatchDto dto, LtiParamDTO param);
        Task<IEnumerable<MeetingSessionDTO>> GetSessions(int meetingId);
        Task<MeetingSessionDTO> CreateSessionAsync(int meetingId, LtiParamDTO param);
        Task<MeetingSessionDTO> SaveSessionAsync(int meetingId, MeetingSessionDTO ev, LtiParamDTO param);
        Task DeleteSessionAsync(int meetingId, int id, LtiParamDTO param);
        Task DeleteMeetingSessionsAsync(LmsCourseMeeting meeting, LtiParamDTO param);
    }
}