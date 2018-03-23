using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EdugameCloud.HttpClient;
using EdugameCloud.Lti.API.AdobeConnect;
using EdugameCloud.Lti.Core.Constants;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.DTO;
using Esynctraining.Core.Logging;
using Esynctraining.Core.Providers;
using Newtonsoft.Json;

namespace EdugameCloud.Lti.Bridge
{
    internal sealed class BridgeCalendarExportService : ICalendarExportService
    {
        private readonly IBridgeApi _api;
        private readonly ILogger _logger;
        private readonly dynamic _settings;
        public BridgeCalendarExportService(ILogger logger, IBridgeApi api, ApplicationSettingsProvider settings)
        {
            _api = api;
            _logger = logger;
            _settings = settings;
        }
        public async Task<IEnumerable<MeetingSessionDTO>> SaveEventsAsync(int meetingId, IEnumerable<MeetingSessionDTO> eventDtos, LtiParamDTO param, ILmsLicense license)
        {
            List<MeetingSessionDTO> result = eventDtos.ToList();
            var sessions = await _api.CreateSessions(param.course_id.ToString(),
                eventDtos.Select(x => new LiveSessionRequest {start_at = x.StartDate,end_at = x.EndDate, notes = x.Summary}), license);

            var users = await _api.GetCourseUsers(param.course_id.ToString(), license);
            foreach (var session in sessions)
            {
                var webConf = await _api.UpdateSessionWebconference(param.course_id.ToString(), session.id,
                    new WebConferenceRequest { provider = "Other", other_provider = "eSyncTraining AC", meeting_url = $"{_settings.BasePath.TrimEnd('/')}/lti/meeting/join?session={"123"}&meetingId={meetingId}" }, license);
                var publishedSession = await _api.PublishSession(param.course_id.ToString(), session.id, license);
                foreach (var user in users)
                {
                    var userRegistered = await _api.RegisterUserToSession(session.id, user.Id, license);
                }

                var evt = result.FirstOrDefault(x => session.start_at.Value == x.StartDate);
                if (evt != null)
                {
                    evt.EventId = session.id.ToString();
                }
            }

            return result;
        }

        public async Task<IEnumerable<string>> DeleteEventsAsync(IEnumerable<string> eventIds, LtiParamDTO param, ILmsLicense license)
        {
            foreach (var id in eventIds)
            {
                var result = await _api.DeleteSession(param.course_id.ToString(), id, license);
            }

            return null;
            //var events = eventIds.Select(x => new SakaiEventDelete()
            //{
            //    SakaiId = x
            //});

            //var apiParam = new SakaiApiDeleteObject
            //{
            //    Params = new SakaiParams { LtiMessageType = "egc-delete-calendars" },
            //    Calendars = new SakaiCalendarDelete[]
            //    {
            //        new SakaiCalendarDelete()
            //        {
            //            SiteId = param.context_id,
            //            CalendarReference = "main",
            //            Events = events.ToArray()
            //        }
            //    }
            //};

            //var json = JsonConvert.SerializeObject(apiParam);
            //string resp = await _httpClientWrapper.UploadJsonStringAsync(GetApiUrl(param), json);

            //return resp.Replace("\n", String.Empty).Replace("\r", String.Empty).Split(',');
        }

        private string GetApiUrl(LtiParamDTO param)
        {
            var scheme = param.lis_outcome_service_url != null
                         && param.lis_outcome_service_url.IndexOf(HttpScheme.Https, StringComparison.InvariantCultureIgnoreCase) >= 0
                ? HttpScheme.Https
                : HttpScheme.Http;

            return $"{scheme}{param.lms_domain}/egcint/service/";
        }

    }
}