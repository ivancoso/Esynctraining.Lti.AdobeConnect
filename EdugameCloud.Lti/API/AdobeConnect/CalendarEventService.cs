using System;
using System.Collections.Generic;
using System.Linq;
using DotNetOpenAuth.Messaging;
using EdugameCloud.Lti.API.Sakai;
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
    public class CalendarEventService : ICalendarEventService
    {
        private readonly LmsCourseMeetingModel _lmsCourseMeetingModel;
        private readonly ISakaiApi _sakaiApiService;
        private readonly ILogger _logger;

        public CalendarEventService(LmsCourseMeetingModel lmsCourseMeetingModel, ILogger logger, ISakaiApi sakaiApiService)
        {
            _lmsCourseMeetingModel = lmsCourseMeetingModel;
            _logger = logger;
            _sakaiApiService = sakaiApiService;
        }

        public IEnumerable<CalendarEventDTO> CreateBatch(CreateCalendarEventsBatchDto dto, LtiParamDTO param)
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
            int i = 1 ;
            var latestDateToCheck = startDate.AddDays(dto.Weeks*7);

            var listOfEvents = new List<SakaiEventDto>();
            while (startDate < latestDateToCheck)
            {
                
                if (dto.DaysOfWeek.Contains(startDate.DayOfWeek))
                {
                    var ev = new SakaiEventDto
                    {
                        Name = nameInfo.meetingName + " #" + i.ToString(),
                        Description = string.Empty,
                        EgcId = nameInfo.meetingName + " " + i.ToString(),
                        StartDate = startDate.ToString("yyyy-MM-dd HH:mm"),
                        EndDate = endDate.ToString("yyyy-MM-dd HH:mm"),
                    };
                    listOfEvents.Add(ev);
                    i++;
                }

                startDate = startDate.AddDays(1);
                endDate = endDate.AddDays(1);
            }

            var events = _sakaiApiService.SaveEvents(meeting.Id, listOfEvents, param);

            meeting.CalendarEvents.AddRange(events.Select(x => new LmsCalendarEvent
            {
                EventId = x.SakaiId,
                Name = x.Name,
                StartDate = DateTime.Parse(x.StartDate),
                EndDate = DateTime.Parse(x.EndDate),
                LmsCourseMeeting = meeting
            }));
            _lmsCourseMeetingModel.RegisterSave(meeting, true);
            return meeting.CalendarEvents.Select(ConvertFromEntity).ToArray();
        }

        public IEnumerable<CalendarEventDTO> GetEvents(int meetingId)
        {
            LmsCourseMeeting meeting = _lmsCourseMeetingModel.GetOneById(meetingId).Value;
            return meeting.CalendarEvents.Select(ConvertFromEntity).ToArray();
        }

        public CalendarEventDTO CreateEvent(int meetingId, LtiParamDTO param)
        {
            LmsCourseMeeting meeting = _lmsCourseMeetingModel.GetOneById(meetingId).Value;
            MeetingNameInfo nameInfo = string.IsNullOrWhiteSpace(meeting.MeetingNameJson)
                ? new MeetingNameInfo()
                : JsonConvert.DeserializeObject<MeetingNameInfo>(meeting.MeetingNameJson);
            var lastEvent = meeting.CalendarEvents.OrderByDescending(x => x.StartDate).FirstOrDefault();
            DateTime startDate = DateTime.UtcNow;
            DateTime endDate = startDate.AddHours(1);
            if (lastEvent != null)
            {
                startDate = lastEvent.StartDate.AddDays(1);
                endDate = lastEvent.EndDate.AddDays(1);
            }
            var ev = new SakaiEventDto()
            {
                Name = nameInfo.meetingName + " #",
                Description = string.Empty,
                EgcId = nameInfo.meetingName,
                StartDate = startDate.ToString("yyyy-MM-dd HH:mm"),
                EndDate = endDate.ToString("yyyy-MM-dd HH:mm"),
            };

            var sakaiEventResult = _sakaiApiService.SaveEvents(meetingId, new SakaiEventDto[] {ev}, param);

            var sakaiEvent = sakaiEventResult.Single();
            var dbEvent = new LmsCalendarEvent
            {
                EventId = sakaiEvent.SakaiId,
                Name = sakaiEvent.Name,
                StartDate = DateTime.Parse(sakaiEvent.StartDate),
                EndDate = DateTime.Parse(sakaiEvent.EndDate),
                LmsCourseMeeting = meeting
            };
            meeting.CalendarEvents.Add(dbEvent);
            _lmsCourseMeetingModel.RegisterSave(meeting, true);
            return ConvertFromEntity(dbEvent);
        }

        public CalendarEventDTO SaveEvent(int meetingId, CalendarEventDTO dto, LtiParamDTO param)
        {
            LmsCourseMeeting meeting = _lmsCourseMeetingModel.GetOneById(meetingId).Value;
            var dbEvent = meeting.CalendarEvents.Single(x => x.EventId == dto.EventId);

            DateTime startDate;
            DateTime endDate;
            if (!DateTime.TryParse(dto.StartDate, out startDate) || !DateTime.TryParse(dto.EndDate, out endDate))
            {
                //todo : log
                throw new InvalidOperationException("Start date or End date is not correct.");
            }

            var ev = new SakaiEventDto
            {
                SakaiId = dto.EventId,
                Name = dto.Name,
                Description = string.Empty,
                EgcId = dto.Name,
                StartDate = startDate.ToString("yyyy-MM-dd HH:mm"),
                EndDate = endDate.ToString("yyyy-MM-dd HH:mm"),
            };

            var sakaiEventResult = _sakaiApiService.SaveEvents(meetingId, new SakaiEventDto[] {ev}, param);
            var sakaiEvent = sakaiEventResult.Single();

            dbEvent.EventId = sakaiEvent.SakaiId;
            dbEvent.Name = sakaiEvent.Name;
            dbEvent.StartDate = DateTime.Parse(sakaiEvent.StartDate);
            dbEvent.EndDate = DateTime.Parse(sakaiEvent.EndDate);

            _lmsCourseMeetingModel.RegisterSave(meeting, true);
            return ConvertFromEntity(dbEvent);
        }

        public void DeleteEvent(int meetingId, string eventId, LtiParamDTO param)
        {
            LmsCourseMeeting meeting = null;
            LmsCalendarEvent dbEvent = null;
            if (meetingId != 0)
            {
                meeting = _lmsCourseMeetingModel.GetOneById(meetingId).Value;

                dbEvent = meeting.CalendarEvents.FirstOrDefault(x => x.EventId == eventId);

                if (dbEvent == null)
                {
                    //todo: try to remove from sakai ?
                    throw new InvalidOperationException(
                        $"Could not find event in database. MeetingId={meetingId}, EventId={eventId}");
                }
            }

            var deleteResult = _sakaiApiService.DeleteEvents(new[] {dbEvent.EventId}, param).Single();


            if (!string.IsNullOrWhiteSpace(deleteResult)
                && !dbEvent.EventId.Equals(deleteResult, StringComparison.InvariantCultureIgnoreCase))
            {
                //todo: logging
                throw new InvalidOperationException("Some events could not be removed from Sakai calendar.");
            }

            if (meetingId != 0)
            {
                meeting.CalendarEvents.Remove(dbEvent);
                _lmsCourseMeetingModel.RegisterSave(meeting, true);
            }
        }

        public void DeleteMeetingEvents(LmsCourseMeeting meeting, LtiParamDTO param)
        {
            if (meeting.CalendarEvents.Any())
            {
                var events = new HashSet<string>(meeting.CalendarEvents.Select(x => x.EventId));


                var deleteResultIds = _sakaiApiService.DeleteEvents(events, param);
                
                if (!events.SetEquals(deleteResultIds))
                {
                    _logger.Error($"List of all calendar events { string.Join(",", events) }, response is  {string.Join(",", deleteResultIds.ToArray())} ");
                    throw new InvalidOperationException("Some events could not be removed from Sakai calendar.");
                }

                meeting.CalendarEvents.Clear();
                _lmsCourseMeetingModel.RegisterSave(meeting);
            }
        }


        private static void FixDateTimeFields(CreateCalendarEventsBatchDto dto)
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

        private CalendarEventDTO ConvertFromEntity(LmsCalendarEvent entity)
        {
            var result = new CalendarEventDTO
            {
                Name = entity.Name,
                EventId = entity.EventId,
                StartDate = entity.StartDate.ToString("MM/dd/yyyy hh:mm tt"),
                EndDate = entity.EndDate.ToString("MM/dd/yyyy hh:mm tt")
            };

            return result;
        }
    }
}