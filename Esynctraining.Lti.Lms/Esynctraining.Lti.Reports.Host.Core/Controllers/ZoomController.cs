using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using Esynctraining.Lti.Reports.Host.Core.Models;
using Microsoft.Reporting.WebForms;

namespace Esynctraining.Lti.Reports.Host.Core.Controllers
{
    public class ZoomController : ControllerBase
    {
        [HttpGet]
        public ActionResult GetReportBySession()
        {
            var dto = new SessionReportDto
            {
                MeetingUrl = "MeetingUrl",
                MeetingTitle = "MeetingTitle",
                CompanyName = "CompanyName",
                CompanyLogo = "",
                IsExcelFormat = true,
                CourseName = "CourseName",
                LocalDate = DateTime.Now.ToString(),
                IsShowMeetingTitle = true,
                Sessions = new List<SessionDto> {new SessionDto
                {
                    DateStarted = "2018-05-24",
                    DateEnded = "2018-05-24 05:00",
                   MeetingId = "123",
                    SessionId = "1",
                    ParticipantsCount = 2,
                    Participants = new List<SessionParticipantDto>{new SessionParticipantDto
                    {
                        ParticipantName = "ParticipantName",
                        DateTimeEntered = "EnteredAt",
                        DateTimeLeft = "LeftAt"
                    }}
                }}
            };
            try
            {
                //var tz = GetTimeZoneInfoForTzdbId(timezone) ?? TimeZoneInfo.Utc;
                //LmsCourseMeeting meeting = lmsCourseMeetingModel.GetOneByCourseAndId(credentials.Id, s.LmsCourseId, meetingId);

                //var acMeeting = acProvider.GetScoInfo(meeting.GetMeetingScoId());
                //var acMeetingUrl = credentials.AcServer + acMeeting.ScoInfo.UrlPath;
                //var acMeetingTitle = acMeeting.ScoInfo.Name;

                //var tempMeetingSessions = this.meetingSetup.GetSessionsReports(
                //    meeting.GetMeetingScoId(),
                //    acProvider,
                //    tz,
                //    startIndex,
                //    limit);

                //var meetingSessions = new List<ACSessionReportDto>();

                //if (tempMeetingSessions.Any())
                //{
                //    meetingSessions = tempMeetingSessions.Select(x => new ACSessionReportDto(x, tz)).ToList();
                //}

                //if (format.ToUpper() != "PDF" && format.ToUpper() != "EXCEL")
                //{
                //    this.RedirectToError("Unable to generate report in such format " + " \"" + format + "\"");
                //    return null;
                //}

                //var company = this.companyModel.GetOneById(s.LmsCompany.CompanyId).Value;

                //if (company == null)
                //{
                //    this.RedirectToError("Unable to retrieve data about company");
                //    return null;
                //}
                var localDate = DateTime.Now;
                var format = dto.IsExcelFormat ? "EXCEL" : "PDF";
                //bool.TryParse(credentials.GetSetting<string>(LmsCompanySettingNames.IsPdfMeetingUrl), out isShowMeetingTitle);
                var parametersList = GetReportParameters(dto);
                var subreports = new Dictionary<string, KeyValuePair<string, SubreportProcessingEventHandler>>
                {
                    {
                        "ParticipantsSubReport",
                        new KeyValuePair<string, SubreportProcessingEventHandler>("ParticipantsSubReport",
                            (sender, args) =>
                            {
                                var sessionId = args.Parameters[0].Values[0];
                                args.DataSources.Clear();

                                var sessions = (IEnumerable<SessionDto>)(((LocalReport) sender).DataSources["ItemDataSet"].Value);
                                var participants = new List<SessionParticipantDto>();
                                if (sessions.Any())
                                {
                                    var tempSession = sessions.FirstOrDefault(x => x.SessionId == sessionId);
                                    if (tempSession != null && tempSession.Participants != null && tempSession.Participants.Any())
                                    {
                                        participants = tempSession.Participants;
                                    }
                                }
                                args.DataSources.Add(new ReportDataSource("ItemDataSet", participants));
                            })
                    }
                };

                string mimeType;
                var reportRenderedBytes = this.GenerateReportBytes(format, "MeetingSessionsReport", dto.Sessions,
                    out mimeType, parametersList, subreports);

                if (reportRenderedBytes != null)
                {
                    var reportName = "SessionsReport_" + localDate.ToString("MMddyyyy") + "_" +
                                     localDate.ToString("HHmmss");//GenerateReportName(s, "Sessions", localDate);

                    return this.File(
                        reportRenderedBytes,
                        mimeType,
                        string.Format("{0}.{1}", reportName, this.GetFileExtensions(format.ToUpper())));
                }

                this.RedirectToError("Unable to generate report. Try again later.");
            }

            catch (Exception ex)
            {
                //this.logger.Error("MeetingSessionsReport", ex);
                this.RedirectToError("Unable to generate report. Try again later.");
            }

            return null;
        }
        // GET: Zoom
        [HttpPost]
        public ActionResult ReportBySession(SessionReportDto dto)
        {
            try
            {
                //var tz = GetTimeZoneInfoForTzdbId(timezone) ?? TimeZoneInfo.Utc;
                //LmsCourseMeeting meeting = lmsCourseMeetingModel.GetOneByCourseAndId(credentials.Id, s.LmsCourseId, meetingId);

                //var acMeeting = acProvider.GetScoInfo(meeting.GetMeetingScoId());
                //var acMeetingUrl = credentials.AcServer + acMeeting.ScoInfo.UrlPath;
                //var acMeetingTitle = acMeeting.ScoInfo.Name;

                //var tempMeetingSessions = this.meetingSetup.GetSessionsReports(
                //    meeting.GetMeetingScoId(),
                //    acProvider,
                //    tz,
                //    startIndex,
                //    limit);

                //var meetingSessions = new List<ACSessionReportDto>();

                //if (tempMeetingSessions.Any())
                //{
                //    meetingSessions = tempMeetingSessions.Select(x => new ACSessionReportDto(x, tz)).ToList();
                //}

                //if (format.ToUpper() != "PDF" && format.ToUpper() != "EXCEL")
                //{
                //    this.RedirectToError("Unable to generate report in such format " + " \"" + format + "\"");
                //    return null;
                //}

                //var company = this.companyModel.GetOneById(s.LmsCompany.CompanyId).Value;

                //if (company == null)
                //{
                //    this.RedirectToError("Unable to retrieve data about company");
                //    return null;
                //}
                var localDate = DateTime.Now;
                var format = dto.IsExcelFormat ? "EXCEL" : "PDF";
                //bool.TryParse(credentials.GetSetting<string>(LmsCompanySettingNames.IsPdfMeetingUrl), out isShowMeetingTitle);
                var parametersList = GetReportParameters(dto);
                var subreports = new Dictionary<string, KeyValuePair<string, SubreportProcessingEventHandler>>
                {
                    {
                        "ParticipantsSubReport",
                        new KeyValuePair<string, SubreportProcessingEventHandler>("ParticipantsSubReport",
                            (sender, args) =>
                            {
                                var sessionId = args.Parameters[0].Values[0];
                                args.DataSources.Clear();

                                var sessions = (IEnumerable<SessionDto>)(((LocalReport) sender).DataSources["ItemDataSet"].Value);
                                var participants = new List<SessionParticipantDto>();
                                if (sessions.Any())
                                {
                                    var tempSession = sessions.FirstOrDefault(x => x.SessionId == sessionId);
                                    if (tempSession != null && tempSession.Participants != null && tempSession.Participants.Any())
                                    {
                                        participants = tempSession.Participants;
                                    }
                                }
                                args.DataSources.Add(new ReportDataSource("ItemDataSet", participants));
                            })
                    }
                };

                string mimeType;
                var reportRenderedBytes = this.GenerateReportBytes(format, "MeetingSessionsReport", dto.Sessions,
                    out mimeType, parametersList, subreports);

                if (reportRenderedBytes != null)
                {
                    var reportName = "SessionsReport_" + localDate.ToString("MMddyyyy") + "_" +
                                     localDate.ToString("HHmmss");//GenerateReportName(s, "Sessions", localDate);

                    return this.File(
                        reportRenderedBytes,
                        mimeType,
                        string.Format("{0}.{1}", reportName, this.GetFileExtensions(format.ToUpper())));
                }

                this.RedirectToError("Unable to generate report. Try again later.");
            }

            catch (Exception ex)
            {
                //this.logger.Error("MeetingSessionsReport", ex);
                this.RedirectToError("Unable to generate report. Try again later.");
            }

            return null;
        }

