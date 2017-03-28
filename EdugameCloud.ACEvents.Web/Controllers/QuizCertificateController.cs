using System;
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
using EdugameCloud.ACEvents.Web.QuizServiceNamespace;
using EdugameCloud.Certificates.Pdf;
using Esynctraining.AC.Provider;
using Esynctraining.AC.Provider.DataObjects;
using Esynctraining.AC.Provider.Entities;
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
        private readonly QuizService _quizService;
        //private readonly FileModel _fileModel;


        public QuizCertificateController(QuizResultService quizResultService, FileService fileService, CompanyAcDomainsService domainsService, 
            CompanyEventsService companyEventsService, QuizService quizService)
        {
            _quizService = quizService;
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
                    return File(imagePath, "image/png", $"Certificate_{quizResult.ParticipantName}.png");
                }

                string filePath = GetCertProcessor(quizResult).RenderPdfDocument(BuildTemplateData(quizResult));
                //var contentDisposition = new ContentDisposition()
                //{
                //    FileName = filePath,
                //    Inline = true
                //};
                //Response.Headers.Add("Content-Disposition", contentDisposition.ToString() );
                return File(filePath, "application/pdf", $"Certificate_{quizResult.ParticipantName}.pdf");
                //return new FilePathResultEx(filePath, System.Web.MimeMapping.GetMimeMapping(filePath))
                //{
                //    FileDownloadName = filePath,
                //    Inline = true
                //};
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

            //if (eventMapping.postQuizId != quizResult.quizId)
            //{
            //    // it should be postQuiz result (that is the same as in mapping)
            //    return null;
            //}

            var quiz = _quizService.GetById(eventMapping.postQuizId, true);
            if (!quiz.isPostQuiz)
                return null;
            var quizPassingScoreInPercents = (float)quiz.passingScore / 100;
            var quizData = _quizService.GetQuizDataByQuizId(eventMapping.postQuizId, true);
            var totalQuestions = quizData.questions.Length;
            var scoreInPercents = (float)quizResult.score / totalQuestions;
            var isSuccess = scoreInPercents >= quizPassingScoreInPercents;
            if (!isSuccess)
                return null;

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
            
            var userAnswers = doc.Root?.Descendants("user_list").Descendants("user").ToList();
            var userAnswer = userAnswers?.FirstOrDefault(x => x.Attribute("login").Value.ToString().ToLower().Equals(quizResult.acEmail, StringComparison.OrdinalIgnoreCase));
            var state = userAnswer?.Attribute("registration_question_" + stateQuestionId)?.Value ?? String.Empty;
            var school = userAnswer?.Attribute("registration_question_" + schoolQuestionId)?.Value ?? String.Empty;

            var userEmail = quizResult.acEmail;
            //var userInfo = proxy.GetPrincipalInfo(userEmail);
            var participantName = quizResult.participantName;
            //var participantName = $"{userInfo.PrincipalInfo.Principal.Name}";

            var eventScoInfo = proxy.GetScoInfo(scoId);
            var eventParticipants = proxy.GetAllMeetingEnrollments(scoId);

            var teacherName = String.Empty;
            if (eventParticipants.Success)
            {
                var host = eventParticipants.Values.FirstOrDefault(x => x.PermissionId == MeetingPermissionId.host);
                if (host != null)
                {
                    teacherName = host.Name;
                }
            }
            else
            {
                var teacher = proxy.GetScoByUrl2(eventScoInfo.ScoInfo.UrlPath).ScoInfo.Owner;
                teacherName = teacher.Name;
            }

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
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp / 1000).ToLocalTime();
            return dtDateTime;
        }


        private static Dictionary<string, string> BuildTemplateData(QuizCertificateInfo certInfo)
        {
            var fields = new Dictionary<string, string>
                {
                    { "ParticipantName", AlignText(certInfo.ParticipantName, 25) },//hardcoded values in pdf template (length of custom fields)
                    { "CourseTitle", AlignText(certInfo.CourseTitle, 29) },//hardcoded values in pdf template (length of custom fields)
                    { "CtoNumber", certInfo.CtoNumber },
                    { "Duration", AlignText(certInfo.Duration, 18) },//hardcoded values in pdf template (length of custom fields)
                    { "Date",  certInfo.Date },
                    { "ExpiresDate",  certInfo.ExpiresDate },
                    { "KnowledgeArea",  certInfo.KnowledgeArea },
                    { "Signature",  certInfo.Signature },
                    { "SpecificId",  certInfo.SpecificId },
                    { "TrainerName",  certInfo.TrainerName }
                };

            return fields;
        }

        private static string AlignText(string value, int fieldLength)
        {
            var fieldValueLength = value.ToCharArray().Length;
            var difference = fieldLength - fieldValueLength;
            var offset = fieldLength > fieldValueLength ? difference / 2 : 0;
            if (offset >= 4 && offset < 8)
                offset = (int)(offset * 1.3);
            if (offset >= 8)
                offset = (int)(offset * 1.6);
            value = fieldValueLength > fieldLength ? value.Take(fieldLength).ToString() : value;
            var result = new String(' ', offset) + value + (offset % 2 > 0 ? new String(' ', offset + 1) : new String(' ', offset));
            return result;
        }

    }
}