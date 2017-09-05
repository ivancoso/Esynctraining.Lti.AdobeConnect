using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Web.Mvc;
using System.Web.UI;
using EdugameCloud.Core.Business.Models;
using EdugameCloud.Core.Domain.Entities;
using EdugameCloud.Lti.API.AdobeConnect;
using EdugameCloud.Lti.Core.Business.Models;
using EdugameCloud.Lti.Core.Constants;
using EdugameCloud.Lti.Core.DTO;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.Extensions;
using EdugameCloud.Lti.Host.Dtos;
using Esynctraining.AdobeConnect.Api.MeetingReports;
using Esynctraining.AdobeConnect.Api.MeetingReports.Dto;
using Esynctraining.Core.Logging;
using Esynctraining.Core.Providers;
using Microsoft.Reporting.WebForms;
using NodaTime.TimeZones;

namespace EdugameCloud.Lti.Host.Areas.Reports.Controllers
{
    public class FileController : BaseController
    {
        private readonly LmsCourseMeetingModel lmsCourseMeetingModel;
        private readonly IReportsService meetingSetup;
        private readonly LtiReportService ltiReports = new LtiReportService();
        private readonly CompanyModel companyModel;
        private readonly ACUserModeModel userModeModel;


        public FileController(
            LmsCourseMeetingModel lmsCourseMeetingModel,
            IReportsService meetingSetup,
            CompanyModel companyModel,
            ACUserModeModel userModeModel,
            LmsUserSessionModel userSessionModel,
            Esynctraining.AdobeConnect.IAdobeConnectAccountService acAccountService,
            ApplicationSettingsProvider settings,
            ILogger logger)
            : base(userSessionModel, acAccountService, settings, logger)
        {
            this.lmsCourseMeetingModel = lmsCourseMeetingModel;
            this.meetingSetup = meetingSetup;
            this.companyModel = companyModel;
            this.userModeModel = userModeModel;
        }


        [HttpGet]
        [OutputCache(Duration = 0, NoStore = true, Location = OutputCacheLocation.None)]
        public virtual ActionResult MeetingAttendanceReport(string session, int meetingId, string timezone, string format = "PDF", int startIndex = 0, int limit = 0)
        {
            try
            {
                var s = this.GetReadOnlySession(session);
                var tz = GetTimeZoneInfoForTzdbId(timezone) ?? TimeZoneInfo.Utc;
                var credentials = s.LmsCompany;
                var acProvider = this.GetAdobeConnectProvider(credentials);

                LmsCourseMeeting meeting = lmsCourseMeetingModel.GetOneByCourseAndId(credentials.Id, s.LmsCourseId, meetingId);

                var acMeeting = acProvider.GetScoInfo(meeting.GetMeetingScoId());
                var acServer = credentials.AcServer;
                if (credentials.AcServer.EndsWith("/"))
                {
                    acServer = acServer.Substring(0, acServer.Length - 1);
                }

                var acMeetingUrl = acServer + acMeeting.ScoInfo.UrlPath;
                var acMeetingTitle = acMeeting.ScoInfo.Name;

                if (format.ToUpper() != "PDF" && format.ToUpper() != "EXCEL")
                {
                    this.RedirectToError("Unable to generate report in such format " + " \"" + format + "\"");
                    return null;
                }

                var tempParticipants = this.meetingSetup.GetAttendanceReports(
                    meeting.GetMeetingScoId(),
                    this.GetAdobeConnectProvider(credentials),
                    tz,
                    startIndex,
                    limit);

                var participants = new List<ACSessionParticipantReportDto>();
                if (tempParticipants.Any())
                {
                    participants = tempParticipants.Select(x => new ACSessionParticipantReportDto(x, tz)).ToList();
                }

                bool isShowMeetingTitle = false;
                bool.TryParse(credentials.GetSetting<string>(LmsCompanySettingNames.IsPdfMeetingUrl), out isShowMeetingTitle);
                string mimeType;
                var company = this.companyModel.GetOneById(s.LmsCompany.CompanyId).Value;

                if (company == null)
                {
                    this.RedirectToError(" Unable to retrieve data about company");
                    return null;
                }
                var localDate = GetLocalDate(tz);
                var parametersList = GetReportParameters(new ReportParamsDto(format, company, s, acMeetingUrl, acMeetingTitle, localDate, isShowMeetingTitle));
                var reportRenderedBytes = this.GenerateReportBytes(format, "MeetingAttendanceReport", participants,
                    out mimeType, parametersList);

                if (reportRenderedBytes != null)
                {
                    var reportName = GenerateReportName(s, "Attendance", localDate);
                    return this.File(
                        reportRenderedBytes,
                        mimeType,
                        string.Format("{0}.{1}", reportName, this.GetFileExtensions(format.ToUpper())));
                }

                this.RedirectToError("Unable to generate report. Try again later.");

            }
            catch (Exception ex)
            {
                this.logger.Error("MeetingAttendanceReport", ex);
                this.RedirectToError("Unable to generate report. Try again later.");
            }

            return null;
        }

