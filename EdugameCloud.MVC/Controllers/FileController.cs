namespace EdugameCloud.MVC.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Mime;
    using System.Text.RegularExpressions;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.UI;
    using System.Xml.Linq;
    using EdugameCloud.Core.Business.Models;
    using EdugameCloud.Core.Domain.DTO;
    using EdugameCloud.Core.Domain.Entities;
    using EdugameCloud.Lti.API.AdobeConnect;
    using EdugameCloud.Lti.Core.Business.Models;
    using EdugameCloud.Lti.Domain.Entities;
    using EdugameCloud.MVC.Attributes;
    using EdugameCloud.MVC.Services;
    using EdugameCloud.MVC.ViewModels;
    using EdugameCloud.MVC.ViewResults;
    using Esynctraining.Core.Business.Models;
    using Esynctraining.Core.Extensions;
    using Esynctraining.Core.Logging;
    using Esynctraining.Core.Providers;
    using Esynctraining.Core.Utils;
    using Microsoft.Reporting.WebForms;
    using NHibernate.Util;

    [HandleError]
    public partial class FileController : BaseController
    {
        #region Fields
        
        private readonly ILogger logger;
        private readonly CompanyModel companyModel;
        private readonly LmsCompanyModel lmsCompanyModel;  // lti reports
        private readonly IAdobeConnectAccountService adobeConnectAccountService; // lti reports
        
        private readonly SNGroupDiscussionModel groupDiscussionModel;
        private readonly UserModel userModel;
        private readonly LmsUserSessionModel userSessionModel;
        private readonly SurveyResultModel surveyResultModel;
        private readonly ACSessionModel sessionModel;
        
        private readonly ACUserModeModel userModeModel; // lti reports
        
        private readonly AuthenticationModel authenticationModel;
        private readonly VCFModel vcfModel;
        private readonly QuizResultModel quizResultModel;
        private readonly TestResultModel testResultModel;

        #endregion

        #region Constructors and Destructors

        public FileController(
            VCFModel vcfModel, 
            CompanyModel companyModel, 
            SNGroupDiscussionModel groupDiscussionModel, 
            UserModel userModel, 
            SurveyResultModel surveyResultModel,
            ACSessionModel sessionModel,
            ACUserModeModel userModeModel,
            AuthenticationModel authenticationModel,
            ApplicationSettingsProvider settings,
            LmsCompanyModel lmsCompanyModel,
            IAdobeConnectAccountService adobeAccountService, 
            LmsUserSessionModel userSessionModel,
            ILogger logger,
            QuizResultModel quizResultModel,
            TestResultModel testResultModel)
            : base(settings)
        {
            this.vcfModel = vcfModel;
            this.companyModel = companyModel;
            this.groupDiscussionModel = groupDiscussionModel;
            this.userModel = userModel;
            this.surveyResultModel = surveyResultModel;
            this.sessionModel = sessionModel;
            this.userModeModel = userModeModel;
            this.authenticationModel = authenticationModel;
            this.lmsCompanyModel = lmsCompanyModel;
            this.adobeConnectAccountService = adobeAccountService;
            this.userSessionModel = userSessionModel;
            this.logger = logger;
            this.quizResultModel = quizResultModel;
            this.testResultModel = testResultModel;
        }

        #endregion

        #region Properties
        
        public User CurrentUser
        {
            get
            {
                return (User)this.authenticationModel.GetCurrentUser(x => this.userModel.GetOneByEmail(x).Value);
            }
        }

        private LmsProviderModel LmsProviderModel
        {
            get { return IoC.Resolve<LmsProviderModel>(); }
        }

        #endregion

        #region Public Methods and Operators
        
        /// <summary>
        /// Get exported file.
        /// </summary>
        /// <param name="id">Unique id.</param>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        [HttpGet]
        [OutputCache(Duration = 0, NoStore = true, Location = OutputCacheLocation.None)]
        [ActionName("export-questions")]
        [CustomAuthorize]
        public virtual ActionResult ExportQuestions(string id)
        {
            var user = this.CurrentUser;
            if (user != null)
            {
                var fileStorage = this.Settings.FileStorage as string ?? string.Empty;
                var storagePath = fileStorage.StartsWith("~") ? HttpContext.Server.MapPath(fileStorage) : fileStorage;
                var exportSubPath = Settings.ExportSubPath as string;
                var fileName = id + ".xml";

                if (storagePath == null || exportSubPath == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.NotFound);
                }

                var filePath = Path.Combine(storagePath, exportSubPath, fileName);
                if (!System.IO.File.Exists(filePath))
                {
                    logger.WarnFormat("ExportQuestions error. File doesn't exist. Path: {0}.", filePath);
                    return new HttpStatusCodeResult(HttpStatusCode.NotFound);
                }
                
                byte[] dataArray;
                try
                {
                    dataArray = System.IO.File.ReadAllBytes(filePath);
                    System.IO.File.Delete(filePath);
                }
                catch (Exception ex)
                {
                    logger.WarnFormat(ex, "ExportQuestions error. File Path: {0}.", filePath);
                    return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
                }
                
                return new FileContentResult(dataArray, MediaTypeNames.Text.Xml);
            }

            return null;
        }
        
        /// <summary>
        /// Add imported file.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        [HttpPost]
        [ValidateInput(false)]
        [OutputCache(Duration = 0, NoStore = true, Location = OutputCacheLocation.None)]
        [ActionName("import-questions-from-xml")]
        [CustomAuthorize]
        public virtual ActionResult ImportQuestions(ImportQuestionsViewModel model)
        {
            var user = this.CurrentUser;
            if (user != null)
            {
                var fileStorage = this.Settings.FileStorage as string ?? string.Empty;
                var storagePath = fileStorage.StartsWith("~") ? HttpContext.Server.MapPath(fileStorage) : fileStorage;
                var importSubPath = Settings.ImportSubPath as string;
                var id = Guid.NewGuid().ToString();
                var fileName = id + ".xml";

                if (storagePath == null || importSubPath == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.NotFound);
                }
                
                var dataArray = model.XmlToImport;
                var filePath = Path.Combine(storagePath, importSubPath, fileName);
                try
                {
                    System.IO.File.WriteAllText(filePath, dataArray);
                }
                catch (Exception ex)
                {
                    logger.WarnFormat(ex, "import-questions-from-xml error. File Path: {0}.", filePath);
                    return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
                }
                
                return new ContentResultWithStatusCode(HttpStatusCode.OK) { Content = id };
            }

            return null;
        }

        /// <summary>
        /// Add imported file.
        /// </summary>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        [HttpPost]
        [ValidateInput(false)]
        [OutputCache(Duration = 0, NoStore = true, Location = OutputCacheLocation.None)]
        [ActionName("import-questions")]
        public virtual ActionResult ImportQuestions()
        {
            var fileStorage = this.Settings.FileStorage as string ?? string.Empty;
            var storagePath = fileStorage.StartsWith("~") ? HttpContext.Server.MapPath(fileStorage) : fileStorage;
            var importSubPath = Settings.ImportSubPath as string;
            var id = Guid.NewGuid().ToString();
            var fileName = id + ".xml";

            if (storagePath == null || importSubPath == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            if (Request.Files.Any())
            {
                var filePath = Path.Combine(storagePath, importSubPath, fileName);
                try
                {
                    foreach (string file in Request.Files)
                    {
                        // ReSharper disable once RedundantCast
                        var hpf = Request.Files[file] as HttpPostedFileBase;
                        if (hpf != null)
                        {
                            if (hpf.ContentLength == 0)
                            {
                                continue;
                            }

                            hpf.SaveAs(filePath);
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.WarnFormat(ex, "import-questions error. File Path: {0}.", filePath);
                    return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
                }                
            }
            
            return new ContentResultWithStatusCode(HttpStatusCode.OK) { Content = id };
        }
        
        [HttpGet]
        [ActionName("test-import-questions")]
        [CustomAuthorize]
        public virtual ActionResult TestImportQuestions()
        {
            var user = this.CurrentUser;
            if (user != null)
            {
                return this.View(EdugameCloudT4.File.Views.ImportQuestions, string.Empty);
            }

            return null;
        }

        /// <summary>
        /// Get full file data.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        [HttpPost]
        [ValidateInput(false)]
        [OutputCache(Duration = 0, NoStore = true, Location = OutputCacheLocation.None)]
        [ActionName("toVcf")]
        [CustomAuthorize(RedirectToLogin = true)]
        public virtual ActionResult ConvertToVCF(VCFViewModel model)
        {
            if (this.ModelState.IsValid)
            {
                string fileName, exception;
                byte[] result = this.vcfModel.ConvertToVCF(model.XmlProfile, out fileName, out exception);
                if (result != null)
                {
                    return this.File(result, @"text/x-vcard", fileName.Trim());
                }

                if (exception != null)
                {
                    model.SetController(this);
                    return this.View(EdugameCloudT4.File.Views.VCF, model);
                }

                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            model.SetController(this);
            return this.View(EdugameCloudT4.File.Views.VCF, model);
        }

        /// <summary>
        ///     The convert to VCF.
        /// </summary>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        [HttpGet]
        [ActionName("toVcf")]
////      [Authorize]
        public virtual ActionResult ConvertToVCF()
        {
            return this.View(EdugameCloudT4.File.Views.VCF, new VCFViewModel(this));
        }

        /// <summary>
        /// The Quiz report.
        /// </summary>
        /// <param name="userId">
        /// user id
        /// </param>
        /// <param name="sessionId">
        /// session id
        /// </param>
        /// <param name="format">
        /// The format.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        [HttpGet]
        [OutputCache(Duration = 0, NoStore = true, Location = OutputCacheLocation.None)]
        [ActionName("collaboration-report")]
        [CustomAuthorize]
        public virtual ActionResult GetCollaborationReport(int userId, int? sessionId, string format = "pdf")
        {
            try
            {
                var cu = this.CurrentUser;
                if (cu != null)
                {
                    User user = IoC.Resolve<UserModel>().GetOneById(userId).Value;
                    string outputName = user.FullName.Replace(" ", "-") + "-collaboration-report-";

                    var userSessions = this.sessionModel.GetSNSessionsByUserId(userId).ToList();
                    if (sessionId.HasValue)
                    {
                        userSessions = userSessions.Where(us => us.acSessionId == sessionId.Value).ToList();
                        outputName += userSessions.FirstOrDefault().Return(x => x.dateCreated, DateTime.Today).ToString("MM-dd-yyyy");
                    }
                    else
                    {
                        outputName += DateTime.Today.ToString("MM-dd-yyyy");
                    }

                    var sessionsById = userSessions.ToDictionary(s => s.acSessionId, s => s);

                    Dictionary<int, SNGroupDiscussion> discussions =
                        IoC.Resolve<SNGroupDiscussionModel>()
                            .GetAllByACSessionIds(sessionsById.Keys.ToList())
                            .GroupBy(o => o.ACSessionId, o => o)
                            .ToDictionary(g => g.Key, g => g.FirstOrDefault());
                    Dictionary<int, List<dynamic>> members =
                        IoC.Resolve<SNMemberModel>()
                            .GetAllByACSessionIds(sessionsById.Keys.ToList())
                            .Select(x => this.ConvertParticipant(x))
                            .ToList()
                            .GroupBy(o => (int)o.ACSessionId, o => o)
                            .ToDictionary(g => g.Key, g => g.ToList());

                    List<dynamic> messageData = this.FillMessageData(discussions);

                    Dictionary<dynamic, List<dynamic>> messages =
                        messageData.GroupBy(o => o.ACSessionId, o => o).ToDictionary(g => g.Key, g => g.ToList());

                    var sessionResults = sessionsById.ToDictionary(
                        kvp => kvp.Value,
                        kvp =>
                        new
                            {
                                discussion = discussions.ContainsKey(kvp.Key) ? discussions[kvp.Key] : null,
                                members = members.ContainsKey(kvp.Key) ? members[kvp.Key] : new List<dynamic>()
                            });

                    Func<SNSessionFromStoredProcedureDTO, IDictionary<int, string>, object> resultConverter =
                        (s, userModes) =>
                        new
                            {
                                s.acSessionId,
                                acUserMode = userModes[s.acUserModeId],
                                showUserMode = false,
                                name = s.groupDiscussionTitle,
                                reportType = "collaboration",
                                s.categoryName,
                                s.dateCreated,
                            };

                    SubreportProcessingEventHandler detailsHandler = (sender, args) =>
                        {
                            try
                            {
                                args.DataSources.Clear();
                                int acSessionId = int.Parse(args.Parameters["acSessionId"].Values.First());
                                var acSession = sessionResults.Keys.First(s => s.acSessionId == acSessionId);
                                List<dynamic> sessionMessages =
                                    messages.Return(
                                        map => map.ContainsKey(acSessionId) ? map[acSessionId] : new List<dynamic>(),
                                        new List<dynamic>());
                                var details =
                                    new[] { acSession }.Select(
                                        s =>
                                        new
                                            {
                                                acSessionId,
                                                discussion =
                                            sessionResults[s].discussion.Return(d => d.GroupDiscussionTitle, string.Empty),
                                                dateCreated =
                                            sessionResults[s].discussion.Return(d => d.DateCreated, DateTime.MinValue),
                                                totalMessages = sessionMessages.Count,
                                                totalLikes = sessionMessages.Sum(m => m.likes),
                                                totalDislikes = sessionMessages.Sum(m => m.dislikes),
                                                total = sessionResults[s].members.Count,
                                                active = s.activeParticipants,
                                            }).ToList();
                                args.DataSources.Add(new ReportDataSource("ItemDataSet", details));
                            }
                            catch (Exception ex)
                            {
                                IoC.Resolve<ILogger>().Error("FileController.GetCollaborationReport.detailsHandler", ex);
                                throw;
                            }
                        };

                    SubreportProcessingEventHandler participantsHandler = (sender, args) =>
                        {
                            try
                            {
                                args.DataSources.Clear();
                                int acSessionId = int.Parse(args.Parameters["acSessionId"].Values.First());
                                var acSession = sessionResults.Keys.First(s => s.acSessionId == acSessionId);
                                List<dynamic> sessionMessages =
                                    messages.Return(
                                        map => map.ContainsKey(acSessionId) ? map[acSessionId] : new List<dynamic>(),
                                        new List<dynamic>());
                                var participants =
                                    sessionResults[acSession].members.Select(
                                        p =>
                                        new
                                            {
                                                acSessionId,
                                                profile =
                                            string.IsNullOrWhiteSpace(p.ParticipantProfile)
                                                ? null
                                                : Url.ActionAbsolute(
                                                    EdugameCloudT4.File.GetProfileVCard(acSessionId, (int)p.Id))
                                                  + (!string.IsNullOrWhiteSpace(user.SessionToken)
                                                         ? "&egcSession=" + user.SessionToken
                                                         : string.Empty),
                                                participant = p.Participant,
                                                totalMessages =
                                            (int)p.ParsedProfile.id != 0
                                                ? sessionMessages.Count(m => (int)m.userId == (int)p.ParsedProfile.id)
                                                : sessionMessages.Count(
                                                    m => m.userName == p.Participant || m.userName == p.ParsedProfile.name),
                                                totalLikes =
                                            (int)p.ParsedProfile.id != 0
                                                ? sessionMessages.Where(m => (int)m.userId == (int)p.ParsedProfile.id)
                                                      .Sum(m => m.likes)
                                                : sessionMessages.Where(
                                                    m => m.userName == p.Participant || m.userName == p.ParsedProfile.name)
                                                      .Sum(m => m.likes),
                                                totalDislikes =
                                            (int)p.ParsedProfile.id != 0
                                                ? sessionMessages.Where(m => (int)m.userId == (int)p.ParsedProfile.id)
                                                      .Sum(m => m.dislikes)
                                                : sessionMessages.Where(
                                                    m => m.userName == p.Participant || m.userName == p.ParsedProfile.name)
                                                      .Sum(m => m.dislikes),
                                            }).ToList();
                                args.DataSources.Add(new ReportDataSource("ItemDataSet", participants));
                            }
                            catch (Exception ex)
                            {
                                IoC.Resolve<ILogger>().Error("FileController.GetCollaborationReport.participantsHandler", ex);
                                throw;
                            }
                        };

                    SubreportProcessingEventHandler messagesHandler = (sender, args) =>
                        {
                            try
                            {
                                args.DataSources.Clear();
                                int acSessionId = int.Parse(args.Parameters["acSessionId"].Values.First());
                                List<dynamic> sessionMessages =
                                    messages.Return(
                                        map => map.ContainsKey(acSessionId) ? map[acSessionId] : new List<dynamic>(),
                                        new List<dynamic>());

                                var messageList =
                                    sessionMessages.Select(
                                        m => new { acSessionId, m.text, participant = m.userName, m.likes, m.dislikes, })
                                        .ToList();
                                args.DataSources.Add(new ReportDataSource("ItemDataSet", messageList));
                            }
                            catch (Exception ex)
                            {
                                IoC.Resolve<ILogger>().Error("FileController.GetCollaborationReport.messagesHandler", ex);
                                throw;
                            }
                        };

                    Dictionary<string, KeyValuePair<string, SubreportProcessingEventHandler>> subReports =
                        new[]
                        {
                            new
                                {
                                    placeholder = "SessionSubReport",
                                    reportName = "SessionDetails.Collaboration",
                                    action = detailsHandler
                                },
                            new
                                {
                                    placeholder = "ParticipantsSubReport",
                                    reportName = "ParticipantDetails.Collaboration",
                                    action = participantsHandler
                                },
                            new
                                {
                                    placeholder = "QuestionsSubReport",
                                    reportName = "QuestionDetails.Collaboration",
                                    action = messagesHandler
                                }
                        }.ToDictionary(
                                o => o.placeholder,
                                o =>
                                new KeyValuePair<string, SubreportProcessingEventHandler>(o.reportName, o.action));

                    return this.GetResultReport(
                        sessionResults,
                        s => s.acUserModeId,
                        resultConverter,
                        "SessionsReport",
                        outputName,
                        format,
                        subReports);
                }
            }
            catch (Exception ex)
            {
                IoC.Resolve<ILogger>().Error("FileController.GetCollaborationReport.", ex);
                throw;
            }

            return null;
        }

        /// <summary>
        /// The crossword report.
        /// </summary>
        /// <param name="userId">
        /// user id
        /// </param>
        /// <param name="sessionId">
        /// session id
        /// </param>
        /// <param name="format">
        /// The format.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        [HttpGet]
        [OutputCache(Duration = 0, NoStore = true, Location = OutputCacheLocation.None)]
        [ActionName("crossword-report")]
        [CustomAuthorize]
        public virtual ActionResult GetCrosswordsReport(int userId, int? sessionId, string format = "pdf")
        {
            try
            {
                var cu = this.CurrentUser;
                if (cu != null)
                {
                    User user = IoC.Resolve<UserModel>().GetOneById(userId).Value;
                    string outputName = user.FullName.Replace(" ", "-") + "-crossword-report-";
                    var appletItemModel = IoC.Resolve<AppletItemModel>();
                    var userSessions = appletItemModel.GetCrosswordSessionsByUserId(userId).ToList();
                    if (sessionId.HasValue)
                    {
                        userSessions = userSessions.Where(us => us.acSessionId == sessionId.Value).ToList();
                        outputName += userSessions.FirstOrDefault().Return(x => x.dateCreated, DateTime.Today).ToString("MM-dd-yyyy");
                    }
                    else
                    {
                        outputName += DateTime.Today.ToString("MM-dd-yyyy");
                    }

                    var sessionResults = userSessions.ToDictionary(s => s, s => appletItemModel.GetCrosswordResultByACSessionId(s.acSessionId).ToList());
                    var crosswords = sessionResults.ToDictionary(s => s.Key, s => this.ReadCrosswordDefinition(s.Value.First().documentXML));

                    Func<CrosswordSessionFromStoredProcedureDTO, IDictionary<int, string>, object> resultConverter =
                        (s, userModes) =>
                        new
                            {
                                s.acSessionId,
                                acUserMode = userModes[s.acUserModeId],
                                showUserMode = true,
                                name = s.appletName,
                                reportType = "crossword",
                                s.categoryName,
                                s.dateCreated,
                            };

                    SubreportProcessingEventHandler detailsHandler = (sender, args) =>
                        {
                            args.DataSources.Clear();
                            int acSessionId = int.Parse(args.Parameters["acSessionId"].Values.First());
                            var acSession = sessionResults.Keys.First(s => s.acSessionId == acSessionId);
                            var details =
                                new[] { acSession }.Select(
                                    s =>
                                    new
                                        {
                                            acSessionId,
                                            total = s.totalParticipants,
                                            active = s.activeParticipants,
                                            averageScore = sessionResults[s].Average(p => p.score),
                                            averageTime =
                                        (long)sessionResults[s].Average(p => (p.endTime - p.startTime).Ticks)
                                        }).ToList();
                            args.DataSources.Add(new ReportDataSource("ItemDataSet", details));
                        };

                    SubreportProcessingEventHandler participantsHandler = (sender, args) =>
                        {
                            args.DataSources.Clear();
                            int acSessionId = int.Parse(args.Parameters["acSessionId"].Values.First());
                            var acSession = sessionResults.Keys.First(s => s.acSessionId == acSessionId);
                            var questions = crosswords[acSession];
                            var participants =
                                sessionResults[acSession].Select(
                                    p =>
                                    new
                                        {
                                            acSessionId,
                                            rank = p.position,
                                            p.score,
                                            p.startTime,
                                            p.endTime,
                                            p.participantName,
                                            totalQuestions = questions.Count,
                                            scorePercent = ((double)p.score / questions.Count).ToString("0.0%")
                                        }).ToList();
                            args.DataSources.Add(new ReportDataSource("ItemDataSet", participants));
                        };

                    SubreportProcessingEventHandler questionsHandler = (sender, args) =>
                        {
                            args.DataSources.Clear();
                            int acSessionId = int.Parse(args.Parameters["acSessionId"].Values.First());
                            var acSession = sessionResults.Keys.First(s => s.acSessionId == acSessionId);
                            var count = crosswords[acSession].Count;
                            var questions =
                                crosswords[acSession].Select(
                                    q => new { acSessionId, question = q.Key, answer = q.Value, totalQuestions = count })
                                    .ToList();
                            args.DataSources.Add(new ReportDataSource("ItemDataSet", questions));
                        };

                    Dictionary<string, KeyValuePair<string, SubreportProcessingEventHandler>> subReports =
                        new[]
                        {
                            new
                                {
                                    placeholder = "SessionSubReport",
                                    reportName = "SessionDetails.Crossword",
                                    action = detailsHandler
                                },
                            new
                                {
                                    placeholder = "ParticipantsSubReport",
                                    reportName = "ParticipantDetails.Crossword",
                                    action = participantsHandler
                                },
                            new
                                {
                                    placeholder = "QuestionsSubReport",
                                    reportName = "QuestionDetails.Crossword",
                                    action = questionsHandler
                                }
                        }.ToDictionary(
                                o => o.placeholder,
                                o =>
                                new KeyValuePair<string, SubreportProcessingEventHandler>(o.reportName, o.action));

                    return this.GetResultReport(
                        sessionResults,
                        s => s.acUserModeId,
                        resultConverter,
                        "SessionsReport",
                        outputName,
                        format,
                        subReports);
                }
            }
            catch (Exception ex)
            {
                IoC.Resolve<ILogger>().Error("FileController.GetCrosswordsReport.", ex);  
                throw;
            }

            return null;
        }
        
        /// <summary>
        /// The products.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        [HttpGet]
        [OutputCache(Duration = 2592000, VaryByParam = "id", NoStore = false, Location = OutputCacheLocation.Any)]
        [ActionName("get-group-discussion")]
        [CustomAuthorize]
        public virtual ActionResult GetGroupDiscussion(int id)
        {
            var user = this.CurrentUser;
            if (user != null)
            {
                SNGroupDiscussion groupDiscussion = this.groupDiscussionModel.GetOneById(id).Value;
                if (groupDiscussion != null)
                {
                    string xsdValidationError;
                    if (
                        !XsdValidator.ValidateXmlAgainsXsd(
                            groupDiscussion.GroupDiscussionData,
                            System.Web.HttpContext.Current.Server.MapPath(Links.Content.xsd.snGroupDiscussion_xsd),
                            out xsdValidationError))
                    {
                        return this.Content(xsdValidationError);
                    }

                    byte[] buffer = this.groupDiscussionModel.ConvertGroupDiscussionToExcel(groupDiscussion);
                    if (buffer != null)
                    {
                        return this.File(buffer, "application/vnd.ms-excel", new Regex(@"[^\w]+", RegexOptions.IgnoreCase).Replace(groupDiscussion.GroupDiscussionTitle, "-").TruncateIfMoreThen(50) + ".xlsx");
                    }
                }

                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            return null;
        }

        /// <summary>
        /// Get full file data.
        /// </summary>
        /// <param name="sessionId">
        /// The session Id.
        /// </param>
        /// <param name="snMemberId">
        /// The SN Member Id.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "Reviewed. Suppression is OK here."), HttpGet]
        [OutputCache(Duration = 2592000, VaryByParam = "sessionId;snMemberId", NoStore = false, 
            Location = OutputCacheLocation.Any)]
        [ActionName("get-profile-vCard")]
        [CustomAuthorize(RedirectToLogin = true)]
        public virtual ActionResult GetProfileVCard(int sessionId, int snMemberId)
        {
            SNMember snMember = IoC.Resolve<SNMemberModel>().GetOneById(snMemberId).Value;

            if (snMember != null && snMember.ACSessionId == sessionId
                && !string.IsNullOrWhiteSpace(snMember.ParticipantProfile))
            {
                string fileName, exception;
                byte[] result = this.vcfModel.ConvertToVCF(snMember.ParticipantProfile, out fileName, out exception);
                if (result != null)
                {
                    return this.File(result, @"text/x-vcard", fileName);
                }

                if (exception != null)
                {
                    return this.Content(exception);
                }
            }

            return new HttpStatusCodeResult(HttpStatusCode.NotFound);
        }

        /// <summary>
        /// The Quiz report.
        /// </summary>
        /// <param name="userId">
        /// user id
        /// </param>
        /// <param name="sessionId">
        /// session id
        /// </param>
        /// <param name="format">
        /// The format.
        /// </param>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        [HttpGet]
        [OutputCache(Duration = 0, NoStore = true, Location = OutputCacheLocation.None)]
        [ActionName("quiz-report")]
        [CustomAuthorize]
        public virtual ActionResult GetQuizReport(int userId, int? sessionId, string format = "pdf", string type = "full", bool detailed = false, string quizType = "live")
        {
            var cu = this.CurrentUser;
            if (cu != null)
            {
                type = (string.IsNullOrWhiteSpace(type) ? "full" : type).ToLower();
                User user = IoC.Resolve<UserModel>().GetOneById(userId).Value;
                string outputName = user.FullName.Replace(" ", "-") + "-quiz-report-";
                var userSessions = this.sessionModel.GetQuizSessionsByUserId(userId).ToList();
                if (sessionId.HasValue)
                {
                    userSessions = userSessions.Where(us => us.acSessionId == sessionId.Value).ToList();
                    outputName +=
                        userSessions.FirstOrDefault().Return(x => x.dateCreated, DateTime.Today).ToString("MM-dd-yyyy");
                }
                else
                {
                    outputName += DateTime.Today.ToString("MM-dd-yyyy");
                }

                if (detailed)
                {
                    IList<ExtendedReportDto> extendedReportDtos = new List<ExtendedReportDto>();

                    /// If QuizResult is related to Goodard There is session reletaed to Post Quiz.
                    /// We need to detect this case and get postData for Post quiz result.
                    var liveQuizData = quizResultModel.GetExtendedReportQuizReportData(sessionId.Value, true);
                    extendedReportDtos.Add(liveQuizData);
                    var postQuizData = quizResultModel.GetExtendedReportQuizReportData(sessionId.Value, false);
                    if (postQuizData.ReportResults.Any())
                    {
                        extendedReportDtos.Add(postQuizData);
                    }


                    int intType;
                    if (!int.TryParse(type, out intType))
                        throw new InvalidOperationException($"Invalid detailed report type: {type}");
                    var reportService = ReportServiceFactory.GetReportService(SubModuleItemType.Quiz, intType);

                    var bytes = reportService.GetExcelExtendedReportBytes(extendedReportDtos);
                    return this.File(
                        bytes,
                        "application/vnd.ms-excel",
                        string.Format("{0}.{1}", outputName, "xlsx"));
                }

                var sessionResults = userSessions.ToDictionary(
                    s => s,
                    s => quizResultModel.GetQuizResultByACSessionId(s.acSessionId, s.subModuleItemId));

                /////////////////////////////////////.
                /// We need to remove all session related to POST quiz.
                /// In Basic Mode we show only information about Live Quiz
                var postSessions = sessionResults.Where(d => d.Value.players.Any(p => p.isPostQuiz)).ToList();
                foreach (var s in postSessions)
                {
                    sessionResults.Remove(s.Key);
                }
                /// /////////////////////////////////

                Func<QuizSessionFromStoredProcedureDTO, IDictionary<int, string>, object> resultConverter =
                    (s, userModes) =>
                    new
                        {
                            s.acSessionId,
                            acUserMode = userModes[s.acUserModeId],
                            showUserMode = true,
                            name = s.quizName,
                            reportType = "quiz",
                            s.categoryName,
                            s.dateCreated,
                        };

                SubreportProcessingEventHandler detailsHandler = (sender, args) =>
                    {
                        args.DataSources.Clear();
                        int acSessionId = int.Parse(args.Parameters["acSessionId"].Values.First());
                        QuizSessionFromStoredProcedureDTO acSession = sessionResults.Keys.First(s => s.acSessionId == acSessionId);
                        var details =
                            new[] { acSession }.Select(
                                s =>
                                {
                                    var players = sessionResults[s].players.Where(p => p.score > 0);
                                    return new
                                    {
                                        acSessionId,
                                        totalQuestions = s.TotalQuestion,
                                        s.TotalScore,
                                        total = s.totalParticipants,
                                        active = s.activeParticipants,
                                        averageScore = players.Any() ? players.Average(p => p.score) : 0,
                                        averageTime = players.Any()
                                            ? (long) players.Average(
                                                p =>
                                                    (p.endTime.ConvertFromUnixTimeStamp() -
                                                     p.startTime.ConvertFromUnixTimeStamp()).Ticks)
                                            : 0
                                    };
                                }).ToList();
                        args.DataSources.Add(new ReportDataSource("ItemDataSet", details));
                    };

                SubreportProcessingEventHandler participantsHandler = (sender, args) =>
                    {
                        args.DataSources.Clear();
                        int acSessionId = int.Parse(args.Parameters["acSessionId"].Values.First());
                        QuizSessionFromStoredProcedureDTO acSession = sessionResults.Keys.First(s => s.acSessionId == acSessionId);
                        var questions = sessionResults[acSession].questions;
                        var participants =
                            sessionResults[acSession].players.ToList().Select(
                                p =>
                                new
                                    {
                                        acSessionId,
                                        rank = p.position,
                                        p.score,
                                        startTime = p.startTime.ConvertFromUnixTimeStamp(), // p.score > 0 ? p.startTime : DateTime.Today,
                                        endTime = p.endTime.ConvertFromUnixTimeStamp(), // p.score > 0 ? p.endTime : DateTime.Today,
                                        p.participantName,
                                        participantEmail = p.acEmail,
                                        showEmail = acSession.includeAcEmails ?? false,
                                        totalQuestions = questions.Length,
                                        scorePercent = ((double)p.score / questions.Length).ToString("0.0%")
                                    }).ToList();
                        args.DataSources.Add(new ReportDataSource("ItemDataSet", participants));
                    };

                SubreportProcessingEventHandler questionsHandler = (sender, args) =>
                    {
                        args.DataSources.Clear();
                        int acSessionId = int.Parse(args.Parameters["acSessionId"].Values.First());
                        QuizSessionFromStoredProcedureDTO acSession = sessionResults.Keys.First(s => s.acSessionId == acSessionId);
                        var questions =
                            sessionResults[acSession].questions.Select(
                                q =>
                                new
                                    {
                                        acSessionId,
                                        q.question,
                                        questionType = q.questionTypeName,
                                        q.isMandatory,
                                        totalCorrect = q.correctAnswerCount,
                                        totalQuestions = acSession.TotalQuestion
                                    }).ToList();
                        args.DataSources.Add(new ReportDataSource("ItemDataSet", questions));
                    };

                Dictionary<string, KeyValuePair<string, SubreportProcessingEventHandler>> subReports =
                    new[]
                        {
                            new
                                {
                                    placeholder = "SessionSubReport",
                                    reportName = "SessionDetails.Quiz",
                                    action = detailsHandler
                                },
                            new
                                {
                                    placeholder = "ParticipantsSubReport",
                                    reportName = "ParticipantDetails.Quiz",
                                    action = participantsHandler
                                },
                            new
                                {
                                    placeholder = "QuestionsSubReport",
                                    reportName = "QuestionDetails.Quiz",
                                    action = questionsHandler
                                }
                        }.ToDictionary(
                            o => o.placeholder,
                            o =>
                            new KeyValuePair<string, SubreportProcessingEventHandler>(o.reportName, o.action));

                return this.GetResultReport(
                    sessionResults,
                    s => s.acUserModeId,
                    resultConverter,
                    type == "full" ? "SessionsReport" : "SessionsReportNoQuestionsNoParticipants",
                    outputName,
                    format,
                    subReports);
            }

            return null;
        }

        [HttpGet]
        [OutputCache(Duration = 0, NoStore = true, Location = OutputCacheLocation.None)]
        [ActionName("survey-report")]
        [CustomAuthorize]
        public virtual ActionResult GetSurveyReport(int userId, int? sessionId, string format = "pdf", string type = "full", bool detailed = false)
        {
            var cu = this.CurrentUser;
            if (cu != null)
            {
                type = (string.IsNullOrWhiteSpace(type) ? "full" : type).ToLower();
                User user = IoC.Resolve<UserModel>().GetOneById(userId).Value;
                string outputName = user.FullName.Replace(" ", "-") + "-survey-report-";

                List<SurveySessionFromStoredProcedureDTO> userSessions = this.sessionModel.GetSurveySessionsByUserId(userId).ToList();
                if (sessionId.HasValue)
                {
                    userSessions = userSessions.Where(us => us.acSessionId == sessionId.Value).ToList();
                    outputName +=
                        userSessions.FirstOrDefault().Return(x => x.dateCreated, DateTime.Today).ToString("MM-dd-yyyy");
                }
                else
                {
                    outputName += DateTime.Today.ToString("MM-dd-yyyy");
                }
                if (detailed)
                {
                    var data = surveyResultModel.GetExtendedReportSurveyReportData(sessionId.Value);
                    int intType;
                    if(!int.TryParse(type, out intType))
                        throw new InvalidOperationException($"Invalid detailed report type: {type}");
                    var reportService = ReportServiceFactory.GetReportService(SubModuleItemType.Survey, intType);
                    var bytes = reportService.GetExcelExtendedReportBytes(new ExtendedReportDto[] { data });
                    return this.File(
                        bytes,
                        "application/vnd.ms-excel",
                        string.Format("{0}.{1}", outputName, "xlsx"));
                }
                var sessionResults = userSessions.ToDictionary(
                    s => s,
                    s => this.surveyResultModel.GetSurveyResultByACSessionId(s.acSessionId, s.subModuleItemId));

                SubreportProcessingEventHandler detailsHandler = (sender, args) =>
                    {
                        args.DataSources.Clear();
                        int acSessionId = int.Parse(args.Parameters["acSessionId"].Values.First());
                        SurveySessionFromStoredProcedureDTO acSession = sessionResults.Keys.First(s => s.acSessionId == acSessionId);
                        var details =
                            new[] { acSession }.Select(
                                s =>
                                new
                                    {
                                        acSessionId,
                                        totalQuestions = s.TotalQuestion,
                                        s.TotalScore,
                                        total = sessionResults[s].players.Length,
                                        active = s.activeParticipants,
                                        averageScore = sessionResults[s].players.Average(p => p.score),
                                        averageTime =
                                    (long)sessionResults[s].players.Average(p => (p.endTime.ConvertFromUnixTimeStamp() - p.startTime.ConvertFromUnixTimeStamp()).Ticks)
                                    })
                                .ToList();
                        args.DataSources.Add(new ReportDataSource("ItemDataSet", details));
                    };

                SubreportProcessingEventHandler participantsHandler = (sender, args) =>
                    {
                        args.DataSources.Clear();
                        int acSessionId = int.Parse(args.Parameters["acSessionId"].Values.First());
                        SurveySessionFromStoredProcedureDTO acSession = sessionResults.Keys.First(s => s.acSessionId == acSessionId);
                        var participants =
                            sessionResults[acSession].players.Select(
                                p =>
                                new
                                    {
                                        acSessionId,
                                        rank = p.position,
                                        p.score,
                                        startTime = p.startTime.ConvertFromUnixTimeStamp(),
                                        endTime = p.endTime.ConvertFromUnixTimeStamp(),
                                        p.participantName,
                                        totalQuestions = acSession.TotalQuestion,
                                        scorePercent = ((double)p.score / acSession.TotalQuestion).ToString("0.0%")
                                    })
                                .ToList();
                        args.DataSources.Add(new ReportDataSource("ItemDataSet", participants));
                    };

                SubreportProcessingEventHandler questionsHandler = (sender, args) =>
                    {
                        args.DataSources.Clear();
                        int acSessionId = int.Parse(args.Parameters["acSessionId"].Values.First());
                        SurveySessionFromStoredProcedureDTO acSession = sessionResults.Keys.First(s => s.acSessionId == acSessionId);
                        var questions =
                            sessionResults[acSession].questions.Select(
                                q =>
                                new
                                    {
                                        acSessionId,
                                        q.questionId,
                                        q.question,
                                        questionType = q.questionTypeName,
                                        q.isMandatory,
                                        totalCorrect = q.correctAnswerCount,
                                        totalQuestions = acSession.TotalQuestion
                                    }).ToList();
                        args.DataSources.Add(new ReportDataSource("ItemDataSet", questions));
                    };

                SubreportProcessingEventHandler answersStatisticsHandler = (sender, args) =>
                    {
                        args.DataSources.Clear();
                        int acSessionId = int.Parse(args.Parameters["acSessionId"].Values.First());
                        int questionId = int.Parse(args.Parameters["questionId"].Values.First());
                        SurveySessionFromStoredProcedureDTO acSession = sessionResults.Keys.First(s => s.acSessionId == acSessionId);
                        var results = this.ConvertAnswers(sessionResults, acSession, questionId).ToList();
                        args.DataSources.Add(new ReportDataSource("ItemDataSet", results));
                    };

                SubreportProcessingEventHandler participantsStatisticsHandler = (sender, args) =>
                    {
                        args.DataSources.Clear();
                        int acSessionId = int.Parse(args.Parameters["acSessionId"].Values.First());
                        int questionId = int.Parse(args.Parameters["questionId"].Values.First());
                        SurveySessionFromStoredProcedureDTO acSession = sessionResults.Keys.First(s => s.acSessionId == acSessionId);
                        var results =
                            this.ConvertAnswersForParticipants(sessionResults, acSession, questionId)
                                .Select(
                                    pa =>
                                    new
                                        {
                                            participantId = pa.player.surveyResultId,
                                            participant = pa.player.participantName,
                                            pa.answer,
                                        })
                                .ToList();
                        args.DataSources.Add(new ReportDataSource("ItemDataSet", results));
                    };

                Dictionary<string, KeyValuePair<string, SubreportProcessingEventHandler>> subReports =
                    new[]
                        {
                            new
                                {
                                    placeholder = "SessionSubReport",
                                    reportName = "SessionDetails.Survey",
                                    action = detailsHandler
                                },
                            new
                                {
                                    placeholder = "ParticipantsSubReport",
                                    reportName = "ParticipantDetails.Survey",
                                    action = participantsHandler
                                },
                            new
                                {
                                    placeholder = "QuestionsSubReport",
                                    reportName = "QuestionDetails.Survey",
                                    action = questionsHandler
                                },
                            new
                                {
                                    placeholder = "QuestionStatistics",
                                    reportName = "QuestionDetails.Survey.AnswersStatistics",
                                    action = answersStatisticsHandler
                                },
                            new
                                {
                                    placeholder = "QuestionPaticipants",
                                    reportName = "QuestionDetails.Survey.ParticipantsStatistics",
                                    action = participantsStatisticsHandler
                                }
                        }.ToDictionary(
                            o => o.placeholder,
                            o =>
                            new KeyValuePair<string, SubreportProcessingEventHandler>(o.reportName, o.action));

                Func<SurveySessionFromStoredProcedureDTO, IDictionary<int, string>, object> resultConverter =
                    (s, userModes) =>
                    new
                        {
                            s.acSessionId,
                            acUserMode = userModes[s.acUserModeId],
                            showUserMode = false,
                            name = s.surveyName,
                            reportType = "survey",
                            s.categoryName,
                            s.dateCreated,
                        };

                return this.GetResultReport(
                    sessionResults,
                    s => s.acUserModeId,
                    resultConverter,
                    type == "full" ? "SessionsReport" : "SessionsReportNoQuestionsNoParticipants",
                    outputName,
                    format,
                    subReports);
            }

            return null;
        }

        /// <summary>
        /// The Test report.
        /// </summary>
        /// <param name="userId">
        /// user id
        /// </param>
        /// <param name="sessionId">
        /// session id
        /// </param>
        /// <param name="format">
        /// The format.
        /// </param>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        [HttpGet]
        [OutputCache(Duration = 0, NoStore = true, Location = OutputCacheLocation.None)]
        [ActionName("test-report")]
        [CustomAuthorize]
        public virtual ActionResult GetTestReport(int userId, int? sessionId, string format = "pdf", string type = "full", bool detailed = false)
        {
            var cu = this.CurrentUser;
            if (cu != null)
            {
                type = (string.IsNullOrWhiteSpace(type) ? "full" : type).ToLower();
                User user = IoC.Resolve<UserModel>().GetOneById(userId).Value;
                string outputName = user.FullName.Replace(" ", "-") + "-test-report-";

                List<TestSessionDTO> userSessions = this.sessionModel.GetTestSessionsByUserId(userId).ToList();
                if (sessionId.HasValue)
                {
                    userSessions = userSessions.Where(us => us.acSessionId == sessionId.Value).ToList();
                    outputName +=
                        userSessions.FirstOrDefault().Return(x => x.dateCreated.ConvertFromUnixTimeStamp(), DateTime.Today).ToString("MM-dd-yyyy");
                }
                else
                {
                    outputName += DateTime.Today.ToString("MM-dd-yyyy");
                }

                if (detailed)
                {
                    var data = testResultModel.GetExtendedReportQuizReportData(sessionId.Value);
                    int intType;
                    if (!int.TryParse(type, out intType))
                        throw new InvalidOperationException($"Invalid detailed report type: {type}");
                    var reportService = ReportServiceFactory.GetReportService(SubModuleItemType.Test, intType);
                    var bytes = reportService.GetExcelExtendedReportBytes(new ExtendedReportDto[] { data });
                    return this.File(
                        bytes,
                        "application/vnd.ms-excel",
                        string.Format("{0}.{1}", outputName, "xlsx"));
                }

                var sessionResults = userSessions.ToDictionary(
                    s => s,
                    s => testResultModel.GetTestResultByACSessionId(s.acSessionId, s.subModuleItemId));

                var testModel = IoC.Resolve<TestModel>();
                var tests = userSessions.ToDictionary(s => s, s => testModel.GetOneBySMIId(s.subModuleItemId).Value);

                Func<TestSessionDTO, IDictionary<int, string>, object> resultConverter =
                    (s, userModes) =>
                    new
                        {
                            s.acSessionId,
                            acUserMode = userModes[s.acUserModeId],
                            showUserMode = false,
                            name = s.testName,
                            reportType = "test",
                            s.categoryName,
                            dateCreated = s.dateCreated.ConvertFromUnixTimeStamp(),
                        };

                SubreportProcessingEventHandler detailsHandler = (sender, args) =>
                    {
                        args.DataSources.Clear();
                        int acSessionId = int.Parse(args.Parameters["acSessionId"].Values.First());
                        TestSessionDTO acSession = sessionResults.Keys.First(s => s.acSessionId == acSessionId);

                        var passingScore = tests[acSession].PassingScore ?? 0;
                        var questions = sessionResults[acSession].questions;
                        var details =
                            new[] { acSession }.Select(
                                s =>
                                new
                                    {
                                        acSessionId,
                                        totalQuestions = questions.Length,
                                        totalScore = s.TotalScore,
                                        total = s.totalParticipants,
                                        active = s.activeParticipants,
                                        averageScore = acSession.avgScore,
                                        averageScorePercent = (acSession.avgScore / questions.Length).ToString("0.0%"),
                                        averageTime =
                                    (long)sessionResults[s].players.Average(p => (p.endTime.ConvertFromUnixTimeStamp() - p.startTime.ConvertFromUnixTimeStamp()).Ticks),
                                        passingScore,
                                        passingScorePercent = ((double)passingScore / 100).ToString("0.0%"),
                                        timeLimit = tests[s].TimeLimit ?? 0,
                                    }).ToList();
                        args.DataSources.Add(new ReportDataSource("ItemDataSet", details));
                    };

                SubreportProcessingEventHandler participantsHandler = (sender, args) =>
                    {
                        args.DataSources.Clear();
                        int acSessionId = int.Parse(args.Parameters["acSessionId"].Values.First());
                        TestSessionDTO acSession = sessionResults.Keys.First(s => s.acSessionId == acSessionId);
                        var questions = sessionResults[acSession].questions;
                        var participants = sessionResults[acSession].players.Select(
                            p =>
                                {
                                    return
                                        new
                                            {
                                                acSessionId,
                                                rank = p.position,
                                                p.score,
                                                startTime = p.startTime.ConvertFromUnixTimeStamp(),
                                                endTime = p.endTime.ConvertFromUnixTimeStamp(),
                                                p.participantName,
                                                participantEmail = p.acEmail,
                                                showEmail = acSession.includeAcEmails ?? false,
                                                totalScore = acSession.TotalScore,
                                                scorePassed = p.scorePassed ? 1 : 0,
                                                timePassed = p.timePassed ? 1 : 0,
                                                totalQuestions = questions.Length,
                                                scorePercent = ((double)p.score / questions.Length).ToString("0.0%")
                                            };
                                })
                            .ToList();
                        args.DataSources.Add(new ReportDataSource("ItemDataSet", participants));
                    };

                SubreportProcessingEventHandler questionsHandler = (sender, args) =>
                    {
                        args.DataSources.Clear();
                        int acSessionId = int.Parse(args.Parameters["acSessionId"].Values.First());
                        TestSessionDTO acSession = sessionResults.Keys.First(s => s.acSessionId == acSessionId);
                        var preQuestions = sessionResults[acSession].questions;
                        var questions =
                            preQuestions.Select(
                                q =>
                                new
                                    {
                                        acSessionId,
                                        q.question,
                                        questionType = q.questionTypeName,
                                        q.isMandatory,
                                        totalCorrect = q.correctAnswerCount,
                                        q.restrictions,
                                        totalQuestions = preQuestions.Length
                                    }).ToList();
                        args.DataSources.Add(new ReportDataSource("ItemDataSet", questions));
                    };

                Dictionary<string, KeyValuePair<string, SubreportProcessingEventHandler>> subReports =
                    new[]
                        {
                            new
                                {
                                    placeholder = "SessionSubReport",
                                    reportName = "SessionDetails.Test",
                                    action = detailsHandler
                                },
                            new
                                {
                                    placeholder = "ParticipantsSubReport",
                                    reportName = "ParticipantDetails.Test",
                                    action = participantsHandler
                                },
                            new
                                {
                                    placeholder = "QuestionsSubReport",
                                    reportName = "QuestionDetails.Test",
                                    action = questionsHandler
                                }
                        }.ToDictionary(
                            o => o.placeholder,
                            o =>
                            new KeyValuePair<string, SubreportProcessingEventHandler>(o.reportName, o.action));

                return this.GetResultReport(
                    sessionResults,
                    s => s.acUserModeId,
                    resultConverter,
                    type == "full" ? "SessionsReport" : "SessionsReportNoQuestionsNoParticipants",
                    outputName,
                    format,
                    subReports);
            }

            return null;
        }

        /// <summary>
        ///     The import of users from excel spread sheet.
        /// </summary>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        [HttpGet]
        [ActionName("import-users")]
        [CustomAuthorize(RedirectToLogin = true)]
        public virtual ActionResult ImportUsers()
        {
            return this.View(
                EdugameCloudT4.File.Views.ImportUsers, 
                new ImportUsersViewModel(this.companyModel.GetAll(), this));
        }

        /// <summary>
        /// Get full file data.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        [HttpPost]
        [ValidateInput(false)]
        [OutputCache(Duration = 0, NoStore = true, Location = OutputCacheLocation.None)]
        [ActionName("import-users")]
        [CustomAuthorize(RedirectToLogin = true)]
        public virtual ActionResult ImportUsers(ImportUsersViewModel model)
        {
            if (this.ModelState.IsValid)
            {
                Company company = this.companyModel.GetOneById(model.CompanyId).Value;
                List<string> failedRows;
                string error;
                this.userModel.UploadBatchOfUsers(
                    company, 
                    model.ProfilesFile.InputStream, 
                    this.GetFileType(model.ProfilesFile.FileName), 
                    out failedRows, 
                    out error, 
                    notifyViaRTMP: false);
                if (error != null || failedRows.Any())
                {
                    this.ModelState.AddModelError(
                        Lambda.Property<ImportUsersViewModel>(x => x.ProfilesFile), 
                        this.CombineErrors(error, failedRows));
                }
            }

            model.SetCompanies(this.companyModel.GetAll());
            model.SetController(this);
            return this.View(EdugameCloudT4.File.Views.ImportUsers, model);
        }

        #endregion

        #region Methods

        /// <summary>
        /// The convert answers.
        /// </summary>
        /// <param name="sessionResults">
        /// The session results.
        /// </param>
        /// <param name="session">
        /// The ac session.
        /// </param>
        /// <param name="questionId">
        /// The question id.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{Object}"/>.
        /// </returns>
        private IEnumerable<dynamic> ConvertAnswers(Dictionary<SurveySessionFromStoredProcedureDTO, SurveyResultDataDTO> sessionResults, SurveySessionFromStoredProcedureDTO session, int questionId)
        {
            var results = sessionResults[session];
            var activePlayers = results.players.Count(x => x.score > 0);
            var question = results.questions.FirstOrDefault(x => x.questionId == questionId);
            var distractors = question.Return(x => x.distractors.Return(d => d.ToList(), new List<DistractorDTO>()), new List<DistractorDTO>());
            var groupedAnswers = results.players.SelectMany(p => p.answers.Where(x => x.questionId == questionId).Select(x => new { player = p, answer = x, answerString = this.ConvertAnswer(x, question, distractors) }))
                .GroupBy(x => x.answerString).ToDictionary(x => x.Key, x => x.Select(a => a)).ToList();
            return
                groupedAnswers.Select(
                    ga =>
                    new
                        {
                            result = ga.Value.Count(),
                            resultPercent = ((double)ga.Value.Count() / activePlayers).ToString("0.0%"),
                            totalResults = groupedAnswers.Count,
                            answer = ga.Key
                        });
        }

        /// <summary>
        /// The convert answer.
        /// </summary>
        /// <param name="answerDTO">
        /// The answer DTO.
        /// </param>
        /// <param name="question">
        /// The question.
        /// </param>
        /// <param name="distractors">
        /// The distractors.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string ConvertAnswer(SurveyQuestionResultAnswerDTO answerDTO, QuestionForAdminDTO question, List<DistractorDTO> distractors)
        {
            string result;
            var modifiedAnswer =
                new
                    {
                        answer = answerDTO,
                        disQuestion = distractors.FirstOrDefault(d => d.distractorId == answerDTO.surveyDistractorId),
                        disAnswer = distractors.FirstOrDefault(d => d.distractorId == answerDTO.surveyDistractorAnswerId)
                    };
            if (question.questionTypeId == (int)QuestionTypeEnum.RateScaleLikert && modifiedAnswer.disQuestion != null)
            {
                result = string.Format("{0} : {1}", modifiedAnswer.disQuestion.distractor, modifiedAnswer.disAnswer.Return(x => x.distractor, modifiedAnswer.answer.value ?? string.Empty));
            }
            else if (question.questionTypeId == (int)QuestionTypeEnum.WeightedBucketRatio && modifiedAnswer.disQuestion != null)
            {
                result = string.Format("{0} : {1}", modifiedAnswer.disQuestion.distractor, modifiedAnswer.answer.value);
            }
            else if (question.questionTypeId == (int)QuestionTypeEnum.TrueFalse)
            {
                result = modifiedAnswer.answer.value == "1" ? "True" : "False";
            }
            else
            {
                result = modifiedAnswer.answer.value;
            }

            return result;
        }

        /// <summary>
        /// The convert answers for participants.
        /// </summary>
        /// <param name="sessionResults">
        /// The session results.
        /// </param>
        /// <param name="session">
        /// The AC session.
        /// </param>
        /// <param name="questionId">
        /// The question id.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{Object}"/>.
        /// </returns>
        private IEnumerable<dynamic> ConvertAnswersForParticipants(Dictionary<SurveySessionFromStoredProcedureDTO, SurveyResultDataDTO> sessionResults, SurveySessionFromStoredProcedureDTO session, int questionId)
        {
            var results = sessionResults[session];
            var question = results.questions.FirstOrDefault(x => x.questionId == questionId);
            var distractors = question.Return(x => x.distractors.Return(d => d.ToList(), new List<DistractorDTO>()), new List<DistractorDTO>());
            return
                results.players.SelectMany(
                    p =>
                    p.answers.Where(x => x.questionId == questionId)
                        .GroupBy(x => p)
                        .ToDictionary(x => x.Key, g => g.Select(q => q))
                        .ToList()
                        .Select(
                            a =>
                            new
                                {
                                    player = a.Key,
                                    answer = this.GetParticipantAnswers(a.Value.ToList(), question, distractors)
                                }));
        }

        /// <summary>
        /// The get participant answers.
        /// </summary>
        /// <param name="answers">
        /// The answers.
        /// </param>
        /// <param name="question">
        /// The question.
        /// </param>
        /// <param name="distractors">
        /// The distractors.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string GetParticipantAnswers(List<SurveyQuestionResultAnswerDTO> answers, QuestionForAdminDTO question, List<DistractorDTO> distractors)
        {
            string result = string.Empty;
            var modifiedAnswers =
                answers.Select(
                    a =>
                    new
                    {
                        answer = a,
                        disQuestion = distractors.FirstOrDefault(d => d.distractorId == a.surveyDistractorId),
                        disAnswer = distractors.FirstOrDefault(d => d.distractorId == a.surveyDistractorAnswerId)
                    });
            if (answers.Any())
            {
                if (question.questionTypeId == (int)QuestionTypeEnum.RateScaleLikert)
                {
                    modifiedAnswers = modifiedAnswers.Where(x => x.disQuestion != null);
                    result = modifiedAnswers.Aggregate(result, (current, modifiedAnswer) => current + string.Format("{0} : {1}, ", modifiedAnswer.disQuestion.distractor, modifiedAnswer.disAnswer.Return(x => x.distractor, modifiedAnswer.answer.value ?? string.Empty)));
                    result = result.TrimEnd(", ".ToCharArray());
                }
                else if (question.questionTypeId == (int)QuestionTypeEnum.WeightedBucketRatio)
                {
                    modifiedAnswers = modifiedAnswers.Where(x => x.disQuestion != null);
                    result = modifiedAnswers.Aggregate(result, (current, modifiedAnswer) => current + string.Format("{0} : {1}, ", modifiedAnswer.disQuestion.distractor, modifiedAnswer.answer.value));
                    result = result.TrimEnd(", ".ToCharArray());
                }
                else if (question.questionTypeId == (int)QuestionTypeEnum.TrueFalse)
                {
                    result = modifiedAnswers.Aggregate(result, (current, modifiedAnswer) => current + string.Format("{0}, ", modifiedAnswer.answer.value == "1" ? "True" : "False"));
                    result = result.TrimEnd(", ".ToCharArray());
                }
                else
                {
                    result = modifiedAnswers.Aggregate(result, (current, modifiedAnswer) => current + string.Format("{0}, ", modifiedAnswer.answer.value));
                    result = result.TrimEnd(", ".ToCharArray());
                }
            }

            return result;
        }

        /// <summary>
        /// The fill participant data.
        /// </summary>
        /// <param name="member">
        /// The member.
        /// </param>
        /// <returns>
        /// The list of dynamic objects.
        /// </returns>
        [SuppressMessage("StyleCop.CSharp.LayoutRules", "SA1501:StatementMustNotBeOnSingleLine", Justification = "Reviewed. Suppression is OK here."), SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1107:CodeMustNotContainMultipleStatementsOnOneLine", Justification = "Reviewed. Suppression is OK here."), NonAction]
        private dynamic FillProfileData(SNMember member)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(member.ParticipantProfile))
                {
                    dynamic p = member.ParticipantProfile.ToDynamic().profile;
                    var baseData = (p as IDictionary<string, object>).With(x => x["base"]) as IDictionary<string, object> ?? new Dictionary<string, object> { { "userID", string.Empty }, { "firstName", member.Participant }, { "lastName", string.Empty }, };
                    return
                        new
                            {
                                id = int.Parse(baseData["userID"].ToString()),
                                name = (baseData["firstName"] + " " + baseData["lastName"]).Trim(),
                                location = this.GetLocation(p.location),
                                imgUrl = this.GetImageUrl(p.social)
                            };
                }
            }
                // ReSharper disable once EmptyGeneralCatchClause
            catch (Exception) { }

            return new { id = 0, name = string.Empty, location = string.Empty, imgUrl = string.Empty };
        }

        /// <summary>
        /// The get location.
        /// </summary>
        /// <param name="location">
        /// The location.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        [NonAction]
        private string GetLocation(dynamic location)
        {
            string locationRes = string.Empty;
            if (location != null)
            {
                var dic = location as IDictionary<string, object>;
                if (dic != null)
                {
                    if (dic.ContainsKey("address") && !string.IsNullOrWhiteSpace(location.address))
                    {
                        locationRes += location.address + ", ";
                    }

                    if (dic.ContainsKey("city") && !string.IsNullOrWhiteSpace(location.city))
                    {
                        locationRes += location.city + ", ";
                    }

                    if (dic.ContainsKey("state") && !string.IsNullOrWhiteSpace(location.state))
                    {
                        locationRes += location.state + ", ";
                    }

                    if (dic.ContainsKey("country") && !string.IsNullOrWhiteSpace(location.country))
                    {
                        locationRes += location.country + ", ";
                    }

                    if (dic.ContainsKey("zip") && !string.IsNullOrWhiteSpace(location.zip))
                    {
                        locationRes += location.zip + ", ";
                    }
                }
            }

            return locationRes;
        }

        /// <summary>
        /// The get image url.
        /// </summary>
        /// <param name="social">
        /// The social.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        [NonAction]
        private string GetImageUrl(dynamic social)
        {
            string imageRes = string.Empty;
            if (social != null)
            {
                var dic = social as IDictionary<string, object>;
                if (dic != null && dic.ContainsKey("selectedImageSocialSource"))
                {
                    var source = (string)social.@selectedImageSocialSource;
                    if (!string.IsNullOrWhiteSpace(source) && dic.ContainsKey(source))
                    {
                        var sourceDic = dic[source] as IDictionary<string, object>;
                        if (sourceDic != null && sourceDic.ContainsKey("imgUrl"))
                        {
                            imageRes = (string)sourceDic["imgUrl"];
                        }
                    }
                }
            }

            return imageRes;
        }

        /// <summary>
        /// The convert participant.
        /// </summary>
        /// <param name="member">
        /// The member.
        /// </param>
        /// <returns>
        /// The dynamic object
        /// </returns>
        [NonAction]
        private dynamic ConvertParticipant(SNMember member)
        {
            return
                new
                {
                    member.Id,
                    member.IsBlocked,
                    member.ParticipantProfile,
                    member.Participant,
                    member.ACSessionId,
                    member.DateCreated,
                    ParsedProfile = this.FillProfileData(member)
                };
        }

        /// <summary>
        /// The fill message data.
        /// </summary>
        /// <param name="discussions">
        /// The discussions.
        /// </param>
        /// <returns>
        /// The list of dynamic objects.
        /// </returns>
        [NonAction]
        private List<dynamic> FillMessageData(Dictionary<int, SNGroupDiscussion> discussions)
        {
            var messageData = new List<dynamic>();
            foreach (SNGroupDiscussion d in discussions.Values)
            {
                try
                {
                    foreach (dynamic m in d.GroupDiscussionData.ToDynamic().messages.message)
                    {
                        var dic = m as IDictionary<string, object>;
                        if (dic != null)
                        {
                            messageData.Add(
                                new
                                    {
                                        d.ACSessionId,
                                        id = long.Parse(m.id),
                                        userName = (string)m.userName,
                                        text = (string)m.text,
                                        likes = int.Parse(m.likes),
                                        dislikes = int.Parse(m.dislikes),
                                        userId = dic.ContainsKey("userId") ? int.Parse(m.userId) : 0,
                                        reply =
                                            dic.ContainsKey("reply") && (string)dic["reply"] != "NaN" ? long.Parse(m.reply) : 0
                                    });
                        }
                    }
                }
                catch (Exception)
                {
                    dynamic m = d.GroupDiscussionData.ToDynamic().messages.message;
                    var dic = m as IDictionary<string, object>;
                    if (dic != null)
                    {
                        messageData.Add(
                            new
                                {
                                    d.ACSessionId,
                                    id = long.Parse(m.id),
                                    userName = (string)m.userName,
                                    text = (string)m.text,
                                    likes = int.Parse(m.likes),
                                    dislikes = int.Parse(m.dislikes),
                                    userId = dic.ContainsKey("userId") ? int.Parse(m.userId) : 0,
                                    reply = dic.ContainsKey("reply") && (string)dic["reply"] != "NaN" ? long.Parse(m.reply) : 0
                                });
                    }
                }
            }

            return messageData;
        }

        /// <summary>
        /// The combine errors.
        /// </summary>
        /// <param name="error">
        /// The error.
        /// </param>
        /// <param name="failedRows">
        /// The failed rows.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        [NonAction]
        private string CombineErrors(string error, List<string> failedRows)
        {
            string result = string.Empty;
            failedRows.Insert(0, error);
            foreach (string errorText in failedRows)
            {
                result += errorText + Environment.NewLine;
            }

            return result;
        }
        
        /// <summary>
        /// The get file type.
        /// </summary>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        [NonAction]
        private string GetFileType(string fileName)
        {
            try
            {
                // ReSharper disable once PossibleNullReferenceException
                string ext = Path.GetExtension(fileName).ToLower();
                if (ext.Contains("csv"))
                {
                    return "csv";
                }

                if (ext.Contains("xls"))
                {
                    return "xls";
                }

                if (ext.Contains("xlsx"))
                {
                    return "xlsx";
                }
            }
                // ReSharper disable once EmptyGeneralCatchClause
            catch (Exception)
            {
            }

            return "csv";
        }
        
        /// <summary>
        /// Gets Hint-word values from crossword's XML definition
        /// </summary>
        /// <param name="documentXml">
        /// Input xml document
        /// </param>
        /// <returns>
        /// The <see cref="List{KeyValuePair}"/>.
        /// </returns>
        [NonAction]
        private List<KeyValuePair<string, string>> ReadCrosswordDefinition(string documentXml)
        {
            XDocument doc = XDocument.Parse(documentXml);
            XElement questionsElement;
            if (doc.Root == null || (questionsElement = doc.Root.Element(XName.Get("questions"))) == null)
            {
                return new List<KeyValuePair<string, string>>();
            }

            var questions = from element in questionsElement.Elements(XName.Get("question"))
                            let qe = element.Element(XName.Get("subject"))
                            where qe != null
                            let ae = element.Element(XName.Get("answer"))
                            where ae != null
                            select
                                new
                                    {
                                        id = element.Attribute(XName.Get("id")).Value, 
                                        question = qe.Value, 
                                        answer = ae.Value
                                    };
            return
                questions.OrderBy(q => q.id)
                    .Select(q => new KeyValuePair<string, string>(q.question, q.answer))
                    .ToList();
        }
        
        private LmsUserSession GetSession(string key)
        {
            Guid uid;
            var session = Guid.TryParse(key, out uid) ? this.userSessionModel.GetByIdWithRelated(uid).Value : null;

            if (session == null)
            {
                this.RedirectToError("Session timed out.");
                return null;
            }

            return session;
        }

        private void RedirectToError(string errorText)
        {
            this.Response.Clear();
            this.Response.Write(string.Format("<h1>{0}</h1>", errorText));
            this.Response.End();
        }

        // lti reports
        //private IAdobeConnectProxy GetAdobeConnectProvider(ILmsLicense lmsCompany)
        //{
        //    IAdobeConnectProxy provider = null;
        //    if (lmsCompany != null)
        //    {
        //        provider = this.Session[string.Format(LtiSessionKeys.ProviderSessionKeyPattern, lmsCompany.Id)] as IAdobeConnectProxy;
        //        if (provider == null)
        //        {
        //            provider = adobeConnectAccountService.GetProvider(lmsCompany);
        //            this.SetAdobeConnectProvider(lmsCompany.Id, provider);
        //        }
        //    }

        //    return provider;
        //}
        
        //private void SetAdobeConnectProvider(int key, IAdobeConnectProxy acp)
        //{
        //    this.Session[string.Format(LtiSessionKeys.ProviderSessionKeyPattern, key)] = acp;
        //}
        
        #endregion

    }

}