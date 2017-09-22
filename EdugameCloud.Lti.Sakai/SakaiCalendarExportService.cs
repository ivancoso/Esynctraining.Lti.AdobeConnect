using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using EdugameCloud.HttpClient;
using EdugameCloud.Lti.API.AdobeConnect;
using EdugameCloud.Lti.Core.Constants;
using EdugameCloud.Lti.DTO;
using EdugameCloud.Lti.Sakai.Dto;
using Newtonsoft.Json;

namespace EdugameCloud.Lti.Sakai
{
    internal sealed class SakaiCalendarExportService : ICalendarExportService
    {
        private static readonly HttpClientWrapper _httpClientWrapper = new HttpClientWrapper();

        public IEnumerable<MeetingSessionDTO> SaveEvents(int meetingId, IEnumerable<MeetingSessionDTO> eventDtos, LtiParamDTO param)
        {
            var apiParam = new SakaiApiObject
            {
                Params = new SakaiParams { LtiMessageType = "egc-submit-calendars" },
                Calendars = new SakaiCalendar[]
                {
                    new SakaiCalendar
                    {
                        MeetingId = meetingId.ToString(),
                        SiteId = param.context_id,
                        CalendarReference = "main",
                        ButtonSource = "https://www.incommon.org/images/joinbutton_03.png",
                        Secret = param.oauth_consumer_key,
                        Events = eventDtos.Select(ConvertSessionDtoToApiDto).ToArray()
                    }
                }
            };

            var json = JsonConvert.SerializeObject(apiParam);
            string resp = _httpClientWrapper.UploadJsonString(GetApiUrl(param), json);

            var events = JsonConvert.DeserializeObject<ExternalEventDto[]>(resp);
            var sessions = events.Select(ConvertFromApiDtoToSessionDto).ToList();
            foreach (var session in sessions)
            {
                var inEvent = eventDtos.SingleOrDefault(x => x.EventId == session.EventId);
                if (inEvent != null)
                {
                    session.Id = inEvent.Id;
                }
            }
            return sessions;
        }

        public string GetApiUrl(LtiParamDTO param)
        {
            var scheme = param.lis_outcome_service_url != null
                         && param.lis_outcome_service_url.IndexOf(HttpScheme.Https, StringComparison.InvariantCultureIgnoreCase) >= 0
                ? HttpScheme.Https
                : HttpScheme.Http;

            return $"{scheme}{param.lms_domain}/egcint/service/";
        }

        public IEnumerable<string> DeleteEvents(IEnumerable<string> eventIds, LtiParamDTO param)
        {
            var events = eventIds.Select(x => new SakaiEventDelete()
            {
                SakaiId = x
            });

            var apiParam = new SakaiApiDeleteObject
            {
                Params = new SakaiParams { LtiMessageType = "egc-delete-calendars" },
                Calendars = new SakaiCalendarDelete[]
                {
                    new SakaiCalendarDelete()
                    {
                        SiteId = param.context_id,
                        CalendarReference = "main",
                        Events = events.ToArray()
                    }
                }
            };

            var json = JsonConvert.SerializeObject(apiParam);
            string resp = _httpClientWrapper.UploadJsonString(GetApiUrl(param), json);

            return resp.Replace("\n", String.Empty).Replace("\r", String.Empty).Split(',');
        }

        private ExternalEventDto ConvertSessionDtoToApiDto(MeetingSessionDTO dto)
        {
            return new ExternalEventDto
            {
                ExternalId = dto.EventId,
                EgcId = dto.Id > 0 ? dto.Id.ToString() : dto.EventId,
                Name = dto.Name,
                Description = dto.Summary ?? string.Empty,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate
            };
        }

        private MeetingSessionDTO ConvertFromApiDtoToSessionDto(ExternalEventDto dto)
        {
            return new MeetingSessionDTO
            {
                EventId = dto.ExternalId,
                //                Id = dto.Id.ToString(),
                Name = dto.Name,
                Summary = dto.Description,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate
            };
        }
    }
}