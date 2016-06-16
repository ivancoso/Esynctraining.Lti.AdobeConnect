using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using EdugameCloud.Lti.API.Sakai;
using EdugameCloud.Lti.Core.Constants;
using EdugameCloud.Lti.DTO;
using EdugameCloud.Lti.Sakai.Dto;
using Esynctraining.Core.Logging;
using Newtonsoft.Json;

namespace EdugameCloud.Lti.Sakai
{
    internal sealed class SakaiApi : ISakaiApi
    {
        private readonly ILogger _logger;

        public SakaiApi(ILogger logger)
        {
            _logger = logger;
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
            string resp;
            using (var webClient = new WebClient())
            {
                webClient.Headers[HttpRequestHeader.ContentType] = "application/json";
                resp = webClient.UploadString(GetApiUrl(param), "POST", json);
            }

            return resp.Replace("\n", String.Empty).Replace("\r", String.Empty).Split(',');
        }

        public IEnumerable<SakaiEventDto> SaveEvents(int meetingId, IEnumerable<SakaiEventDto> eventDtos, LtiParamDTO param)
        {
            var apiParam = new SakaiApiObject
            {
                Params = new SakaiParams {LtiMessageType = "egc-submit-calendars"},
                Calendars = new SakaiCalendar[]
                {
                    new SakaiCalendar
                    {
                        MeetingId = meetingId.ToString(),
                        SiteId = param.context_id,
                        CalendarReference = "main",
                        ButtonSource = "https://www.incommon.org/images/joinbutton_03.png",
                        Secret = param.oauth_consumer_key,
                        Events = eventDtos.ToArray()
                    }
                }
            };

            var json = JsonConvert.SerializeObject(apiParam);
            string resp;
            using (var webClient = new WebClient())
            {
                webClient.Headers[HttpRequestHeader.ContentType] = "application/json";
                resp = webClient.UploadString(GetApiUrl(param), "POST", json);
            }

            var events = JsonConvert.DeserializeObject<SakaiEventDto[]>(resp);
            return events;
        }


        public string GetApiUrl(LtiParamDTO param)
        {
            var scheme = param.lis_outcome_service_url != null
                         && param.lis_outcome_service_url.IndexOf(HttpScheme.Https, StringComparison.InvariantCultureIgnoreCase) >= 0
                ? HttpScheme.Https
                : HttpScheme.Http;

            return $"{scheme}{param.lms_domain}/egcint/service/";
        }
    }
}