        [HttpGet]
        [OutputCache(Duration = 0, NoStore = true, Location = OutputCacheLocation.None)]
        public virtual ActionResult MeetingSessionsReport(string session, int meetingId, string timezone, string format = "PDF", int startIndex = 0, int limit = 0)
        {
            try
            {
                var s = this.GetReadOnlySession(session);
                var credentials = s.LmsCompany;
                var acProvider = this.GetAdobeConnectProvider(credentials);
                var tz = GetTimeZoneInfoForTzdbId(timezone) ?? TimeZoneInfo.Utc;
                LmsCourseMeeting meeting = lmsCourseMeetingModel.GetOneByCourseAndId(credentials.Id, s.LmsCourseId, meetingId);

                var acMeeting = acProvider.GetScoInfo(meeting.GetMeetingScoId());
                var acMeetingUrl = credentials.AcServer + acMeeting.ScoInfo.UrlPath;
                var acMeetingTitle = acMeeting.ScoInfo.Name;

                var tempMeetingSessions = this.meetingSetup.GetSessionsReports(
                    meeting.GetMeetingScoId(),
                    acProvider,
                    tz,
                    startIndex,
                    limit);

                var meetingSessions = new List<ACSessionReportDto>();

                if (tempMeetingSessions.Any())
                {
                    meetingSessions = tempMeetingSessions.Select(x => new ACSessionReportDto(x, tz)).ToList();
                }

                if (format.ToUpper() != "PDF" && format.ToUpper() != "EXCEL")
                {
                    this.RedirectToError("Unable to generate report in such format " + " \"" + format + "\"");
                    return null;
                }

                var company = this.companyModel.GetOneById(s.LmsCompany.CompanyId).Value;

                if (company == null)
                {
                    this.RedirectToError("Unable to retrieve data about company");
                    return null;
                }
                var localDate = GetLocalDate(tz);
                bool isShowMeetingTitle = false;
                bool.TryParse(credentials.GetSetting<string>(LmsCompanySettingNames.IsPdfMeetingUrl), out isShowMeetingTitle);
                var parametersList = GetReportParameters(new ReportParamsDto(format, company, s, acMeetingUrl, acMeetingTitle, localDate, isShowMeetingTitle));
                var subreports = new Dictionary<string, KeyValuePair<string, SubreportProcessingEventHandler>>
                {
                    {
                        "ParticipantsSubReport",
                        new KeyValuePair<string, SubreportProcessingEventHandler>("ParticipantsSubReport",
                            (sender, args) =>
                            {
                                var assetId = args.Parameters[0].Values[0];
                                args.DataSources.Clear();

                                var sessions = (IEnumerable<ACSessionReportDto>)(((LocalReport) sender).DataSources["ItemDataSet"].Value);
                                var participants = new List<ACSessionParticipantReportDto>();
                                if (sessions.Any())
                                {
                                    var tempSession = sessions.FirstOrDefault(x => x.assetId == assetId);
                                    if (tempSession != null && tempSession.participants != null && tempSession.participants.Any())
                                    {
                                        participants = tempSession.participants;
                                    }
                                }
                                args.DataSources.Add(new ReportDataSource("ItemDataSet", participants));
                            })
                    }
                };

                string mimeType;
                var reportRenderedBytes = this.GenerateReportBytes(format, "MeetingSessionsReport", meetingSessions,
                    out mimeType, parametersList, subreports);

                if (reportRenderedBytes != null)
                {
                    var reportName = GenerateReportName(s, "Sessions", localDate);

                    return this.File(
                        reportRenderedBytes,
                        mimeType,
                        string.Format("{0}.{1}", reportName, this.GetFileExtensions(format.ToUpper())));
                }

                this.RedirectToError("Unable to generate report. Try again later.");
            }

            catch (Exception ex)
            {
                this.logger.Error("MeetingSessionsReport", ex);
                this.RedirectToError("Unable to generate report. Try again later.");
            }

            return null;
        }

