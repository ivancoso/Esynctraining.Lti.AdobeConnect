using System;
using System.Collections.Generic;
using System.Linq;
using DotNetOpenAuth.Messaging;
using EdugameCloud.Lti.Core.Business.MeetingNameFormatting;
using EdugameCloud.Lti.Core.Business.Models;
using EdugameCloud.Lti.Core.Domain.Entities;
using EdugameCloud.Lti.Core.DTO;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.DTO;
using Esynctraining.Core.Logging;
using Newtonsoft.Json;

namespace EdugameCloud.Lti.API.AdobeConnect
{
    public class MeetingSessionService : IMeetingSessionService
    {
        private readonly LmsCourseMeetingModel _lmsCourseMeetingModel;
        private readonly ICalendarExportService _calendarExportService;
        private readonly ILogger _logger;

        public MeetingSessionService(LmsCourseMeetingModel lmsCourseMeetingModel, ILogger logger, ICalendarExportService calendarExportService)
        {
            _lmsCourseMeetingModel = lmsCourseMeetingModel ?? throw new ArgumentNullException(nameof(lmsCourseMeetingModel));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _calendarExportService = calendarExportService;
        }

        public IEnumerable<MeetingSessionDTO> CreateBatch(CreateMeetingSessionsBatchDto dto, LtiParamDTO param)
        {
            LmsCourseMeeting meeting = _lmsCourseMeetingModel.GetOneById(dto.MeetingId).Value;
            FixDateTimeFields(dto);
            DateTime startDate = DateTime.UtcNow;
            DateTime dateBegin;

            if (DateTime.TryParse(dto.StartDate + " " + dto.StartTime, out dateBegin))
            {
                startDate = dateBegin;
            }

            DateTime endDate = startDate.AddHours(1);
            TimeSpan duration;
            if (TimeSpan.TryParse(dto.Duration, out duration))
            {
                endDate =
                    startDate.AddMinutes((int)duration.TotalMinutes);
            }

            MeetingNameInfo nameInfo = string.IsNullOrWhiteSpace(meeting.MeetingNameJson)
                ? new MeetingNameInfo()
                : JsonConvert.DeserializeObject<MeetingNameInfo>(meeting.MeetingNameJson);
            var meetingName = nameInfo.meetingName ?? nameInfo.reusedMeetingName;
            int i = 1;
            var latestDateToCheck = startDate.AddDays(dto.Weeks*7);

            var listOfEvents = new List<MeetingSessionDTO>();
            while (startDate < latestDateToCheck)
            {
                if (dto.DaysOfWeek.Contains(startDate.DayOfWeek))
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
                listOfEvents = _calendarExportService.SaveEvents(meeting.Id, listOfEvents, param).ToList();
            }

            meeting.MeetingSessions.AddRange(listOfEvents.Select(x => new LmsMeetingSession
            {
                EventId = x.EventId,
                Name = x.Name,
                StartDate = x.StartDate,
                EndDate = x.EndDate,
                LmsCourseMeeting = meeting
            }));
            _lmsCourseMeetingModel.RegisterSave(meeting, true);
            return meeting.MeetingSessions.Select(ConvertFromEntity).ToArray();
        }

        public IEnumerable<MeetingSessionDTO> GetSessions(int meetingId)
        {
            LmsCourseMeeting meeting = _lmsCourseMeetingModel.GetOneById(meetingId).Value;
            return meeting.MeetingSessions.Select(ConvertFromEntity).ToArray();
        }

