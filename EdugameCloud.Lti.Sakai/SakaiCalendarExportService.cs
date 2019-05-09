using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using EdugameCloud.Lti.API.AdobeConnect;
using EdugameCloud.Lti.Core.Constants;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.DTO;
using EdugameCloud.Lti.Sakai.Dto;
using Esynctraining.Lti.Lms.Common.Dto;
using Newtonsoft.Json;

namespace EdugameCloud.Lti.Sakai
{
    public sealed class SakaiCalendarExportService : ICalendarExportService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public SakaiCalendarExportService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        }

        public async Task<IEnumerable<MeetingSessionDTO>> SaveEventsAsync(int meetingId, IEnumerable<MeetingSessionDTO> eventDtos, LtiParamDTO param, ILmsLicense license)
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

            var client = _httpClientFactory.CreateClient();
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PostAsync(GetApiUrl(param), content);
            var resp = await response.Content.ReadAsStringAsync();

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

        public async Task<IEnumerable<string>> DeleteEventsAsync(IEnumerable<string> eventIds, LtiParamDTO param, ILmsLicense license)
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

            var client = _httpClientFactory.CreateClient();
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PostAsync(GetApiUrl(param), content);
            var resp = await response.Content.ReadAsStringAsync();

            return resp.Replace("\n", String.Empty).Replace("\r", String.Empty).Split(',');
        }

        private string GetApiUrl(LtiParamDTO param)
        {
            var scheme = param.lis_outcome_service_url != null
                         && param.lis_outcome_service_url.IndexOf(HttpScheme.Https, StringComparison.InvariantCultureIgnoreCase) >= 0
                ? HttpScheme.Https
                : HttpScheme.Http;

            return $"{scheme}{param.lms_domain}/egcint/service/";
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