        [HttpGet]
        [OutputCache(Duration = 0, NoStore = true, Location = OutputCacheLocation.None)]
        //[ActionName("meeting-recordings-report")]
        public virtual ActionResult MeetingRecordingsReport(string session, int meetingId, string timezone, string format = "PDF", int startIndex = 0, int limit = 0)
        {
            try
            {
                var s = this.GetReadOnlySession(session);
                var credentials = s.LmsCompany;
                var acProvider = this.GetAdobeConnectProvider(credentials);
                var tz = GetTimeZoneInfoForTzdbId(timezone) ?? TimeZoneInfo.Utc;
                LmsCourseMeeting meeting = lmsCourseMeetingModel.GetOneByCourseAndId(credentials.Id, s.LmsCourseId, meetingId);

                var acMeeting = acProvider.GetScoInfo(meeting.GetMeetingScoId());
                var acServer = credentials.AcServer;
                if (credentials.AcServer.EndsWith("/"))
                {
                    acServer = acServer.Substring(0, acServer.Length - 1);
                }

                if (format.ToUpper() != "PDF" && format.ToUpper() != "EXCEL")
                {
                    this.RedirectToError("Unable to generate report in such format " + " \"" + format + "\"");
                    return null;
                }

                var tempParticipants = ltiReports.GetRecordingsReport(
                    this.GetAdobeConnectProvider(credentials),
                    meeting,
                    startIndex,
                    limit);

                var participants = new List<ACRecordingViewReportDTO>();
                if (tempParticipants.Any())
                {
                    participants = tempParticipants.Select(x => new ACRecordingViewReportDTO(x, tz)).ToList();
                }

                string mimeType;
                var company = this.companyModel.GetOneById(s.LmsCompany.CompanyId).Value;

                if (company == null)
                {
                    this.RedirectToError(" Unable to retrieve data about company");
                    return null;
                }

                var acMeetingUrl = acServer + acMeeting.ScoInfo.UrlPath;
                var acMeetingTitle = acMeeting.ScoInfo.Name;
                var localDate = GetLocalDate(tz);
                bool isShowMeetingTitle;
                bool.TryParse(credentials.GetSetting<string>(LmsCompanySettingNames.IsPdfMeetingUrl), out isShowMeetingTitle);
                var parametersList = GetReportParameters(new ReportParamsDto(format, company, s, acMeetingUrl, acMeetingTitle, localDate, isShowMeetingTitle));

                var reportRenderedBytes = GenerateReportBytes(format, "RecordingAttendanceReport", participants,
                    out mimeType, parametersList);

                if (reportRenderedBytes != null)
                {
                    var reportName = GenerateReportName(s, "RecordingViews", localDate);
                    return this.File(
                        reportRenderedBytes,
                        mimeType,
                        string.Format("{0}.{1}", reportName, this.GetFileExtensions(format.ToUpper())));
                }

                this.RedirectToError("Unable to generate report. Try again later.");

            }
            catch (Exception ex)
            {
                this.logger.Error("MeetingRecordingsReport", ex);
                this.RedirectToError("Unable to generate report. Try again later.");
            }

            return null;
        }