        public MeetingSessionDTO CreateSession(int meetingId, LtiParamDTO param)
        {
            LmsCourseMeeting meeting = _lmsCourseMeetingModel.GetOneById(meetingId).Value;
            MeetingNameInfo nameInfo = string.IsNullOrWhiteSpace(meeting.MeetingNameJson)
                ? new MeetingNameInfo()
                : JsonConvert.DeserializeObject<MeetingNameInfo>(meeting.MeetingNameJson);
            var lastEvent = meeting.MeetingSessions.OrderByDescending(x => x.StartDate).FirstOrDefault();
            DateTime startDate = DateTime.UtcNow;
            DateTime endDate = startDate.AddHours(1);
            if (lastEvent != null)
            {
                startDate = lastEvent.StartDate.AddDays(1);
                endDate = lastEvent.EndDate.AddDays(1);
            }
            var ev = new MeetingSessionDTO()
            {
                Name = nameInfo.meetingName + " #",
                Summary = string.Empty,
                StartDate = startDate,//.ToString("yyyy-MM-dd HH:mm"),
                EndDate = endDate,//.ToString("yyyy-MM-dd HH:mm"),
            };

            if (_calendarExportService != null)
            {
                var sakaiEventResult = _calendarExportService.SaveEvents(meetingId, new MeetingSessionDTO[] {ev}, param);
                ev = sakaiEventResult.Single();
            }
            var dbEvent = new LmsMeetingSession
            {
                EventId = ev.EventId,
                Name = ev.Name,
                StartDate = ev.StartDate,
                EndDate = ev.EndDate,
                LmsCourseMeeting = meeting
            };
            meeting.MeetingSessions.Add(dbEvent);
            _lmsCourseMeetingModel.RegisterSave(meeting, true);
            return ConvertFromEntity(dbEvent);
        }

        public MeetingSessionDTO SaveSession(int meetingId, MeetingSessionDTO dto, LtiParamDTO param)
        {
            //DateTime startDate;
            //DateTime endDate;
            //if (!DateTime.TryParseExact(dto.StartDate, "MM/dd/yyyy hh:mm tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out startDate) 
            //    || !DateTime.TryParseExact(dto.EndDate, "MM/dd/yyyy hh:mm tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out endDate))
            //{
            //    //todo : log
            //    throw new InvalidOperationException("Start date or End date is not correct.");
            //}

            //dto.StartDate = startDate.ToString("yyyy-MM-dd HH:mm");
            //dto.EndDate = endDate.ToString("yyyy-MM-dd HH:mm");

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
                var sakaiEventResult = _calendarExportService.SaveEvents(meetingId, new MeetingSessionDTO[] {dto}, param);
                dto = sakaiEventResult.Single();
            }
            dbEvent.Name = dto.Name;
            dbEvent.Summary = dto.Summary;
            dbEvent.StartDate = dto.StartDate;
            dbEvent.EndDate = dto.EndDate;

            _lmsCourseMeetingModel.RegisterSave(meeting, true);
            return ConvertFromEntity(dbEvent);
        }

        public void DeleteSession(int meetingId, int id, LtiParamDTO param)
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

            var deleteResult = _calendarExportService?.DeleteEvents(new[] {dbEvent.EventId}, param).Single();
            if (!string.IsNullOrWhiteSpace(deleteResult)
                && !dbEvent.EventId.Equals(deleteResult, StringComparison.InvariantCultureIgnoreCase))
            {
                //todo: logging
                throw new InvalidOperationException("Some events could not be removed from Sakai calendar.");
            }

            if (meetingId != 0)
            {
                meeting.MeetingSessions.Remove(dbEvent);
                _lmsCourseMeetingModel.RegisterSave(meeting, true);
            }
        }

        public void DeleteMeetingSessions(LmsCourseMeeting meeting, LtiParamDTO param)
        {
            if (meeting.MeetingSessions.Any())
            {
                var events = new HashSet<string>(meeting.MeetingSessions.Select(x => x.EventId));

                if (_calendarExportService != null)
                {
                    var deleteResultIds = _calendarExportService.DeleteEvents(events, param);

                    if (!events.SetEquals(deleteResultIds))
                    {
                        _logger.Error(
                            $"List of all calendar events {string.Join(",", events)}, response is  {string.Join(",", deleteResultIds.ToArray())} ");
                        throw new InvalidOperationException("Some events could not be removed from Sakai calendar.");
                    }
                }

                meeting.MeetingSessions.Clear();
                _lmsCourseMeetingModel.RegisterSave(meeting);
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
                StartDate = entity.StartDate,//.ToString("MM/dd/yyyy hh:mm tt"),
                EndDate = entity.EndDate,//.ToString("MM/dd/yyyy hh:mm tt"),
                Summary = entity.Summary
            };

            return result;
        }

    }

}