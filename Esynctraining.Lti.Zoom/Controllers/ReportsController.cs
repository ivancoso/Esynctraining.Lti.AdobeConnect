using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using Esynctraining.Core.Json;
using Esynctraining.Core.Logging;
using Esynctraining.Core.Providers;
using Esynctraining.Lti.Lms.Common.Dto;
using Esynctraining.Lti.Zoom.Common.Dto;
using Esynctraining.Lti.Zoom.Common.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Esynctraining.Lti.Zoom.Controllers
{
    public class ReportsController : BaseController
    {
        private readonly ILmsLicenseService _lmsLicenseService;
        private readonly IJsonDeserializer _deserializer;
        private readonly IJsonSerializer _serializer;
        private readonly ZoomReportService _reportService;
        private readonly ZoomMeetingService _meetingService;

        public ReportsController(ILogger logger, ApplicationSettingsProvider settings, UserSessionService sessionService,
            ILmsLicenseService lmsLicenseService, IJsonSerializer serializer, IJsonDeserializer deserializer, ZoomReportService reportService,
            ZoomMeetingService meetingService) : base(logger, settings, sessionService)
        {
            _lmsLicenseService = lmsLicenseService;
            _serializer = serializer;
            _deserializer = deserializer;
            _reportService = reportService;
            _meetingService = meetingService;
        }

        [HttpGet]
        public virtual async Task<ActionResult> DownloadReport(int meetingId, string session, string type)
        {
            var s = await GetSession(session);
            LmsLicenseDto license = await _lmsLicenseService.GetLicense(s.LicenseKey);
            var param = _deserializer.JsonDeserialize<LtiParamDTO>(s.SessionData);
            var dbMeeting = await _meetingService.GetMeeting(meetingId, param.course_id.ToString());
            var apiMeeting = await _meetingService.GetMeetingDetails(meetingId, param.course_id.ToString());
            var sessions = _reportService.GetSessionsReport(dbMeeting.ProviderMeetingId, dbMeeting.ProviderHostId);

            byte[] fileBytes = new byte[0];
            var url = Settings.ReportsUrl.TrimEnd('/') + "/ReportBySession";
            var model = new
            {
                MeetingUrl = apiMeeting.JoinUrl,
                MeetingTitle = apiMeeting.Topic,
                CompanyName = license.Domain,
                CompanyLogo = "",
                IsExcelFormat = type.ToLower() == "excel",
                CourseName = param.context_title,
                LocalDate = DateTime.Now.ToString(),
                IsShowMeetingTitle = true,
                Sessions = sessions.Select(x => new
                {
                    dateStarted = x.StartedAt.ToString(),
                    dateEnded = x.EndedAt.ToString(),
                    meetingId = apiMeeting.Id,
                    sessionId = x.SessionId,
                    participantsCount = x.Participants.Count,
                    Participants = x.Participants.Select(p => new
                    {
                        participantName = p.ParticipantName,
                        dateTimeEntered = p.EnteredAt.ToString(),
                        dateTimeLeft = p.LeftAt.ToString()
                    }).ToArray()
                }).ToArray()
            };

            var stringContent = new StringContent(_serializer.JsonSerialize(model), Encoding.UTF8, "application/json");
            string mimeType = null;
            using (var client = new System.Net.Http.HttpClient())
            {

                using (var result = await client.PostAsync(url, stringContent))
                {
                    if (result.IsSuccessStatusCode)
                    {
                        fileBytes = await result.Content.ReadAsByteArrayAsync();
                        mimeType = result.Content.Headers.ContentType.MediaType;
                    }

                }
            }

            return File(fileBytes,
                mimeType,
                $"SessionsReport_{DateTime.Now.ToString("yyyyMMddHHmm")}.{(type.ToLower() == "excel" ? "xls" : "pdf")}");
        }

        [HttpGet]
        public virtual async Task<ActionResult> DownloadReportSessionDetails(int meetingId, string session, string meetingSessionId)
        {
            var s = await GetSession(session);
            var param = _deserializer.JsonDeserialize<LtiParamDTO>(s.SessionData);
            var dbMeeting = await _meetingService.GetMeeting(meetingId, param.course_id.ToString());
            if (dbMeeting == null)
                return NotFound(meetingId);

            var sessions = _reportService.GetSessionsReport(dbMeeting.ProviderMeetingId, dbMeeting.ProviderHostId, WebUtility.UrlDecode(meetingSessionId).Replace(" ", "+"));
            var records = sessions.First().Participants.Select(x => new
            {
                x.Details.Name,
                x.Details.EnteredAt,
                x.Details.LeftAt,
                x.Details.Device,
                x.Details.IpAddress,
                x.Details.Location,
                x.Details.NetworkType,
                ToolVersion = x.Details.Version
            });

            using (var mem = new MemoryStream())
            using (var writer = new StreamWriter(mem))
            using (var csvWriter = new CsvWriter(writer))
            {

                csvWriter.WriteRecords(records);
                writer.Flush();
                var result = mem.ToArray();
                //mem.
                return File(result, "text/csv", $"CsvReport_{DateTime.Now.ToString("yyyyMMddHHmm")}.csv");
            }

            //return Content("Reports implementation is still in progress...");
        }
    }
}