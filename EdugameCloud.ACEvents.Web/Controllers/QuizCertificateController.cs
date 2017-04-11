﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using CompanyAcDomainsNamespace;
using CompanyEventsServiceNamespace;

using EdugameCloud.ACEvents.Web.Certificates;

using EdugameCloud.Certificates.Pdf;
using Esynctraining.AC.Provider;
using Esynctraining.AC.Provider.DataObjects;
using Esynctraining.AC.Provider.DataObjects.Results;
using Esynctraining.AC.Provider.Entities;
using Esynctraining.AdobeConnect;
using Esynctraining.Core.Utils;
using FileServiceNamespace;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using QuizResultServiceNamespace;
using QuizServiceNamespace;
using ILogger = Esynctraining.Core.Logging.ILogger;

namespace EdugameCloud.ACEvents.Web.Controllers
{
    public class QuizCertificateController : Controller
    {
        private static readonly object _locker = new object();
        private readonly IQuizResultService _quizResultService;

        private readonly IFileService _fileService;
        private readonly ICompanyAcDomainsService _companyAcDomainsService;
        private readonly ICompanyEventsService _companyEventsService;
        private readonly IQuizService _quizService;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly Microsoft.Extensions.Logging.ILogger _logger;
        private readonly IAdobeConnectAccountService _adobeConnectAccountService;
        private readonly AppSettings _appSettings;
        private readonly IQuizCerfificateProcessor _quizCerfificateProcessor;
        private readonly IGoddardApiConsumer _goddardApiConsumer;

        public QuizCertificateController(IQuizResultService quizResultService, IFileService fileService, ICompanyAcDomainsService domainsService,
            ICompanyEventsService companyEventsService, IQuizService quizService, IHostingEnvironment hostingEnvironment, IHttpContextAccessor context,
            IAdobeConnectAccountService acProvider, ILoggerFactory loggerFactory, IOptionsSnapshot<AppSettings> appSettings, IQuizCerfificateProcessor quizCerfificateProcessor, IGoddardApiConsumer goddardApiConsumer)
        {
            _goddardApiConsumer = goddardApiConsumer;
            _quizCerfificateProcessor = quizCerfificateProcessor;
            _appSettings = appSettings.Value;
            _adobeConnectAccountService = acProvider;
            _httpContextAccessor = context;
            _hostingEnvironment = hostingEnvironment;
            _quizService = quizService;
            _companyEventsService = companyEventsService;
            _companyAcDomainsService = domainsService;
            _fileService = fileService;
            _quizResultService = quizResultService;
            _logger = loggerFactory.CreateLogger<QuizCerfificateProcessor>();
        }


        //[OutputCache(CacheProfile = "QuizPreview")]
        public virtual ActionResult Preview(Guid quizResultGuid)
        {
            try
            {
                QuizCertificateInfo quizResult = FindCompletedQuizResult(quizResultGuid);
                if (quizResult == null)
                    return NotFound();

                //var certTemplatePath = GetCertificatePath(quizResult.CertificateTemplateGuid);
                string imagePath = _quizCerfificateProcessor.RenderPreview(quizResultGuid.ToString(), quizResult.CertificateTemplateGuid.ToString(), ImageFormat.Jpeg, BuildTemplateData(quizResult));
                return PhysicalFile(imagePath, "image/jpeg", "CertificatePreview.jpg");

            }
            catch (Exception ex)
            {
                _logger.LogError("Error during Certificate Preview.", ex);
                throw;
            }
        }

        //[OutputCache(CacheProfile = "QuizDownload")]
        public virtual ActionResult Download(Guid quizResultGuid)
        {
            try
            {
                QuizCertificateInfo quizResult = FindCompletedQuizResult(quizResultGuid);
                if (quizResult == null)
                    return NotFound();

                //string certificateTemplateFilePath = GetCertificatePath(quizResult.CertificateTemplateGuid);
                var templateUid = quizResult.CertificateTemplateGuid.ToString();
                if (_httpContextAccessor.HttpContext.Request.IsMobileBrowser())
                {

                    string imagePath = _quizCerfificateProcessor.RenderPreview(quizResultGuid.ToString(), templateUid, ImageFormat.Png, BuildTemplateData(quizResult), resize: false);
                    return PhysicalFile(imagePath, "image/png", $"Certificate_{quizResult.ParticipantName}.png");
                }

                string filePath = _quizCerfificateProcessor.RenderPdfDocument(quizResultGuid.ToString(), templateUid, BuildTemplateData(quizResult));
                //var contentDisposition = new ContentDisposition()
                //{
                //    FileName = filePath,
                //    Inline = true
                //};
                //Response.Headers.Add("Content-Disposition", contentDisposition.ToString() );
                return PhysicalFile(filePath, "application/pdf", $"Certificate_{quizResult.ParticipantName}.pdf");
               
            }
            catch (Exception ex)
            {
                _logger.LogError("Error during Certificate Preview.", ex);
                throw;
            }
        }

