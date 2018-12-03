using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Esynctraining.Core.Json;
using Esynctraining.Core.Logging;
using Esynctraining.Lti.Lms.Canvas;
using Esynctraining.Lti.Lms.Common.Dto;
using Esynctraining.Lti.Zoom.Common.Dto.Sessions;
using Esynctraining.Lti.Zoom.Common.Services;
using Esynctraining.Lti.Zoom.Domain;

namespace Esynctraining.Lti.Zoom.Api.Services
{
    public class MeetingSessionService : IMeetingSessionService
    {
        private readonly ZoomDbContext _dbContext;
        private readonly IJsonSerializer _jsonSerializer;
        private readonly ILmsLicenseAccessor _licenseAccessor;
        private readonly ZoomMeetingApiService _zoomMeetingApiService;
        //        private readonly LmsCourseMeetingModel _lmsCourseMeetingModel;
        //        //private readonly ICalendarExportService _calendarExportService;
        //        private readonly ILmsLicense _license;
        private readonly ILogger _logger;
        private readonly CalendarEventService _calendarEventService;

        private readonly ZoomMeetingService _meetingService;
        //
        public MeetingSessionService(
            ZoomDbContext dbContext, 
            IJsonSerializer jsonSerializer, 
            ILmsLicenseAccessor licenseAccessor, 
            ILogger logger, 
            ZoomMeetingService meetingService, 
            ZoomMeetingApiService zoomMeetingApiService, 
            CalendarEventService calendarEventService)
        {
            //            _lmsCourseMeetingModel = lmsCourseMeetingModel ?? throw new ArgumentNullException(nameof(lmsCourseMeetingModel));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            //            //_calendarExportService = calendarExportService;
            //            _license = license;
            _dbContext = dbContext;
            _jsonSerializer = jsonSerializer;
            _licenseAccessor = licenseAccessor;
            _meetingService = meetingService;
            _zoomMeetingApiService = zoomMeetingApiService;
            _calendarEventService = calendarEventService;
        }

        public async Task<IEnumerable<MeetingSessionDto>> CreateBatchAsync(CreateMeetingSessionsBatchDto dto, LmsCourseMeeting meeting, string courseId, Dictionary<string, object> lmsSettings)
        {
            var apiDetails = await _zoomMeetingApiService.GetMeetingApiDetails(meeting);
            DateTime startDate = dto.StartDate;

            var endDate = startDate.AddMinutes(dto.Duration);

            //MeetingNameInfo nameInfo = string.IsNullOrWhiteSpace(meeting.MeetingNameJson)
            //    ? new MeetingNameInfo()
            //    : JsonConvert.DeserializeObject<MeetingNameInfo>(meeting.MeetingNameJson);
            //var meetingName = nameInfo.meetingName ?? nameInfo.reusedMeetingName;
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

            foreach (var session in meetingSessions)
            {
                var lmsEvent = new LmsCalendarEventDTO
                {
                    StartAt = session.StartDate,
                    EndAt = session.EndDate,
                    Title = session.Name
                };
                var calendarEvent = await _calendarEventService.CreateEvent(courseId, lmsSettings, lmsEvent);

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
            //LmsCourseMeeting meeting = _lmsCourseMeetingModel.GetOneById(meetingId).Value;
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

            //if (_calendarExportService != null)
            //{
            //    var sakaiEventResult = await _calendarExportService.SaveEventsAsync(meetingId, new MeetingSessionDto[] { ev }, param, _license);
            //    ev = sakaiEventResult.Single();
            //}

            LmsCalendarEventDTO lmsCalendarEvent = new LmsCalendarEventDTO
            {
                Title = ev.Name,
                StartAt = ev.StartDate,
                EndAt = ev.EndDate
            };
            var newLmsEvent = await _calendarEventService.CreateEvent(meeting.CourseId, lmsSettings, lmsCalendarEvent);

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
            //LmsCourseMeeting meeting = _lmsCourseMeetingModel.GetOneById(meetingId).Value;
            var session = meeting.MeetingSessions.SingleOrDefault(x => x.Id == sessionId)
                          ?? new LmsMeetingSession { Meeting = meeting };

            if (session.Id == 0)
            {
                meeting.MeetingSessions.Add(session);
            }

            //if (_calendarExportService != null)
            //{
            //    dto.EventId = session.EventId;
            //    var sakaiEventResult = await _calendarExportService.SaveEventsAsync(meetingId, new MeetingSessionDto[] { dto }, param, _license);
            //    dto = sakaiEventResult.Single();
            //}
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
//            if (meetingId != 0)
//            {
//                meeting = _lmsCourseMeetingModel.GetOneById(meetingId).Value;

            session = meeting.MeetingSessions.FirstOrDefault(x => x.Id == id);

            if (session == null)
            {
                throw new InvalidOperationException(
                    $"Could not find meeting session in database. MeetingId={meeting.Id}, Id={id}");
            }
//            }

            //if (_calendarExportService != null)
            //{
            //    var deleteResult = (await _calendarExportService.DeleteEventsAsync(new[] { session.EventId }, param, _license))
            //        .Single();

            //    if (!string.IsNullOrWhiteSpace(deleteResult)
            //        && !session.EventId.Equals(deleteResult, StringComparison.InvariantCultureIgnoreCase))
            //    {
            //        // TODO: logging
            //        throw new InvalidOperationException("Some events could not be removed from Sakai calendar.");
            //    }
            //}

            //if (meetingId != 0)
            //{
            if (session.LmsCalendarEventId.HasValue)
            {
                await _calendarEventService.DeleteCalendarEvent(session.LmsCalendarEventId.Value, lmsSettings);
            }
            
            meeting.MeetingSessions.Remove(session);
            await _dbContext.SaveChangesAsync();
            //}
        }

        public async Task DeleteMeetingSessionsAsync(LmsCourseMeeting meeting, Dictionary<string, object> lmsSettings)
        {
            if (meeting.MeetingSessions.Any())
            {
                //var sessions = new HashSet<string>(meeting.MeetingSessions.Select(x => x.EventId));

                //if (_calendarExportService != null)
                //{
                //    var deleteResultIds = await _calendarExportService.DeleteEventsAsync(events, param, _license);

                //    if (!events.SetEquals(deleteResultIds))
                //    {
                //        _logger.Error(
                //            $"List of all calendar events {string.Join(",", events)}, response is  {string.Join(",", deleteResultIds.ToArray())} ");
                //        throw new InvalidOperationException("Some events could not be removed from Sakai calendar.");
                //    }
                //}

                foreach (var session in meeting.MeetingSessions.Where(s=> s.LmsCalendarEventId.HasValue))
                {
                    await _calendarEventService.DeleteCalendarEvent(session.LmsCalendarEventId.Value, lmsSettings);
                }

                meeting.MeetingSessions.Clear();
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}