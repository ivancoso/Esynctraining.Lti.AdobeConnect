using Esynctraining.Core.Json;
using Esynctraining.Core.Logging;
using Esynctraining.Lti.Lms.Common.API;
using Esynctraining.Lti.Lms.Common.Dto;
using Esynctraining.Lti.Zoom.Common;
using Esynctraining.Lti.Zoom.Common.Dto.Sessions;
using Esynctraining.Lti.Zoom.Common.Services;
using Esynctraining.Lti.Zoom.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Esynctraining.Lti.Zoom.Api.Services
{
    public class MeetingSessionService : IMeetingSessionService
    {
        private readonly ZoomDbContext _dbContext;
        private readonly IJsonSerializer _jsonSerializer;
        private readonly ILmsLicenseAccessor _licenseAccessor;
        private readonly ZoomMeetingApiService _zoomMeetingApiService;
        private readonly ILogger _logger;
        private readonly LmsCalendarEventServiceBase _lmsEventService;
        private readonly LmsCalendarEventServiceFactory _lmsCalendarEventServiceFactory;

        private readonly ZoomMeetingService _meetingService;
        //
        public MeetingSessionService(
            ZoomDbContext dbContext, 
            IJsonSerializer jsonSerializer, 
            ILmsLicenseAccessor licenseAccessor, 
            ILogger logger, 
            ZoomMeetingService meetingService, 
            ZoomMeetingApiService zoomMeetingApiService,
            LmsCalendarEventServiceFactory lmsCalendarEventServiceFactory)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _dbContext = dbContext;
            _jsonSerializer = jsonSerializer;
            _licenseAccessor = licenseAccessor;
            _meetingService = meetingService;
            _zoomMeetingApiService = zoomMeetingApiService;
            _lmsCalendarEventServiceFactory = lmsCalendarEventServiceFactory ?? throw new ArgumentNullException(nameof(lmsCalendarEventServiceFactory)); ;
        }

        public async Task<IEnumerable<MeetingSessionDto>> CreateBatchAsync(CreateMeetingSessionsBatchDto dto, LmsCourseMeeting meeting, string courseId, Dictionary<string, object> lmsSettings)
        {

            var apiDetails = await _zoomMeetingApiService.GetMeetingApiDetails(meeting);
            DateTime startDate = dto.StartDate;

            var endDate = startDate.AddMinutes(dto.Duration);

            int i = 1;
            var latestDateToCheck = startDate.AddDays(dto.Weeks * 7);

            var meetingSessions = new List<MeetingSessionDto>();
            var lmsEvents = new List<LmsCalendarEventDTO>();
            while (startDate < latestDateToCheck)
            {
                if (dto.DaysOfWeek.Contains((int)startDate.DayOfWeek))
                {
                    var ev = new MeetingSessionDto
                    {
                        Name = apiDetails.Topic + " #" + i.ToString(),
                        Summary = string.Empty,
                        StartDate = startDate,//.ToString("yyyy-MM-dd HH:mm"),
                        EndDate = endDate,//.ToString("yyyy-MM-dd HH:mm"),
                    };
                    meetingSessions.Add(ev);
                    i++;
                }

                startDate = startDate.AddDays(1);
                endDate = endDate.AddDays(1);
            }

            var licenseDto = await _licenseAccessor.GetLicense();
            var lmsCalendarEventService = _lmsCalendarEventServiceFactory.GetService(licenseDto.ProductId, lmsSettings);
            

            foreach (var session in meetingSessions)
            {
                LmsCalendarEventDTO calendarEvent = null;
                if (lmsCalendarEventService != null)
                {
                    var lmsEvent = new LmsCalendarEventDTO(session.StartDate, session.EndDate, session.Name);
                    calendarEvent = await lmsCalendarEventService.CreateEvent(courseId, lmsSettings, lmsEvent);
                }

                var newLmsMeetingSession = new LmsMeetingSession
                {
                    Name = session.Name,
                    StartDate = session.StartDate,
                    EndDate = session.EndDate,
                    Meeting = meeting,
                    LmsCalendarEventId = calendarEvent?.Id
                };
                meeting.MeetingSessions.Add(newLmsMeetingSession);
            }

            await _dbContext.SaveChangesAsync();
            return meeting.MeetingSessions.Select(MeetingSessionConverter.ConvertFromEntity).ToArray();
        }

        public async Task<IEnumerable<MeetingSessionDto>> GetSessions(LmsCourseMeeting meeting)
        {
            return meeting.MeetingSessions.Select(MeetingSessionConverter.ConvertFromEntity).ToArray();
        }

        public async Task<MeetingSessionDto> CreateSessionAsync(LmsCourseMeeting meeting, Dictionary<string, object> lmsSettings)
        {
            var apiDetails = await _zoomMeetingApiService.GetMeetingApiDetails(meeting);
            
            var lastEvent = meeting.MeetingSessions.OrderByDescending(x => x.StartDate).FirstOrDefault();
            DateTime startDate = DateTime.UtcNow.AddHours(1);
            DateTime endDate = startDate.AddHours(1);
            if (lastEvent != null)
            {
                startDate = lastEvent.StartDate.AddDays(1);
                endDate = lastEvent.EndDate.AddDays(1);
            }
            var ev = new MeetingSessionDto
            {
                Name = apiDetails.Topic + " #",
                Summary = string.Empty,
                StartDate = startDate,
                EndDate = endDate,
            };

            var licenseDto = await _licenseAccessor.GetLicense();
            var lmsCalendarEventService = _lmsCalendarEventServiceFactory.GetService(licenseDto.ProductId, lmsSettings);
            LmsCalendarEventDTO newLmsEvent = null;
            if (lmsCalendarEventService != null)
            {
                LmsCalendarEventDTO lmsCalendarEvent = new LmsCalendarEventDTO(ev.StartDate, ev.EndDate, ev.Name);
                newLmsEvent =
                    await lmsCalendarEventService.CreateEvent(meeting.CourseId, lmsSettings, lmsCalendarEvent);
            }

            var session = new LmsMeetingSession
            {
                //EventId = ev.EventId,
                Name = ev.Name,
                StartDate = ev.StartDate,
                EndDate = ev.EndDate,
                Meeting = meeting,
                LmsCalendarEventId = newLmsEvent?.Id
            };
            meeting.MeetingSessions.Add(session);
            await _dbContext.SaveChangesAsync();
            return MeetingSessionConverter.ConvertFromEntity(session);
        }

        public async Task<MeetingSessionDto> SaveSessionAsync(LmsCourseMeeting meeting, int sessionId, MeetingSessionUpdateDto dto)
        {
            var session = meeting.MeetingSessions.SingleOrDefault(x => x.Id == sessionId)
                          ?? new LmsMeetingSession { Meeting = meeting };

            if (session.Id == 0)
            {
                meeting.MeetingSessions.Add(session);
            }

            session.Name = dto.Name;
            session.Summary = dto.Summary;
            session.StartDate = dto.StartDate;
            session.EndDate = dto.EndDate;

            await _dbContext.SaveChangesAsync();

            return MeetingSessionConverter.ConvertFromEntity(session);
        }

        public async Task DeleteSessionAsync(LmsCourseMeeting meeting, int id, Dictionary<string, object> lmsSettings)
        {
            LmsMeetingSession session = null;
            session = meeting.MeetingSessions.FirstOrDefault(x => x.Id == id);

            if (session == null)
            {
                throw new InvalidOperationException(
                    $"Could not find meeting session in database. MeetingId={meeting.Id}, Id={id}");
            }
            var licenseDto = await _licenseAccessor.GetLicense();
            var lmsCalendarEventService = _lmsCalendarEventServiceFactory.GetService(licenseDto.ProductId, lmsSettings);

            if (lmsCalendarEventService != null)
            {
                if (!string.IsNullOrEmpty(session.LmsCalendarEventId))
                {
                    await lmsCalendarEventService.DeleteCalendarEvent(session.LmsCalendarEventId, lmsSettings);
                }
            }

            meeting.MeetingSessions.Remove(session);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteMeetingSessionsAsync(LmsCourseMeeting meeting, Dictionary<string, object> lmsSettings)
        {
            if (meeting.MeetingSessions.Any())
            {
                var licenseDto = await _licenseAccessor.GetLicense();
                var lmsCalendarEventService = _lmsCalendarEventServiceFactory.GetService(licenseDto.ProductId, lmsSettings);
                if (lmsCalendarEventService != null)
                {
                    foreach (var session in meeting.MeetingSessions.Where(s => !string.IsNullOrEmpty(s.LmsCalendarEventId)))
                    {
                        await lmsCalendarEventService.DeleteCalendarEvent(session.LmsCalendarEventId,
                            lmsSettings);
                    }
                }

                meeting.MeetingSessions.Clear();
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}