        private QuizCertificateInfo FindCompletedQuizResult(Guid quizResultGuid)
        {
            var quizResult = _quizResultService.GetByGuidAsync(quizResultGuid).Result;
            var eventMapping = _companyEventsService.GetByIdAsync(quizResult.eventQuizMappingId ?? 0).Result;
            var acDomain = _companyAcDomainsService.GetByIdAsync(eventMapping.companyAcDomainId).Result;
            var acUrl = acDomain.path;
            var login = acDomain.user;
            var pass = acDomain.password;
            var apiUrl = new Uri(acUrl);

            var scoId = eventMapping.acEventScoId;
            var proxy = _adobeConnectAccountService.GetProvider(new AdobeConnectAccess(apiUrl, login, pass), false);

            var loginResult = proxy.Login(new UserCredentials(login, pass));
            if (!loginResult.Success)
                throw new InvalidOperationException("Can't login to AC");

            //if (eventMapping.postQuizId != quizResult.quizId)
            //{
            //    // it should be postQuiz result (that is the same as in mapping)
            //    return null;
            //}

            var quiz = _quizService.GetByIdAsync(eventMapping.postQuizId).Result;
            if (!quiz.isPostQuiz)
                return null;
            var quizPassingScoreInPercents = (float) quiz.passingScore / 100;
            var quizData = _quizService.GetQuizDataByQuizIdAsync(eventMapping.postQuizId).Result;
            var totalQuestions = quizData.questions.Length;
            var scoreInPercents = (float) quizResult.score / totalQuestions;
            var isSuccess = scoreInPercents >= quizPassingScoreInPercents;
            if (!isSuccess)
                return null;

            var dynamicQuestionAnswers = GetDynamicQuestionAnswers(acUrl, scoId, loginResult.Status.SessionInfo,
                quizResult.acEmail);
            var state = dynamicQuestionAnswers["state"];
            //var school = dynamicQuestionAnswers["school"];

            //var userEmail = quizResult.acEmail;
            //var userInfo = proxy.GetPrincipalInfo(userEmail);
            var participantName = quizResult.participantName;
            //var participantName = $"{userInfo.PrincipalInfo.Principal.Name}";

            var eventScoInfo = proxy.GetScoInfo(scoId);

            var trainerId = GetTeacherId(acUrl, scoId, loginResult.Status.SessionInfo);
            var trainer = _goddardApiConsumer.GetTrainer(trainerId);
            var teacherName = $"{trainer.FirstName} {trainer.LastName}";

            var courseId = GetCourseId(acUrl, scoId, loginResult.Status.SessionInfo);
            var course = _goddardApiConsumer.GetCourse(courseId)?.CourseTitle ?? eventScoInfo.ScoInfo.Name;

            var coreKnowledgeArea = _goddardApiConsumer.GetCoreKnowledgeArea(courseId, state)?.Name ?? String.Empty;
            var encodedEventName = WebUtility.UrlEncode(WebUtility.UrlEncode(eventScoInfo.ScoInfo.Name));//AA: we need double encode for slash symbol (at least it didn't work with one encoding when I tried)
            var eventApprovalCode = _goddardApiConsumer.GetEventStateCourseNumber(encodedEventName, state)?.Name ?? String.Empty;
            var stateTrainerNumber = _goddardApiConsumer.GetStateTrainerNumber(trainerId, state)?.Label ?? String.Empty;
            var stateTrainerCourseNumber = _goddardApiConsumer.GetStateTrainerCourseNumber(courseId, state, trainerId)?.CourseTitle ?? String.Empty;

            var hours = (eventScoInfo.ScoInfo.EndDate - eventScoInfo.ScoInfo.BeginDate).Hours;
            var hoursEnding = hours > 1 ? "s" : string.Empty;
            var quizCertificateInfo = new QuizCertificateInfo()
            {
                ParticipantName = participantName,
                KnowledgeArea = coreKnowledgeArea,
                EventApprovalCode = eventApprovalCode,
                StateTrainerNumber = stateTrainerNumber,
                StateTrainerCourseNumber = stateTrainerCourseNumber,
                //State = stateQuestionId,
                Duration = $"{hours} clock hour{hoursEnding}",
                CourseTitle = course,
                TrainerName = teacherName,
                Date = UnixTimeStampToDateTime(quizResult.dateCreated).ToString("MMMM dd, yyyy")
            };
            quizCertificateInfo.CertificateTemplateGuid = CertTemplateBuilder.GetTemplateGuid(state);

            return quizCertificateInfo;
        }