        private IEnumerable<ReportParameter> GetReportParameters(SessionReportDto paramsDto)
        {
            var acMeetingUrlParam = new ReportParameter("MeetingUrl", paramsDto.MeetingUrl);
            var acMeetingTitleParam = new ReportParameter("MeetingTitle", paramsDto.MeetingTitle);
            var companyNameParam = new ReportParameter("CompanyName", paramsDto.CompanyName);
            //var companyLogoParam = new ReportParameter("CompanyLogo", paramsDto.CompanyLogo);
            var isExcelParam = new ReportParameter("IsExcelFormat", paramsDto.IsExcelFormat.ToString());
            var courseNameParam = new ReportParameter("CourseName", paramsDto.CourseName);
            var localDateParam = new ReportParameter("LocalDate", paramsDto.LocalDate);
            var isShowMeetingTitle = new ReportParameter("IsShowMeetingTitle", paramsDto.IsShowMeetingTitle.ToString());

            var parametersList = new List<ReportParameter>
            {
                companyNameParam,
                //companyLogoParam,
                isExcelParam,
                courseNameParam,
                acMeetingUrlParam,
                acMeetingTitleParam,
                localDateParam,
                isShowMeetingTitle
            };
            return parametersList;
        }

        //private static string GenerateReportName(LmsUserSession session, string reportName, DateTime localDate)
        //{
        //    var strBiulder = new StringBuilder();