        #region Methods

        private void RedirectToError(string errorText)
        {
            this.Response.Clear();
            this.Response.Write(string.Format("<h1>{0}</h1>", errorText));
            this.Response.End();
        }

        private static string GetDeviceInfo(string format)
        {
            switch (format)
            {
                case "EXCEL":
                    return "<DeviceInfo><SimplePageHeaders>False</SimplePageHeaders></DeviceInfo>";
                case "IMAGE":
                    return "<DeviceInfo><OutputFormat>PNG</OutputFormat></DeviceInfo>";
                default:
                    return string.Format("<DeviceInfo><OutputFormat>{0}</OutputFormat></DeviceInfo>", format);
            }
        }

        /// <summary>
        /// The get file extensions.
        /// </summary>
        /// <param name="format">
        /// The format.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        [NonAction]
        private string GetFileExtensions(string format)
        {
            switch (format)
            {
                case "EXCEL":
                    return "xls";
                case "IMAGE":
                    return "png";
                case "HTML4.0":
                    return "html";
                case "CSV":
                    return "csv";
                case "PDF":
                    return "pdf";
                case "XML":
                    return "xml";
                default:
                    return format.ToLower();
            }
        }

        /// <summary>
        /// The result report.
        /// </summary>
        /// <typeparam name="TSession">
        /// Session type
        /// </typeparam>
        /// <typeparam name="TResult">
        /// Result type
        /// </typeparam>
        /// <param name="sessionResults">
        /// results by session
        /// </param>
        /// <param name="userModeIdSelector">
        /// session user mode id selector
        /// </param>
        /// <param name="resultConverter">
        /// participant result to report object converter
        /// </param>
        /// <param name="reportName">
        /// report name to search in embedded files
        /// </param>
        /// <param name="outputName">
        /// output file name
        /// </param>
        /// <param name="format">
        /// The format.
        /// </param>
        /// <param name="subReports">
        /// sub report processors [placeholderName]-&gt;[subReportName]-&gt;[dataSourceSetter]
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        [NonAction]
        private ActionResult GetResultReport<TSession, TResult>(
            IDictionary<TSession, TResult> sessionResults,
            Func<TSession, int> userModeIdSelector,
            Func<TSession, IDictionary<int, string>, object> resultConverter,
            string reportName,
            string outputName,
            string format = "PDF",
            IDictionary<string, KeyValuePair<string, SubreportProcessingEventHandler>> subReports = null)
        {
            format = format.ToUpper();
            if (!SupportedReportFormats().Contains(format))
            {
                format = "PDF";
            }

            string mimeType;
            var localReport = new LocalReport { EnableHyperlinks = true };

            string reportPath = string.Format("EdugameCloud.Lti.Host.Areas.Reports.Reports.{0}.rdlc", reportName);
            Stream reportSource = Assembly.GetExecutingAssembly().GetManifestResourceStream(reportPath);
            localReport.LoadReportDefinition(reportSource);

            if (null != subReports)
            {
                foreach (string placeholder in subReports.Keys)
                {
                    string subReportName = string.Format(
                        "EdugameCloud.Lti.Host.Areas.Reports.Reports.{0}.rdlc",
                        subReports[placeholder].Key);

                    Stream subReportSource = Assembly.GetExecutingAssembly().GetManifestResourceStream(subReportName);
                    if (null != subReportSource)
                    {
                        localReport.LoadSubreportDefinition(placeholder, subReportSource);
                    }
                }

                localReport.SubreportProcessing += (sender, args) =>
                {
                    string placeholder = args.ReportPath;

                    if (!string.IsNullOrEmpty(placeholder))
                    {
                        KeyValuePair<string, SubreportProcessingEventHandler> rep;
                        if (subReports.TryGetValue(placeholder, out rep))
                        {
                            rep.Value(sender, args);
                        }
                    }
                    else
                    {
                        var trickReportName = args.Parameters.FirstOrDefault(x => x.Name.StartsWith("trick"));
                        if (trickReportName != null)
                        {
                            string rptName = trickReportName.Name.Substring("trick".Length);

                            KeyValuePair<string, SubreportProcessingEventHandler> rep;
                            if (subReports.TryGetValue(rptName, out rep))
                            {
                                rep.Value(sender, args);
                            }
                        }
                    }
                };
            }

            localReport.DataSources.Clear();

            var userModes =
                this.userModeModel.GetAllByIds(sessionResults.Keys.Select(userModeIdSelector).Distinct().ToList())
                    .ToDictionary(m => m.Id, m => m.UserMode);

            var results = (from s in sessionResults.Keys select resultConverter(s, userModes)).ToList();

            localReport.DataSources.Add(new ReportDataSource("ItemDataSet", results));
            string encoding;
            string fileNameExtension;

            Warning[] warnings;
            string[] streams;
            var renderedBytes = localReport.Render(
                format,
                GetDeviceInfo(format),
                out mimeType,
                out encoding,
                out fileNameExtension,
                out streams,
                out warnings);
            if (renderedBytes != null)
            {
                return this.File(
                    renderedBytes,
                    mimeType,
                    string.Format("{0}.{1}", outputName, this.GetFileExtensions(format)));
            }

            return new HttpStatusCodeResult(HttpStatusCode.NotFound);
        }
        