        private string GetTeacherId(string acUrl, string scoId, string breezeSession)
        {
            var eventInfoUrl = $"{acUrl}/api/xml?action=event-info&sco-id={scoId}&session={breezeSession}";
            string reply;
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/xml"));
                var request = new HttpRequestMessage(HttpMethod.Get, eventInfoUrl);
                reply = client.SendAsync(request).Result.Content.ReadAsStringAsync().Result;
            }
            var doc = XDocument.Parse(reply);
            var speaker = doc.Root?.Descendants("event-info").FirstOrDefault()?.Descendants("speaker-name").FirstOrDefault()?.Value;

            if (string.IsNullOrEmpty(speaker))
            {
                throw new InvalidOperationException($"Speaker should be set for the event with sco-id {scoId}");
            }

            return speaker;
        }

        private string GetCourseId(string acUrl, string scoId, string breezeSession)
        {
            var eventInfoUrl = $"{acUrl}/api/xml?action=event-info&sco-id={scoId}&session={breezeSession}";
            string reply;
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/xml"));
                var request = new HttpRequestMessage(HttpMethod.Get, eventInfoUrl);
                reply = client.SendAsync(request).Result.Content.ReadAsStringAsync().Result;
            }
            var doc = XDocument.Parse(reply);
            var speaker = doc.Root?.Descendants("event-info").FirstOrDefault()?.Descendants("event-info").FirstOrDefault()?.Value;
            if (string.IsNullOrEmpty(speaker))
            {
                throw new InvalidOperationException($"Speaker should be set for the event with sco-id {scoId}");
            }
            speaker = speaker.Trim();

            return speaker;
        }

        private static Dictionary<string, string> GetDynamicQuestionAnswers(string acUrl, string scoId, string breezeSession, string userEmail)
        {
            var userStateSchoolAnswersUrl =
                $"{acUrl}/api/xml?action=report-event-participants-complete-information&sco-id={scoId}&session={breezeSession}";

            var reply = string.Empty;

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/xml"));
                var request = new HttpRequestMessage(HttpMethod.Get, userStateSchoolAnswersUrl);
                reply = client.SendAsync(request).Result.Content.ReadAsStringAsync().Result;
            }

            var doc = XDocument.Parse(reply);
            var questions = doc.Root?.Descendants("registration_questions").Descendants("question").ToList();
            var stateNode =
                questions?.FirstOrDefault(
                    x =>
                        x.Attribute("description")
                            .Value.ToString()
                            .ToLower()
                            .Equals("state", StringComparison.OrdinalIgnoreCase));
            var schoolNode =
                questions?.FirstOrDefault(
                    x =>
                        x.Attribute("description")
                            .Value.ToString()
                            .ToLower()
                            .Equals("school", StringComparison.OrdinalIgnoreCase));

            var stateQuestionId = stateNode?.Attribute("id")?.Value.ToString() ?? string.Empty;
            var schoolQuestionId = schoolNode?.Attribute("id")?.Value.ToString() ?? string.Empty;

            var userAnswers = doc.Root?.Descendants("user_list").Descendants("user").ToList();
            var userAnswer =
                userAnswers?.FirstOrDefault(
                    x =>
                        x.Attribute("login")
                            .Value.ToString()
                            .ToLower()
                            .Equals(userEmail, StringComparison.OrdinalIgnoreCase));
            var stateAnswer = userAnswer?.Attribute("registration_question_" + stateQuestionId)?.Value ?? String.Empty;
            var schoolAnswer = userAnswer?.Attribute("registration_question_" + schoolQuestionId)?.Value ?? String.Empty;
            var result = new Dictionary<string, string>(){ {"state", stateAnswer }, {"school", schoolAnswer }};
            return result;
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
                    { "ParticipantName", certInfo.ParticipantName },//hardcoded values in pdf template (length of custom fields)
                    { "CourseTitle", certInfo.CourseTitle },//hardcoded values in pdf template (length of custom fields)
                    { "CtoNumber", certInfo.CtoNumber },
                    { "Duration", certInfo.Duration },//hardcoded values in pdf template (length of custom fields)
                    { "Date",  certInfo.Date },
                    { "ExpiresDate",  certInfo.ExpiresDate },
                    { "KnowledgeArea",  certInfo.KnowledgeArea },
                    { "Signature",  certInfo.Signature },
                    { "SpecificId",  certInfo.SpecificId },
                    { "TrainerName",  certInfo.TrainerName },
                    { "EventApprovalCode",  certInfo.EventApprovalCode },
                    { "StateTrainerNumber",  certInfo.StateTrainerNumber },
                    { "StateTrainerCourseNumber",  certInfo.StateTrainerCourseNumber },
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