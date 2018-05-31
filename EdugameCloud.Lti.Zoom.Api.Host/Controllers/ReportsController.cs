using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using Edugamecloud.Lti.Zoom.Dto;
using Edugamecloud.Lti.Zoom.Services;
using EdugameCloud.Lti.Core.Business.Models;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.Zoom.Api.Host.FIlters;
using Esynctraining.Core.Caching;
using Esynctraining.Core.Domain;
using Esynctraining.Core.Logging;
using Esynctraining.Core.Providers;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace EdugameCloud.Lti.Zoom.Api.Host.Controllers
{
    [Microsoft.AspNetCore.Mvc.Route("reports")]
    public class ReportsController : BaseApiController
    {
        private readonly LmsCourseMeetingModel _lmsCourseMeetingModel;
        private readonly LmsUserSessionModel _userSessionModel;
        private readonly ZoomReportService _reportService;
        private readonly ZoomMeetingService _meetingService;

        public ReportsController(
            ApplicationSettingsProvider settings,
            ILogger logger, ICache cache, LmsCourseMeetingModel lmsCourseMeetingModel, LmsUserSessionModel userSessionModel,
            ZoomReportService reportService, ZoomMeetingService meetingService)
            : base(settings, logger, cache)
        {
            _lmsCourseMeetingModel = lmsCourseMeetingModel;
            _userSessionModel = userSessionModel;
            _reportService = reportService;
            _meetingService = meetingService;
        }

        [Microsoft.AspNetCore.Mvc.Route("meetings/{meetingId}/by-sessions")]
        [Microsoft.AspNetCore.Mvc.HttpGet]
        [LmsAuthorizeBase(ApiCallEnabled = true)]
        public virtual async Task<OperationResultWithData<IEnumerable<ZoomSessionDto>>> GetReportBySessions(int meetingId)
        {
            var dbMeeting = _lmsCourseMeetingModel.GetOneByCourseAndId(LmsCompany.Id, CourseId, meetingId);
            if (dbMeeting == null)
                return OperationResultWithData<IEnumerable<ZoomSessionDto>>.Error("Meeting not found");
            var sessions = _reportService.GetSessionsReport(dbMeeting.ScoId, dbMeeting.AudioProfileId);
            return sessions.ToSuccessResult();
        }

        [Microsoft.AspNetCore.Mvc.Route("meetings/{meetingId}/by-sessions/download")]
        [Microsoft.AspNetCore.Mvc.HttpGet]
        //[LmsAuthorizeBase]
        public virtual async Task<ActionResult> DownloadReport(int meetingId, string session, string type)
        {
            var s = GetReadOnlySession(session);
            LmsCompany license = s.LmsCompany;
            var param = s.LtiSession.LtiParam;
            var dbMeeting = _lmsCourseMeetingModel.GetOneByCourseAndId(license.Id, param.course_id, meetingId);

            //if (meeting == null && credentials.ConsumerKey == "b622bf8b-a120-4b40-816e-05f530a750d9" && param.course_id == 557)
            //{
            //    var mId = meetingId / 100000;
            //    viewModel = LmsCourseMeetingModel.GetOneByCourseAndId(credentials.Id, param.course_id, mId);
            if (dbMeeting == null)
                //404
                return NotFound(meetingId);
            var apiMeeting = _meetingService.GetMeetingDetails(dbMeeting.ScoId);
            var sessions = _reportService.GetSessionsReport(dbMeeting.ScoId, dbMeeting.AudioProfileId);

            byte[] fileBytes = new byte[0];
            var url = Settings.ReportsUrl.TrimEnd('/') + "/ReportBySession";
            var model = new
            {
                MeetingUrl = apiMeeting.JoinUrl,
                MeetingTitle = apiMeeting.Topic,
                CompanyName = license.LmsDomain,
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

            var stringContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
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
            //return Content("Reports implementation is still in progress...");
            return File(fileBytes,
                mimeType,
                $"SessionsReport_{DateTime.Now.ToString("yyyyMMddHHmm")}.{(type.ToLower() == "excel" ? "xls" : "pdf")}");
        }

        [Microsoft.AspNetCore.Mvc.Route("meetings/{meetingId}/details/{meetingSessionId}/download")]
        [Microsoft.AspNetCore.Mvc.HttpGet]
        //[LmsAuthorizeBase]
        public virtual async Task<ActionResult> DownloadReportSessionDetails(int meetingId, string session, string meetingSessionId)
        {
            var s = GetReadOnlySession(session);
            LmsCompany license = s.LmsCompany;
            var param = s.LtiSession.LtiParam;
            var dbMeeting = _lmsCourseMeetingModel.GetOneByCourseAndId(license.Id, param.course_id, meetingId);

            //if (meeting == null && credentials.ConsumerKey == "b622bf8b-a120-4b40-816e-05f530a750d9" && param.course_id == 557)
            //{
            //    var mId = meetingId / 100000;
            //    viewModel = LmsCourseMeetingModel.GetOneByCourseAndId(credentials.Id, param.course_id, mId);
            if (dbMeeting == null)
                //404
                return NotFound(meetingId);
            var sessions = _reportService.GetSessionsReport(dbMeeting.ScoId, dbMeeting.AudioProfileId, WebUtility.UrlDecode(meetingSessionId).Replace(" ", "+"));
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

        protected LmsUserSession GetReadOnlySession(string key)
        {
            Guid uid;
            var session = Guid.TryParse(key, out uid) ? this._userSessionModel.GetByIdWithRelated(uid).Value : null;

            if (session == null)
            {
                Logger.WarnFormat("LmsUserSession not found. Key: {0}.", key);
                throw new Core.WarningMessageException(Resources.Messages.SessionTimeOut);
            }

            return session;
        }
    }
}