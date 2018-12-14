using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotNetOpenAuth.Messaging;
using EdugameCloud.Lti.Core.Business.MeetingNameFormatting;
using EdugameCloud.Lti.Core.Business.Models;
using EdugameCloud.Lti.Core.Domain.Entities;
using EdugameCloud.Lti.Core.DTO;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.DTO;
using Esynctraining.Core.Logging;
using Esynctraining.Core.Providers;
using Esynctraining.Core.Utils;
using Esynctraining.Lti.Lms.Common.API;
using Esynctraining.Lti.Lms.Common.Dto;
using Newtonsoft.Json;

namespace EdugameCloud.Lti.API.AdobeConnect
{
    public class MeetingSessionService : IMeetingSessionService
    {
        private dynamic Settings => IoC.Resolve<ApplicationSettingsProvider>();

        private readonly LmsCourseMeetingModel _lmsCourseMeetingModel;
        private readonly ICalendarExportService _calendarExportService;
        private readonly ILmsLicense _license;
        private readonly ILogger _logger;
        private readonly LmsCalendarEventServiceBase _calendarEventService;

        public MeetingSessionService(LmsCourseMeetingModel lmsCourseMeetingModel, ILogger logger, ICalendarExportService calendarExportService, ILmsLicense license, LmsCalendarEventServiceBase calendarEventService)
        {
            _lmsCourseMeetingModel = lmsCourseMeetingModel ?? throw new ArgumentNullException(nameof(lmsCourseMeetingModel));

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _calendarExportService = calendarExportService;
            _license = license;
            _calendarEventService = calendarEventService;
        }

