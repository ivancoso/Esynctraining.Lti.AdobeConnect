﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Xml.Linq;
using EdugameCloud.ACEvents.Web.AcDomainsNamespace;
using EdugameCloud.ACEvents.Web.Certificates;
using EdugameCloud.ACEvents.Web.CompanyEventsServiceNamespace;
using EdugameCloud.ACEvents.Web.FileServiceNamespace;
using EdugameCloud.ACEvents.Web.Models;
using EdugameCloud.ACEvents.Web.QuizResultServiceNamespace;
using EdugameCloud.Certificates.Pdf;
using Esynctraining.AC.Provider;
using Esynctraining.AC.Provider.DataObjects;
using Esynctraining.AdobeConnect;
using Esynctraining.Core.Logging;
using Esynctraining.Core.Utils;

//using EdugameCloud.Certificates.Pdf;

//using SimuLyve.Mbm.Core.BusinessModels;
//using SimuLyve.Mbm.Core.BusinessModels.Pdf;
//using SimuLyve.Mbm.Core.DomailModel;

namespace EdugameCloud.ACEvents.Web.Controllers
{
    public partial class QuizCertificateController : Controller
    {
        private static readonly object _locker = new object();
        private readonly QuizResultService _quizResultService;
        
        private readonly FileService _fileService;
        private readonly CompanyAcDomainsService _companyAcDomainsService;
        private readonly CompanyEventsService _companyEventsService;
        //private readonly FileModel _fileModel;


        public QuizCertificateController(QuizResultService quizResultService, FileService fileService, CompanyAcDomainsService domainsService, CompanyEventsService companyEventsService)
        {
            _companyEventsService = companyEventsService;
            _companyAcDomainsService = domainsService;
            _fileService = fileService;
            
            _quizResultService = quizResultService;
        }


        [OutputCache(CacheProfile = "QuizPreview")]
        public virtual ActionResult Preview(Guid quizResultGuid)
        {
            try
            {
                QuizCertificateInfo quizResult = FindCompletedQuizResult(quizResultGuid);
                if (quizResult == null)
                    return HttpNotFound();

                string imagePath = GetCertProcessor(quizResult).RenderPreview(ImageFormat.Jpeg, BuildTemplateData(quizResult));
                return File(imagePath, "image/jpeg", "CertificatePreview.jpg");

            }
            catch (Exception ex)
            {
                Esynctraining.Core.Utils.IoC.Resolve<ILogger>().Error("Error during Certificate Preview.", ex);
                throw;
            }
        }

        [OutputCache(CacheProfile = "QuizDownload")]
        public virtual ActionResult Download(Guid quizResultGuid)
        {
            try
            {
                QuizCertificateInfo quizResult = FindCompletedQuizResult(quizResultGuid);
                if (quizResult == null)
                    return HttpNotFound();

                if (Request.Browser.IsMobileDevice)
                {
                    string imagePath = GetCertProcessor(quizResult).RenderPreview(ImageFormat.Png, BuildTemplateData(quizResult), resize: false);
                    return File(imagePath, "image/png", "Certificate.png");
                }

                string filePath = GetCertProcessor(quizResult).RenderPdfDocument(BuildTemplateData(quizResult));
                //var contentDisposition = new ContentDisposition()
                //{
                //    FileName = filePath,
                //    Inline = true
                //};
                //Response.Headers.Add("Content-Disposition", contentDisposition.ToString() );
                return new FilePathResultEx(filePath, System.Web.MimeMapping.GetMimeMapping(filePath))
                {
                    FileDownloadName = filePath,
                    Inline = true
                };
            }
            catch (Exception ex)
            {
                Esynctraining.Core.Utils.IoC.Resolve<ILogger>().Error("Error during Certificate Preview.", ex);
                throw;
            }
        }


        private QuizCerfificateProcessor GetCertProcessor(QuizCertificateInfo value)
        {
            string certificateTemplateFilePath = GetCertificatePath(value.CertificateGuid);
            return new QuizCerfificateProcessor(certificateTemplateFilePath, value.CertificateGuid.ToString());
        }

        //private  GetCertProcessor(ProjectCertificateInfo value)
        //{
        //    string certificateTemplateFilePath = GetCertificatePath(value.CertificateTemplateContentId.Value);
        //    return new QuizCerfificateProcessor(certificateTemplateFilePath, value.CertificateTemplateContentId.Value.ToString());
        //}

        private string GetCertificatePath(Guid certificateTemplateContentId)
        {
            string certificateTemplateFilePath = GetPdfTempatePath(certificateTemplateContentId);

            // TODO: think about by-key locking
            if (!System.IO.File.Exists(certificateTemplateFilePath))
            {
                lock (_locker)
                {
                    if (!System.IO.File.Exists(certificateTemplateFilePath))
                    {
                        //var certTemplate = _fileModel.GetOneById(certificateTemplateContentId).Value;
                        //var certTemplate = _fileModel.GetOneById(certificateTemplateContentId).Value;
                        var certTemplate = _fileService.GetById(certificateTemplateContentId.ToString());
                        //var certTemplate = new FileDTO()
                        //{
                        //    fileName = "temp"
                        //};
                        var content = System.IO.File.ReadAllBytes(certTemplate.fileName);
                        System.IO.File.WriteAllBytes(certificateTemplateFilePath, content);
                    }
                }
            }

            return certificateTemplateFilePath;
        }

        private static string GetPdfTempatePath(Guid certificateTemplateContentId)
        {
            string setting = ConfigurationManager.AppSettings["PdfTemplateFolder"];
            string folder = setting.StartsWith("~")
                ? HostingEnvironment.MapPath(setting)
                : setting;

            return Path.Combine(folder,
                string.Format("{0}.pdf", certificateTemplateContentId.ToString()));
        }

