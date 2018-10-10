using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Esynctraining.Core.Json;
using Esynctraining.Core.Logging;
using Esynctraining.Lti.Zoom.Api.Dto.Sessions;
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

        private readonly ZoomMeetingService _meetingService;
        //
        public MeetingSessionService(ZoomDbContext dbContext, IJsonSerializer jsonSerializer, 
            ILmsLicenseAccessor licenseAccessor, ILogger logger, ZoomMeetingService meetingService, ZoomMeetingApiService zoomMeetingApiService)
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
        }

        public async Task<IEnumerable<MeetingSessionDto>> CreateBatchAsync(CreateMeetingSessionsBatchDto dto, LmsCourseMeeting meeting)
        {
            var apiDetails = await _zoomMeetingApiService.GetMeetingApiDetails(meeting);
            DateTime startDate = dto.StartDate;
            
            var endDate = TimeSpan.TryParse(dto.Duration, out var duration)
                ? startDate.AddMinutes((int)duration.TotalMinutes)
                : startDate.AddHours(1);

            //MeetingNameInfo nameInfo = string.IsNullOrWhiteSpace(meeting.MeetingNameJson)
            //    ? new MeetingNameInfo()
            //    : JsonConvert.DeserializeObject<MeetingNameInfo>(meeting.MeetingNameJson);
            //var meetingName = nameInfo.meetingName ?? nameInfo.reusedMeetingName;
            int i = 1;
            var latestDateToCheck = startDate.AddDays(dto.Weeks * 7);

            var listOfEvents = new List<MeetingSessionDto>();
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
                    listOfEvents.Add(ev);
                    i++;
                }

                startDate = startDate.AddDays(1);
                endDate = endDate.AddDays(1);
            }

            //if (_calendarExportService != null)
            //{
            //    listOfEvents = (await _calendarExportService.SaveEventsAsync(meeting.Id, listOfEvents, param, _license))
            //        .ToList();
            //}

            meeting.MeetingSessions.AddRange(listOfEvents.Select(x => new LmsMeetingSession
            {
                //EventId = x.EventId,
                Name = x.Name,
                StartDate = x.StartDate,
                EndDate = x.EndDate,
                Meeting = meeting
            }));

            await _dbContext.SaveChangesAsync();
            return meeting.MeetingSessions.Select(ConvertFromEntity).ToArray();
        }

        public async Task<IEnumerable<MeetingSessionDto>> GetSessions(LmsCourseMeeting meeting)
        {
            //LmsCourseMeeting meeting = _lmsCourseMeetingModel.GetOneById(meetingId).Value;
            return meeting.MeetingSessions.Select(ConvertFromEntity).ToArray();
        }

        public async Task<MeetingSessionDto> CreateSessionAsync(LmsCourseMeeting meeting)
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
            var session = new LmsMeetingSession
            {
                //EventId = ev.EventId,
                Name = ev.Name,
                StartDate = ev.StartDate,
                EndDate = ev.EndDate,
                Meeting = meeting,
            };
            meeting.MeetingSessions.Add(session);
            await _dbContext.SaveChangesAsync();
            return ConvertFromEntity(session);
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

            return ConvertFromEntity(session);
        }

        public async Task DeleteSessionAsync(LmsCourseMeeting meeting, int id)
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
            meeting.MeetingSessions.Remove(session);
            await _dbContext.SaveChangesAsync();
            //}
        }

        public async Task DeleteMeetingSessionsAsync(LmsCourseMeeting meeting)
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

                meeting.MeetingSessions.Clear();
                await _dbContext.SaveChangesAsync();
            }
        }


        //private static void FixDateTimeFields(CreateMeetingSessionsBatchDto dto)
        //{
        //    if (dto.StartTime != null)
        //    {
        //        dto.StartTime = dto.StartTime.PadLeft(8, '0');
        //    }

        //    if (dto.StartDate != null)
        //    {
        //        dto.StartDate = dto.StartDate.Substring(6, 4) + "-"
        //                        + dto.StartDate.Substring(0, 5);
        //    }
        //}

        private MeetingSessionDto ConvertFromEntity(LmsMeetingSession entity)
        {
            var result = new MeetingSessionDto
            {
                Id = entity.Id,
                Name = entity.Name,
                StartDate = entity.StartDate,
                EndDate = entity.EndDate,
                Summary = entity.Summary,
            };

            return result;
        }

    }
}