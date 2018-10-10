using System.Collections.Generic;
using System.Threading.Tasks;
using Esynctraining.Lti.Zoom.Api.Dto.Sessions;
using Esynctraining.Lti.Zoom.Domain;

namespace Esynctraining.Lti.Zoom.Api.Services
{
    public interface IMeetingSessionService
    {
        Task<IEnumerable<MeetingSessionDto>> CreateBatchAsync(CreateMeetingSessionsBatchDto dto, LmsCourseMeeting meeting);
        Task<IEnumerable<MeetingSessionDto>> GetSessions(LmsCourseMeeting meeting);
        Task<MeetingSessionDto> CreateSessionAsync(LmsCourseMeeting meeting);
        Task<MeetingSessionDto> SaveSessionAsync(LmsCourseMeeting meeting, int sessionId, MeetingSessionUpdateDto session);
        Task DeleteSessionAsync(LmsCourseMeeting meeting, int sessionId);
        Task DeleteMeetingSessionsAsync(LmsCourseMeeting meeting);
    }
}