using EdugameCloud.Lti.Core.Business.Models;
using EdugameCloud.Lti.Core.DTO;

namespace EdugameCloud.MVC.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Reflection;
    using System.Web.Mvc;
    using System.Web.UI;
    using EdugameCloud.MVC.Attributes;
    using Microsoft.Reporting.WebForms;
    using NHibernate.Util;

    [HandleError]
    public partial class FileController : BaseController
    {
        #region Public Methods and Operators
        
        [HttpGet]
        [OutputCache(Duration = 0, NoStore = true, Location = OutputCacheLocation.None)]
        [ActionName("meeting-host-report")]
        [CustomAuthorize]
        public virtual ActionResult MeetingHostReport(int lmsCompanyId, string format = "PDF")
        {
            var license = this.lmsCompanyModel.GetOneById(lmsCompanyId).Value;

            if (license == null)
            {
               return this.HttpNotFound();
            }

            var lmsCompanyName = LmsProviderModel.GetById(license.LmsProviderId).LmsProviderName;

            var provider = adobeConnectAccountService.GetProvider(license);

            var meetingHosts =  adobeConnectAccountService.GetMeetingHostReport(provider).ToArray();

            var results = new List<MeetingHostReportItemDTO>();

            if (meetingHosts.Any())
            {
                results = meetingHosts.Select(x => new MeetingHostReportItemDTO(x)).ToList().OrderByDescending(x => x.LastMeetingAttend).ToList();
            }

            format = format.ToUpper();
            if (!this.SupportedReportFormats().Contains(format))
            {
                format = "PDF";
            }

            string mimeType;
            var localReport = new LocalReport { EnableHyperlinks = true };

            var reportPath = string.Format("EdugameCloud.MVC.Reports.{0}.rdlc", "MeetingHostReport");
            var reportSource = Assembly.GetExecutingAssembly().GetManifestResourceStream(reportPath);
            localReport.LoadReportDefinition(reportSource);

            localReport.DataSources.Clear();

            var reportParameters = new ReportParameterCollection
            {
                new ReportParameter("AccountUrl", license.AcServer),
                new ReportParameter("LmsCompanyName", lmsCompanyName)
            };
            localReport.SetParameters(reportParameters);

            localReport.DataSources.Add(new ReportDataSource("ItemDataSet", new MeetingHostReportDto(results)));
            string encoding;
            string fileNameExtension;

            Warning[] warnings;
            string[] streams;
            var renderedBytes = localReport.Render(
                format,
                this.GetDeviceInfo(format),
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
                    string.Format("{0}.{1}", "meeting-host-report", this.GetFileExtensions(format)));
            }

            return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            
        }
        
        #endregion

        #region Methods
        
        /// <summary>
        /// The get device info.
        /// </summary>
        /// <param name="format">
        /// The format.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        [NonAction]
        private string GetDeviceInfo(string format)
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
            if (!this.SupportedReportFormats().Contains(format))
            {
                format = "PDF";
            }

            string mimeType;
            var localReport = new LocalReport { EnableHyperlinks = true };

            string reportPath = string.Format("EdugameCloud.MVC.Reports.{0}.rdlc", reportName);
            Stream reportSource = Assembly.GetExecutingAssembly().GetManifestResourceStream(reportPath);
            localReport.LoadReportDefinition(reportSource);

            if (null != subReports)
            {
                foreach (string placeholder in subReports.Keys)
                {
                    string subReportName = string.Format(
                        "EdugameCloud.MVC.Reports.{0}.rdlc", 
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
                this.GetDeviceInfo(format), 
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
        
        private List<string> SupportedReportFormats()
        {
            return new List<string> { "IMAGE", "PDF", "EXCEL" };
        }
        
        #endregion

    }

}