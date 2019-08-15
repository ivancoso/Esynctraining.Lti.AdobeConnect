using System.Collections.Generic;
using System.Threading.Tasks;
using Esynctraining.Lti.Lms.Common.Dto;
using Esynctraining.Lti.Zoom.Common.Dto.Sessions;
using Esynctraining.Lti.Zoom.Domain;

namespace Esynctraining.Lti.Zoom.Api.Services
{
    public interface IMeetingSessionService
    {
        Task<IEnumerable<MeetingSessionDto>> CreateBatchAsync(CreateMeetingSessionsBatchDto dto, LmsCourseMeeting meeting, string courseId, Dictionary<string, object> lmsSettings, ILtiParam ltiParam);
        Task<IEnumerable<MeetingSessionDto>> GetSessions(LmsCourseMeeting meeting);
        Task<MeetingSessionDto> CreateSessionAsync(LmsCourseMeeting meeting, Dictionary<string, object> lmsSettings, ILtiParam ltiParam);
        Task<MeetingSessionDto> SaveSessionAsync(LmsCourseMeeting meeting, int sessionId, MeetingSessionUpdateDto session, string courseId, Dictionary<string, object> lmsSettings);
        Task DeleteSessionAsync(LmsCourseMeeting meeting, int sessionId, Dictionary<string, object> lmsSettings);
        Task DeleteMeetingSessionsAsync(LmsCourseMeeting meeting, Dictionary<string, object> lmsSettings);
    }
}