        private static List<string> SupportedReportFormats()
        {
            return new List<string> { "IMAGE", "PDF", "EXCEL" };
        }

        private IEnumerable<ReportParameter> GetReportParameters(ReportParamsDto paramsDto)
        {
            if (paramsDto.company == null)
                throw new ArgumentNullException(nameof(paramsDto.company));
            if (paramsDto.userSession == null)
                throw new ArgumentNullException(nameof(paramsDto.userSession));
            if (paramsDto.acMeetingUrl == null)
                throw new ArgumentNullException(nameof(paramsDto.acMeetingUrl));
            if (paramsDto.acMeetingTitle == null)
                throw new ArgumentNullException(nameof(paramsDto.acMeetingTitle));

            var companyName = paramsDto.company.CompanyName;
            var companyLogo = this.GetCompanyLogoUrl(paramsDto.company);
            var isExcelFormat = (paramsDto.format.ToUpper() == "EXCEL").ToString();
            var courseName = paramsDto.userSession.LtiSession.LtiParam.context_title ?? string.Empty;

            var acMeetingUrlParam = new ReportParameter("ACMeetingUrl", paramsDto.acMeetingUrl);
            var acMeetingTitleParam = new ReportParameter("ACMeetingTitle", paramsDto.acMeetingTitle);
            var companyNameParam = new ReportParameter("CompanyName", companyName);
            var companyLogoParam = new ReportParameter("CompanyLogo", companyLogo);
            var isExcelParam = new ReportParameter("IsExcelFormat", isExcelFormat);
            var courseNameParam = new ReportParameter("CourseName", courseName);
            var localDateParam = new ReportParameter("LocalDate", String.Format("{0:F}", paramsDto.localDate));
            var isShowMeetingTitle = new ReportParameter("IsShowMeetingTitle", paramsDto.isShowMeetingTitle.ToString());

            var parametersList = new List<ReportParameter>
            {
                companyNameParam,
                companyLogoParam,
                isExcelParam,
                courseNameParam,
                acMeetingUrlParam,
                acMeetingTitleParam,
                localDateParam,
                isShowMeetingTitle
            };
            return parametersList;
        }