        private QuizCertificateInfo FindCompletedQuizResult(Guid quizResultGuid)
        {
            //if ((projectId <= 0) || (token == Guid.Empty))
            //    return null;

            //AccountSession session = _sessionModel.GetParticipantByToken(token);
            //if (session == null)
            //    return null;

            //return _projectModel.FindInfoByProjectUser(projectId, session.AccountId);

            var quizResult = _quizResultService.GetByGuid(quizResultGuid.ToString());
            var eventMapping = _companyEventsService.GetById(quizResult.eventQuizMappingId ?? 0, true);
            var acDomain = _companyAcDomainsService.GetById(eventMapping.companyAcDomainId, true);
            var acUrl = acDomain.path;
            var apiUrl = new Uri(acUrl);
            var logger = IoC.Resolve<ILogger>();

            var scoId = eventMapping.acEventScoId;
            var proxy = new AdobeConnectProxy(new AdobeConnectProvider(new ConnectionDetails(apiUrl)), logger, apiUrl);
            var loginResult = proxy.Login(new UserCredentials(acDomain.user, acDomain.password));
            if (!loginResult.Success)
                throw new InvalidOperationException("Can't login to AC");
            //var additionalFields = proxy.GetEventRegistrationDetails(scoId);
            //var userState = string.Empty;

            var userStateSchoolAnswersUrl =
                $"{acUrl}/api/xml?action=report-event-participants-complete-information&sco-id={scoId}&session={loginResult.Status.SessionInfo}";
           
            var reply = string.Empty;

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/xml"));
                var request = new HttpRequestMessage(HttpMethod.Get, userStateSchoolAnswersUrl);
                reply = client.SendAsync(request).Result.Content.ReadAsStringAsync().Result;
            }

            var doc = XDocument.Parse(reply);
            var questions = doc.Root?.Descendants("registration_questions").Descendants("question").ToList();
            var stateNode = questions?.FirstOrDefault(x => x.Attribute("description").Value.ToString().ToLower().Equals("state", StringComparison.OrdinalIgnoreCase));
            var schoolNode = questions?.FirstOrDefault(x => x.Attribute("description").Value.ToString().ToLower().Equals("school", StringComparison.OrdinalIgnoreCase));

            var stateQuestionId = stateNode?.Attribute("id")?.Value.ToString() ?? string.Empty;
            var schoolQuestionId = schoolNode?.Attribute("id")?.Value.ToString() ?? string.Empty;
            //foreach (var eventRegistrationDetail in additionalFields.EventFields)
            //{
            //    if (string.Equals(eventRegistrationDetail.Description, "state", StringComparison.OrdinalIgnoreCase))
            //        userState = additionalFields.UserFields.ToList().Where(x => x.Name == )

            //}

            var userAnswers = doc.Root?.Descendants("user_list").Descendants("user").ToList();
            var userAnswer = userAnswers?.FirstOrDefault(x => x.Attribute("login").Value.ToString().ToLower().Equals(quizResult.acEmail, StringComparison.OrdinalIgnoreCase));
            var state = userAnswer?.Attribute("registration_question_" + stateQuestionId)?.Value ?? String.Empty;
            var school = userAnswer?.Attribute("registration_question_" + schoolQuestionId)?.Value ?? String.Empty;

            var userEmail = quizResult.acEmail;
            //var userInfo = proxy.GetPrincipalInfo(userEmail);
            var participantName = quizResult.participantName;
            //var participantName = $"{userInfo.PrincipalInfo.Principal.Name}";

            
            var eventScoInfo = proxy.GetScoInfo(scoId);
            var teacher = proxy.GetScoByUrl2(eventScoInfo.ScoInfo.UrlPath).ScoInfo.Owner;
            var teacherName = teacher.Name;

            var hours = (eventScoInfo.ScoInfo.EndDate - eventScoInfo.ScoInfo.BeginDate).Hours;
            var hoursEnding = hours > 1 ? "s" : string.Empty;
            var quizCertificateInfo = new QuizCertificateInfo()
            {
                ParticipantName = participantName,
                KnowledgeArea = "Core Knowledge Area",
                State = stateQuestionId,
                Duration = $"{hours} clock hour{hoursEnding}",
                CourseTitle = eventScoInfo.ScoInfo.Name,
                TrainerName = teacherName,
                Date = UnixTimeStampToDateTime(quizResult.dateCreated).ToString("MMMM dd, yyyy")
            };
            quizCertificateInfo.CertificateGuid = CertTemplateBuilder.GetTemplateGuid(state);

            return quizCertificateInfo;
        }

        private static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            CultureInfo en = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentCulture = en;
            // Unix timestamp is seconds past epoch
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp/1000).ToLocalTime();
            return dtDateTime;
        }


        private static Dictionary<string, string> BuildTemplateData(QuizCertificateInfo certInfo)
        {
            var fields = new Dictionary<string, string>
                {
                    { "ParticipantName", certInfo.ParticipantName },
                    { "CourseTitle", certInfo.CourseTitle },
                    { "CtoNumber", certInfo.CtoNumber },
                    { "Duration", certInfo.Duration },
                    { "Date",  certInfo.Date },
                    { "ExpiresDate",  certInfo.ExpiresDate },
                    { "KnowledgeArea",  certInfo.KnowledgeArea },
                    { "Signature",  certInfo.Signature },
                    { "SpecificId",  certInfo.SpecificId },
                    { "TrainerName",  certInfo.TrainerName }
                };

            return fields;
        }

    }
}