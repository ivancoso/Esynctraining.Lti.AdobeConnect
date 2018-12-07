using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EdugameCloud.Lti.API.AdobeConnect;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.DTO;
using Esynctraining.Core.Logging;
using Esynctraining.Core.Providers;
using Esynctraining.Lti.Lms.Common.Dto;

namespace EdugameCloud.Lti.Bridge
{
    public sealed class BridgeCalendarExportService : ICalendarExportService
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
            IEnumerable<LiveSessionResponse> sessions = await _api.CreateSessions(param.course_id.ToString(),
                eventDtos.Select(x => new LiveSessionRequest {start_at = x.StartDate,end_at = x.EndDate, notes = x.Summary}), license.GetLMSSettings(_settings));

            var users = await _api.GetCourseUsers(param.course_id.ToString(), license.GetLMSSettings(_settings));
            foreach (var session in sessions)
            {
                var webConf = await _api.UpdateSessionWebconference(param.course_id.ToString(), session.id,
                    new WebConferenceRequest
                    {
                        provider = "Other",
                        other_provider = "eSyncTraining AC",
                        meeting_url = $"{_settings.BasePath.TrimEnd('/')}/lti/meeting/join?session={"123"}&meetingId={meetingId}"
                    }, license.GetLMSSettings(_settings));
                var publishedSession = await _api.PublishSession(param.course_id.ToString(), session.id, license.GetLMSSettings(_settings));
                foreach (var user in users)
                {
                    var userRegistered = await _api.RegisterUserToSession(session.id, user.Id, license.GetLMSSettings(_settings));
                }

                //var evt = result.FirstOrDefault(x => session.start_at.Value == x.StartDate);
                //if (evt != null)
                //{
                //    evt.EventId = session.id.ToString();
                //}
            }

            //return result;

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

        public async Task<IEnumerable<string>> DeleteEventsAsync(IEnumerable<string> eventIds, LtiParamDTO param, ILmsLicense license)
        {
            List<string> ids = new List<string>();
            foreach (var id in eventIds)
            {
                var result = await _api.DeleteSession(param.course_id.ToString(), id, license.GetLMSSettings(_settings));
                if(result)
                    ids.Add(id);
            }

            return ids;
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
    }
}