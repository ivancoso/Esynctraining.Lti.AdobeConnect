using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EdugameCloud.Lti.API.AdobeConnect;
using EdugameCloud.Lti.Core.Business.Models;
using EdugameCloud.Lti.Core.DTO;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.DTO;
using Esynctraining.Core.Logging;
using Esynctraining.Lti.Lms.Common.Dto;

namespace EdugameCloud.Lti.Bridge
{
    public class BridgeMeetingSessionService : IMeetingSessionService
    {
        private readonly LmsCourseMeetingModel _lmsCourseMeetingModel;
        private readonly ICalendarExportService _calendarExportService;
        private readonly ILmsLicense _license;
        private readonly LtiParamDTO _param;
        private readonly ILogger _logger;
        private readonly IBridgeApi _api;

        public BridgeMeetingSessionService(LmsCourseMeetingModel lmsCourseMeetingModel, ILogger logger, ICalendarExportService calendarExportService, IBridgeApi api, ILmsLicense license, LtiParamDTO param)
        {
            _lmsCourseMeetingModel = lmsCourseMeetingModel ?? throw new ArgumentNullException(nameof(lmsCourseMeetingModel));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _calendarExportService = calendarExportService;
            _license = license;
            _param = param;
            _api = api;
        }

        public async Task<IEnumerable<MeetingSessionDTO>> CreateBatchAsync(CreateMeetingSessionsBatchDto dto, LtiParamDTO param)
        {
            LmsCourseMeeting meeting = _lmsCourseMeetingModel.GetOneById(dto.MeetingId).Value;
            DateTime startDate = dto.StartTimestamp;

            var endDate = TimeSpan.TryParse(dto.Duration, out var duration)
                ? startDate.AddMinutes((int)duration.TotalMinutes)
                : startDate.AddHours(1);

            //MeetingNameInfo nameInfo = string.IsNullOrWhiteSpace(meeting.MeetingNameJson)
            //    ? new MeetingNameInfo()
            //    : JsonConvert.DeserializeObject<MeetingNameInfo>(meeting.MeetingNameJson);
            //var meetingName = nameInfo.meetingName ?? nameInfo.reusedMeetingName;
            //int i = 1;
            var latestDateToCheck = startDate.AddDays(dto.Weeks * 7);

            var listOfEvents = new List<MeetingSessionDTO>();
            while (startDate < latestDateToCheck)
            {
                if (dto.DaysOfWeek.Contains((int)startDate.DayOfWeek))
                {
                    var ev = new MeetingSessionDTO
                    {
                        //Name = meetingName + " #" + i.ToString(),
                        Summary = string.Empty,
                        StartDate = startDate,//.ToString("yyyy-MM-dd HH:mm"),
                        EndDate = endDate,//.ToString("yyyy-MM-dd HH:mm"),
                    };
                    listOfEvents.Add(ev);
                    //i++;
                }

                startDate = startDate.AddDays(1);
                endDate = endDate.AddDays(1);
            }

            if (_calendarExportService != null)
            {
                listOfEvents = (await _calendarExportService.SaveEventsAsync(meeting.Id, listOfEvents, param, _license))
                    .ToList();
            }

            return listOfEvents.ToArray();
        }

        public async Task<IEnumerable<MeetingSessionDTO>> GetSessions(int meetingId)
        {
            var sessions = await _api.ListSessions(_param.course_id.ToString(), _license.GetLMSSettings(null)); //todo: settings parameter
            return sessions.Select(ConvertToDto);
        }

        private MeetingSessionDTO ConvertToDto(LiveSessionResponse resp)
        {
            return new MeetingSessionDTO
            {
                Id = resp.id,
                StartDate = resp.start_at.Value,
                EndDate = resp.end_at.Value,
                EventId = resp.id.ToString(),
                Summary = resp.notes,
                Name = resp.start_at.Value.ToString()
            };
        }