        private string GetCompanyLogoUrl(Company company)
        {
            if (company.Theme == null || company.Theme.Logo == null)
            {
                return string.Empty;
            }

            var basePath = this.Settings.BasePath;
            if (string.IsNullOrEmpty(basePath))
            {
                throw new InvalidOperationException("BasePath");
            }

            return basePath + "file/get?id=" + company.Theme.Logo.Id;
        }

        private static string GenerateReportName(LmsUserSession session, string reportName, DateTime localDate)
        {
            var strBiulder = new StringBuilder();

            var courseLabel = session.LtiSession.LtiParam.context_label;
            var courseTitle = session.LtiSession.LtiParam.context_title;

            if (!string.IsNullOrEmpty(courseLabel))
            {
                strBiulder.Append(courseLabel.Replace(" ", "_") + "_");
            }
            else if (string.IsNullOrEmpty(courseLabel) && !string.IsNullOrEmpty(courseTitle))
            {
                strBiulder.Append(courseTitle.Replace(" ", "_") + "_");
            }

            strBiulder.Append(reportName + "_");
            strBiulder.Append(localDate.ToString("MMddyyyy") + "_");
            strBiulder.Append(localDate.ToString("HHmmss"));

            return strBiulder.ToString();
        }

        private byte[] GenerateReportBytes<T>(
            string format,
            string reportName,
            IEnumerable<T> reportDataSource,
            out string mimeType,
            IEnumerable<ReportParameter> reportParameters = null,
            IDictionary<string, KeyValuePair<string, SubreportProcessingEventHandler>> subReports = null)
        {
            var localReport = new LocalReport { EnableHyperlinks = true, EnableExternalImages = true };

            var reportPath = string.Format("EdugameCloud.Lti.Host.Areas.Reports.Reports.{0}.rdlc", reportName);
            var reportSource = Assembly.GetExecutingAssembly().GetManifestResourceStream(reportPath);
            localReport.LoadReportDefinition(reportSource);

            if (subReports != null && subReports.Any())
            {
                foreach (string placeholder in subReports.Keys)
                {
                    var subReportName = string.Format(
                    "EdugameCloud.Lti.Host.Areas.Reports.Reports.SubReports.{0}.rdlc",
                    subReports[placeholder].Key);

                    var subReportSource = Assembly.GetExecutingAssembly().GetManifestResourceStream(subReportName);
                    if (null != subReportSource)
                    {
                        localReport.LoadSubreportDefinition(placeholder, subReportSource);
                    }

                    localReport.SubreportProcessing += subReports[placeholder].Value;
                }
            }

            localReport.DataSources.Clear();
            if (reportParameters != null)
            {
                localReport.SetParameters(reportParameters);
            }

            localReport.DataSources.Add(new ReportDataSource("ItemDataSet", reportDataSource));
            string encoding;
            string fileNameExtension;

            Warning[] warnings;
            string[] streams;
            return localReport.Render(
                format,
                GetDeviceInfo(format),
                out mimeType,
                out encoding,
                out fileNameExtension,
                out streams,
                out warnings);

        }

        //to core
        private DateTime GetLocalDate(TimeZoneInfo tz)
        {
            return TimeZoneInfo.ConvertTime(DateTime.UtcNow, tz);
        }

        private TimeZoneInfo GetTimeZoneInfoForTzdbId(string tzdbId)
        {
            var olsonMappings = TzdbDateTimeZoneSource.Default.WindowsMapping.MapZones;
            var map = olsonMappings.FirstOrDefault(x =>
                x.TzdbIds.Any(z => z.Equals(tzdbId, StringComparison.OrdinalIgnoreCase)));
            return map != null ? FindSystemTimeZoneByIdOrDefault(map.WindowsId) : null;
        }

        private TimeZoneInfo FindSystemTimeZoneByIdOrDefault(string timezoneId)
        {
            try
            {
                return TimeZoneInfo.FindSystemTimeZoneById(timezoneId);
            }
            catch (TimeZoneNotFoundException e)
            {
                logger.Error($"Timezone not found. Id: {timezoneId}", e);
                return null;
            }
        }
        #endregion

    }

}