        public async Task<IEnumerable<MeetingSessionDTO>> CreateBatchAsync(CreateMeetingSessionsBatchDto dto, LtiParamDTO param)
        {
            LmsCourseMeeting meeting = _lmsCourseMeetingModel.GetOneById(dto.MeetingId).Value;
            DateTime startDate = dto.StartTimestamp;
            if (dto.StartTimestamp < DateTime.UtcNow) //todo: remove these crazy conditions and parsing
            {
                FixDateTimeFields(dto);
                startDate = DateTime.UtcNow;
                if (DateTime.TryParse(dto.StartDate + " " + dto.StartTime, out var dateBegin))
                {
                    startDate = dateBegin;
                }
            }

            var endDate = TimeSpan.TryParse(dto.Duration, out var duration)
                ? startDate.AddMinutes((int) duration.TotalMinutes)
                : startDate.AddHours(1);

            MeetingNameInfo nameInfo = string.IsNullOrWhiteSpace(meeting.MeetingNameJson)
                ? new MeetingNameInfo()
                : JsonConvert.DeserializeObject<MeetingNameInfo>(meeting.MeetingNameJson);
            var meetingName = nameInfo.meetingName ?? nameInfo.reusedMeetingName;
            int i = 1;
            var latestDateToCheck = startDate.AddDays(dto.Weeks*7);

            var listOfEvents = new List<MeetingSessionDTO>();
            while (startDate < latestDateToCheck)
            {
                if (dto.DaysOfWeek.Contains((int)startDate.DayOfWeek))
                {
                    var ev = new MeetingSessionDTO
                    {
                        Name = meetingName + " #" + i.ToString(),
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

            if (_calendarExportService != null)
            {
                listOfEvents = (await _calendarExportService.SaveEventsAsync(meeting.Id, listOfEvents, param, _license))
                    .ToList();
            }

            meeting.MeetingSessions.AddRange(listOfEvents.Select(x => new LmsMeetingSession
            {
                EventId = x.EventId,
                Name = x.Name,
                StartDate = x.StartDate,
                EndDate = x.EndDate,
                LmsCourseMeeting = meeting
            }));

            await CreateCalendarEvents(param, meeting);

            _lmsCourseMeetingModel.RegisterSave(meeting, true);
            return meeting.MeetingSessions.Select(ConvertFromEntity).ToArray();
        }

        private async Task CreateCalendarEvents(LtiParamDTO param, LmsCourseMeeting meeting)
        {
            if (_calendarEventService == null)
                return;

            foreach (var session in meeting.MeetingSessions)
            {
                var eventDto = new LmsCalendarEventDTO
                {
                    StartAt = session.StartDate,
                    EndAt = session.EndDate,
                    Title = session.Name
                };

                var lmsSettings = _license.GetLMSSettings(Settings);

                try
                {
                    LmsCalendarEventDTO lmsCalendarEvent =
                        await _calendarEventService.CreateEvent(param.course_id.ToString(), lmsSettings, eventDto);

                    session.LmsCalendarEventId = lmsCalendarEvent?.Id;
                }
                catch (Exception e)
                {
                    _logger.Error(e.Message);
                    return;
                }
            }
        }

        public async Task<IEnumerable<MeetingSessionDTO>> GetSessions(int meetingId)
        {
            LmsCourseMeeting meeting = _lmsCourseMeetingModel.GetOneById(meetingId).Value;
            return meeting.MeetingSessions.Select(ConvertFromEntity).ToArray();
        }

        public async Task<MeetingSessionDTO> CreateSessionAsync(int meetingId, LtiParamDTO param)
        {
            LmsCourseMeeting meeting = _lmsCourseMeetingModel.GetOneById(meetingId).Value;
            MeetingNameInfo nameInfo = string.IsNullOrWhiteSpace(meeting.MeetingNameJson)
                ? new MeetingNameInfo()
                : JsonConvert.DeserializeObject<MeetingNameInfo>(meeting.MeetingNameJson);
            var lastEvent = meeting.MeetingSessions.OrderByDescending(x => x.StartDate).FirstOrDefault();
            DateTime startDate = DateTime.UtcNow.AddHours(1);
            DateTime endDate = startDate.AddHours(1);
            if (lastEvent != null)
            {
                startDate = lastEvent.StartDate.AddDays(1);
                endDate = lastEvent.EndDate.AddDays(1);
            }
            var ev = new MeetingSessionDTO
            {
                Name = nameInfo.meetingName + " #",
                Summary = string.Empty,
                StartDate = startDate,
                EndDate = endDate,
            };

            if (_calendarExportService != null)
            {
                var sakaiEventResult = await _calendarExportService.SaveEventsAsync(meetingId, new MeetingSessionDTO[] {ev}, param, _license);
                ev = sakaiEventResult.Single();
            }
            var dbEvent = new LmsMeetingSession
            {
                EventId = ev.EventId,
                Name = ev.Name,
                StartDate = ev.StartDate,
                EndDate = ev.EndDate,
                LmsCourseMeeting = meeting,
            };

            if (_calendarEventService != null)
            {
                var lmsSettings = _license.GetLMSSettings(Settings);
                LmsCalendarEventDTO calendarEventDto = new LmsCalendarEventDTO()
                {
                    Title = dbEvent.Name,
                    StartAt = dbEvent.StartDate,
                    EndAt = dbEvent.EndDate
                };
                LmsCalendarEventDTO newClaendarEventDto =  await _calendarEventService.CreateEvent(param.course_id.ToString(), lmsSettings, calendarEventDto);
                dbEvent.LmsCalendarEventId = newClaendarEventDto?.Id;
            }

            meeting.MeetingSessions.Add(dbEvent);
            _lmsCourseMeetingModel.RegisterSave(meeting, true);
            return ConvertFromEntity(dbEvent);
        }

        public async Task<MeetingSessionDTO> SaveSessionAsync(int meetingId, MeetingSessionDTO dto, LtiParamDTO param)
        {
            LmsCourseMeeting meeting = _lmsCourseMeetingModel.GetOneById(meetingId).Value;
            var dbEvent = meeting.MeetingSessions.SingleOrDefault(x => x.Id == dto.Id)
                   ?? new LmsMeetingSession { LmsCourseMeeting = meeting};

            if (dbEvent.Id == 0)
            {
                meeting.MeetingSessions.Add(dbEvent);
            }

            if (_calendarExportService != null)
            {
                dto.EventId = dbEvent.EventId;
                var sakaiEventResult = await _calendarExportService.SaveEventsAsync(meetingId, new MeetingSessionDTO[] {dto}, param, _license);
                dto = sakaiEventResult.Single();
            }

            if (_calendarEventService != null)
            {
                if (dbEvent.LmsCalendarEventId.HasValue)
                {
                    var lmsSettings = _license.GetLMSSettings(Settings);
                    LmsCalendarEventDTO calendarEventDto = new LmsCalendarEventDTO()
                    {
                        Id = dbEvent.LmsCalendarEventId.Value,
                        Title = dbEvent.Name,
                        StartAt = dbEvent.StartDate,
                        EndAt = dbEvent.EndDate
                    };
                    await _calendarEventService.UpdateEvent(param.course_id.ToString(), lmsSettings, calendarEventDto);
                }
            }

            dbEvent.Name = dto.Name;
            dbEvent.Summary = dto.Summary;
            dbEvent.StartDate = dto.StartDate;
            dbEvent.EndDate = dto.EndDate;

            _lmsCourseMeetingModel.RegisterSave(meeting, true);
            return ConvertFromEntity(dbEvent);
        }

        public async Task DeleteSessionAsync(int meetingId, int id, LtiParamDTO param)
        {
            LmsCourseMeeting meeting = null;
            LmsMeetingSession dbEvent = null;
            if (meetingId != 0)
            {
                meeting = _lmsCourseMeetingModel.GetOneById(meetingId).Value;

                dbEvent = meeting.MeetingSessions.FirstOrDefault(x => x.Id == id);

                if (dbEvent == null)
                {
                    throw new InvalidOperationException(
                        $"Could not find meeting session in database. MeetingId={meetingId}, Id={id}");
                }
            }

            if (_calendarExportService != null)
            {
                var deleteResult = (await _calendarExportService.DeleteEventsAsync(new[] { dbEvent.EventId }, param, _license))
                    .Single();

                if (!string.IsNullOrWhiteSpace(deleteResult)
                    && !dbEvent.EventId.Equals(deleteResult, StringComparison.InvariantCultureIgnoreCase))
                {
                    // TODO: logging
                    throw new InvalidOperationException("Some events could not be removed from Sakai calendar.");
                }
            }

            await RemoveCalendarEvent(dbEvent);

            if (meetingId != 0)
            {
                meeting.MeetingSessions.Remove(dbEvent);
                _lmsCourseMeetingModel.RegisterSave(meeting, true);
            }
        }

        public async Task DeleteMeetingSessionsAsync(LmsCourseMeeting meeting, LtiParamDTO param)
        {
            if (meeting.MeetingSessions.Any())
            {
                var events = new HashSet<string>(meeting.MeetingSessions.Select(x => x.EventId));

                if (_calendarExportService != null)
                {
                    var deleteResultIds = await _calendarExportService.DeleteEventsAsync(events, param, _license);

                    if (!events.SetEquals(deleteResultIds))
                    {
                        _logger.Error(
                            $"List of all calendar events {string.Join(",", events)}, response is  {string.Join(",", deleteResultIds.ToArray())} ");
                        throw new InvalidOperationException("Some events could not be removed from Sakai calendar.");
                    }
                }

                await RemoveCalendarEvents(meeting);

                meeting.MeetingSessions.Clear();
                _lmsCourseMeetingModel.RegisterSave(meeting);
            }
        }

        private async Task RemoveCalendarEvent(LmsMeetingSession dbEvent)
        {
            if (_calendarEventService != null)
            {
                try
                {
                    var lmsSettings = _license.GetLMSSettings(Settings);
                    if (dbEvent.LmsCalendarEventId.HasValue)
                    {
                        await _calendarEventService.DeleteCalendarEvent(dbEvent.LmsCalendarEventId.Value, lmsSettings);
                    }
                }
                catch (Exception e)
                {
                    _logger.Error(e.Message);
                }
            }
        }

        private async Task RemoveCalendarEvents(LmsCourseMeeting meeting)
        {
            if (_calendarEventService != null)
            {
                try
                {
                    var lmsSettings = _license.GetLMSSettings(Settings);
                    foreach (var session in meeting.MeetingSessions.Where(s => s.LmsCalendarEventId.HasValue))
                    {
                        await _calendarEventService.DeleteCalendarEvent(session.LmsCalendarEventId.Value, lmsSettings);
                    }
                }
                catch (Exception e)
                {
                    _logger.Error(e.Message);
                }
            }
        }


        private static void FixDateTimeFields(CreateMeetingSessionsBatchDto dto)
        {
            if (dto.StartTime != null)
            {
                dto.StartTime = dto.StartTime.PadLeft(8, '0');
            }

            if (dto.StartDate != null)
            {
                dto.StartDate = dto.StartDate.Substring(6, 4) + "-"
                                + dto.StartDate.Substring(0, 5);
            }
        }

        private MeetingSessionDTO ConvertFromEntity(LmsMeetingSession entity)
        {
            var result = new MeetingSessionDTO
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