        public async Task<MeetingSessionDTO> CreateSessionAsync(int meetingId, LtiParamDTO param)
        {
            //LmsCourseMeeting meeting = _lmsCourseMeetingModel.GetOneById(meetingId).Value;
            //MeetingNameInfo nameInfo = string.IsNullOrWhiteSpace(meeting.MeetingNameJson)
            //    ? new MeetingNameInfo()
            //    : JsonConvert.DeserializeObject<MeetingNameInfo>(meeting.MeetingNameJson);
            //var lastEvent = meeting.MeetingSessions.OrderByDescending(x => x.StartDate).FirstOrDefault();
            DateTime startDate = DateTime.UtcNow.AddHours(1);
            DateTime endDate = startDate.AddHours(1);
            //if (lastEvent != null)
            //{
            //    startDate = lastEvent.StartDate.AddDays(1);
            //    endDate = lastEvent.EndDate.AddDays(1);
            //}
            var ev = new MeetingSessionDTO
            {
                //Name = nameInfo.meetingName + " #",
                Summary = string.Empty,
                StartDate = startDate,
                EndDate = endDate,
            };

            if (_calendarExportService != null)
            {
                var session = await _calendarExportService.SaveEventsAsync(meetingId, new MeetingSessionDTO[] { ev }, param, _license);
                return session.Single();
            }

            return ev;
        }

        public async Task<MeetingSessionDTO> SaveSessionAsync(int meetingId, MeetingSessionDTO dto, LtiParamDTO param)
        {
            //LmsCourseMeeting meeting = _lmsCourseMeetingModel.GetOneById(meetingId).Value;
            //var dbEvent = meeting.MeetingSessions.SingleOrDefault(x => x.Id == dto.Id)
            //       ?? new LmsMeetingSession { LmsCourseMeeting = meeting };

            //if (dbEvent.Id == 0)
            //{
            //    meeting.MeetingSessions.Add(dbEvent);
            //}

            if (_calendarExportService != null)
            {
                //dto.EventId = dto.Id.ToString();
                var session = await _api.UpdateSession(param.course_id.ToString(), int.Parse(dto.EventId),
                    new LiveSessionRequest {start_at = dto.StartDate, end_at = dto.EndDate, notes = dto.Summary},
                    _license.GetLMSSettings(null)); //todo: settings parameter
                return ConvertToDto(session);
                //_calendarExportService.SaveEventsAsync(meetingId, new MeetingSessionDTO[] { dto }, param, _license);
                //dto = sakaiEventResult.Single();
            }
            //dbEvent.Name = dto.Name;
            //dbEvent.Summary = dto.Summary;
            //dbEvent.StartDate = dto.StartDate;
            //dbEvent.EndDate = dto.EndDate;

            //_lmsCourseMeetingModel.RegisterSave(meeting, true);
            //return ConvertFromEntity(dbEvent);
            return null;
        }

        public async Task DeleteSessionAsync(int meetingId, int id, LtiParamDTO param)
        {
            //LmsCourseMeeting meeting = null;
            //LmsMeetingSession dbEvent = null;
            //if (meetingId != 0)
            //{
            //    meeting = _lmsCourseMeetingModel.GetOneById(meetingId).Value;

            //    dbEvent = meeting.MeetingSessions.FirstOrDefault(x => x.Id == id);

            //    if (dbEvent == null)
            //    {
            //        throw new InvalidOperationException(
            //            $"Could not find meeting session in database. MeetingId={meetingId}, Id={id}");
            //    }
            //}

            //if (_calendarExportService != null)
            //{
            var deleteResult = (await _calendarExportService.DeleteEventsAsync(new[] { id.ToString() }, param, _license))
                .Single();

            //if (!string.IsNullOrWhiteSpace(deleteResult)
            //    && !dbEvent.EventId.Equals(deleteResult, StringComparison.InvariantCultureIgnoreCase))
            //{
            //    // TODO: logging
            //    throw new InvalidOperationException("Some events could not be removed from Sakai calendar.");
            //}
            //}

            //if (meetingId != 0)
            //{
            //    meeting.MeetingSessions.Remove(dbEvent);
            //    _lmsCourseMeetingModel.RegisterSave(meeting, true);
            //}
        }

        public async Task DeleteMeetingSessionsAsync(LmsCourseMeeting meeting, LtiParamDTO param)
        {
            //if (meeting.MeetingSessions.Any())
            //{
            //    var events = new HashSet<string>(meeting.MeetingSessions.Select(x => x.EventId));

            //    if (_calendarExportService != null)
            //    {
            //todo:uncomment if need to delete sessions from API when deleting meeting
            //var deleteResultIds = await _calendarExportService.DeleteEventsAsync(null, param, _license);

            //    if (!events.SetEquals(deleteResultIds))
            //    {
            //        _logger.Error(
            //            $"List of all calendar events {string.Join(",", events)}, response is  {string.Join(",", deleteResultIds.ToArray())} ");
            //        throw new InvalidOperationException("Some events could not be removed from Sakai calendar.");
            //    }
            //}

            //    meeting.MeetingSessions.Clear();
            //    _lmsCourseMeetingModel.RegisterSave(meeting);
            //}
        }
    }
}