        //    var courseLabel = session.LtiSession.LtiParam.context_label;
        //    var courseTitle = session.LtiSession.LtiParam.context_title;

        //    if (!string.IsNullOrEmpty(courseLabel))
        //    {
        //        strBiulder.Append(courseLabel.Replace(" ", "_") + "_");
        //    }
        //    else if (string.IsNullOrEmpty(courseLabel) && !string.IsNullOrEmpty(courseTitle))
        //    {
        //        strBiulder.Append(courseTitle.Replace(" ", "_") + "_");
        //    }

        //    strBiulder.Append(reportName + "_");
        //    strBiulder.Append(localDate.ToString("MMddyyyy") + "_");
        //    strBiulder.Append(localDate.ToString("HHmmss"));

        //    return strBiulder.ToString();
        //}

        private byte[] GenerateReportBytes<T>(
            string format,
            string reportName,
            IEnumerable<T> reportDataSource,
            out string mimeType,
            IEnumerable<ReportParameter> reportParameters = null,
            IDictionary<string, KeyValuePair<string, SubreportProcessingEventHandler>> subReports = null)
        {
            var localReport = new LocalReport { EnableHyperlinks = true, EnableExternalImages = true };

            var reportPath = string.Format("Esynctraining.Lti.Reports.Host.Core.Reports.Zoom.{0}.rdlc", reportName);
            var reportSource = Assembly.GetExecutingAssembly().GetManifestResourceStream(reportPath);
            localReport.LoadReportDefinition(reportSource);

            if (subReports != null && subReports.Any())
            {
                foreach (string placeholder in subReports.Keys)
                {
                    var subReportName = string.Format(
                    "Esynctraining.Lti.Reports.Host.Core.Reports.Zoom.Subreports.{0}.rdlc",
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

        private void RedirectToError(string errorText)
        {
            Response.StatusCode = 500;
            this.Response.Clear();
            //this.Response.Write(string.Format("<h1>{0}</h1>", errorText));
            //this.Response.End();
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
    }
}
