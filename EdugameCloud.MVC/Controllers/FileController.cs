namespace EdugameCloud.MVC.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Mime;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.UI;
    using System.Xml.Linq;

    using EdugameCloud.Core.Business.Models;
    using EdugameCloud.Core.Domain.DTO;
    using EdugameCloud.Core.Domain.Entities;
    using EdugameCloud.MVC.ViewModels;
    using EdugameCloud.MVC.ViewResults;

    using Esynctraining.Core.Extensions;
    using Esynctraining.Core.Providers;
    using Esynctraining.Core.Utils;

    using Microsoft.Reporting.WebForms;

    using NHibernate.Util;

    using File = EdugameCloud.Core.Domain.Entities.File;

    /// <summary>
    ///     The product controller.
    /// </summary>
    [HandleError]
    public partial class FileController : BaseController
    {
        #region Fields

        /// <summary>
        ///     The company model.
        /// </summary>
        private readonly CompanyModel companyModel;

        /// <summary>
        ///     The product model.
        /// </summary>
        private readonly FileModel fileModel;

        /// <summary>
        ///     The group discussion model.
        /// </summary>
        private readonly SNGroupDiscussionModel groupDiscussionModel;

        /// <summary>
        ///     The user model.
        /// </summary>
        private readonly UserModel userModel;

        /// <summary>
        /// The survey result model.
        /// </summary>
        private readonly SurveyResultModel surveyResultModel;

        /// <summary>
        /// The AC session model.
        /// </summary>
        private readonly ACSessionModel sessionModel;

        /// <summary>
        /// The AC user mode model.
        /// </summary>
        private readonly ACUserModeModel userModeModel;

        /// <summary>
        ///     Gets the VCF model.
        /// </summary>
        private readonly VCFModel vcfModel;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FileController"/> class.
        /// </summary>
        /// <param name="fileModel">
        /// The file Model.
        /// </param>
        /// <param name="vcfModel">
        /// The VCF Model.
        /// </param>
        /// <param name="companyModel">
        /// The company Model.
        /// </param>
        /// <param name="groupDiscussionModel">
        /// the group discussion model
        /// </param>
        /// <param name="userModel">
        /// The user Model.
        /// </param>
        /// <param name="surveyResultModel">
        /// The survey Result Model.
        /// </param>
        /// <param name="sessionModel">
        /// The ac Session Model.
        /// </param>
        /// <param name="userModeModel">
        /// The ac User Mode Model.
        /// </param>
        /// <param name="settings">
        /// The settings
        /// </param>
        public FileController(
            FileModel fileModel, 
            VCFModel vcfModel, 
            CompanyModel companyModel, 
            SNGroupDiscussionModel groupDiscussionModel, 
            UserModel userModel, 
            SurveyResultModel surveyResultModel,
            ACSessionModel sessionModel,
            ACUserModeModel userModeModel,
            ApplicationSettingsProvider settings)
            : base(settings)
        {
            this.fileModel = fileModel;
            this.vcfModel = vcfModel;
            this.companyModel = companyModel;
            this.groupDiscussionModel = groupDiscussionModel;
            this.userModel = userModel;
            this.surveyResultModel = surveyResultModel;
            this.sessionModel = sessionModel;
            this.userModeModel = userModeModel;
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
        public virtual ActionResult ExportQuestions(string id)
        {
            var storagePath = Settings.FileStorage as string;
            var exportSubPath = Settings.ExportSubPath as string;
            var fileName = id + ".xml";

            if (storagePath == null || exportSubPath == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            var filePath = Path.Combine(storagePath, exportSubPath, fileName);
            if (!System.IO.File.Exists(filePath))
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            var isSuccess = true;
            var dataArray = new byte[0];
            try
            {
                dataArray = System.IO.File.ReadAllBytes(filePath);
                System.IO.File.Delete(filePath);
            }
            catch
            {
                isSuccess = false;
            }
            
            if (!isSuccess)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }

            return new FileContentResult(dataArray, MediaTypeNames.Text.Xml);
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
        public virtual ActionResult ImportQuestions(ImportQuestionsViewModel model)
        {
            var storagePath = Settings.FileStorage as string;
            var importSubPath = Settings.ImportSubPath as string;
            var id = Guid.NewGuid().ToString();
            var fileName = id + ".xml";

            if (storagePath == null || importSubPath == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            var isSuccess = true;

            var dataArray = model.XmlToImport;
            var filePath = Path.Combine(storagePath, importSubPath, fileName);
            try
            {
                System.IO.File.WriteAllText(filePath, dataArray);
            }
            catch
            {
                isSuccess = false;
            }

            if (!isSuccess)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }

            return new ContentResultWithStatusCode(HttpStatusCode.OK) { Content = id };
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
            var storagePath = Settings.FileStorage as string;
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
                var isSuccess = false;
                try
                {
                    foreach (string file in Request.Files)
                    {
                        var hpf = Request.Files[file] as HttpPostedFileBase;
                        if (hpf.ContentLength == 0)
                        {
                            continue;
                        }

                        hpf.SaveAs(filePath);
                        isSuccess = true;
                        break;
                    }
                }
                catch (Exception)
                {
                   isSuccess = false;
                }

                if (!isSuccess)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
                }
            }
            
            return new ContentResultWithStatusCode(HttpStatusCode.OK) { Content = id };
        }

        /// <summary>
        /// The import questions.
        /// </summary>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        [HttpGet]
        [ActionName("test-import-questions")]
//        [Authorize]
        public virtual ActionResult TestImportQuestions()
        {
            return this.View(EdugameCloudT4.File.Views.ImportQuestions, string.Empty);
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
////      [Authorize]
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
////        [Authorize]
        public virtual ActionResult GetCollaborationReport(int userId, int? sessionId, string format = "pdf")
        {
            User user = IoC.Resolve<UserModel>().GetOneById(userId).Value;
            string outputName = user.FullName.Replace(" ", "-") + "-collaboration-report-";

            List<SNSessionDTO> userSessions = this.sessionModel.GetSNSessionsByUserId(userId).ToList();
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

            Dictionary<int, SNSessionDTO> sessionsById = userSessions.ToDictionary(s => s.acSessionId, s => s);

            Dictionary<int, SNGroupDiscussion> discussions =
                IoC.Resolve<SNGroupDiscussionModel>()
                    .GetAllByACSessionIds(sessionsById.Keys.ToList())
                    .GroupBy(o => o.ACSessionId, o => o)
                    .ToDictionary(g => g.Key, g => g.FirstOrDefault());
            Dictionary<int, List<dynamic>> members =
                IoC.Resolve<SNMemberModel>()
                    .GetAllByACSessionIds(sessionsById.Keys.ToList()).Select(x => this.ConvertParticipant(x)).ToList()
                    .GroupBy(o => (int)o.ACSessionId, o => o)
                    .ToDictionary(g => g.Key, g => g.ToList());

            List<dynamic> messageData = this.FillMessageData(discussions);

            Dictionary<dynamic, List<dynamic>> messages = messageData.GroupBy(o => o.ACSessionId, o => o).ToDictionary(g => g.Key, g => g.ToList());

            var sessionResults = sessionsById.ToDictionary(kvp => kvp.Value, kvp => new { discussion = discussions.ContainsKey(kvp.Key) ? discussions[kvp.Key] : null, members = members.ContainsKey(kvp.Key) ? members[kvp.Key] : new List<dynamic>() });

            Func<SNSessionDTO, IDictionary<int, string>, object> resultConverter =
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
                    args.DataSources.Clear();
                    int acSessionId = int.Parse(args.Parameters["acSessionId"].Values.First());
                    SNSessionDTO acSession = sessionResults.Keys.First(s => s.acSessionId == acSessionId);
                    List<dynamic> sessionMessages = messages.Return(map => map.ContainsKey(acSessionId) ? map[acSessionId] : new List<dynamic>(), new List<dynamic>());
                    var details =
                        new[] { acSession }.Select(
                            s =>
                            new
                                {
                                    acSessionId, 
                                    discussion = sessionResults[s].discussion.Return(d => d.GroupDiscussionTitle, string.Empty), 
                                    dateCreated = sessionResults[s].discussion.Return(d => d.DateCreated, DateTime.MinValue), 
                                    totalMessages = sessionMessages.Count, 
                                    totalLikes = sessionMessages.Sum(m => m.likes), 
                                    totalDislikes = sessionMessages.Sum(m => m.dislikes), 
                                    total = sessionResults[s].members.Count, 
                                    active = s.activeParticipants, 
                                }).ToList();
                    args.DataSources.Add(new ReportDataSource("ItemDataSet", details));
                };

            SubreportProcessingEventHandler participantsHandler = (sender, args) =>
                {
                    args.DataSources.Clear();
                    int acSessionId = int.Parse(args.Parameters["acSessionId"].Values.First());
                    SNSessionDTO acSession = sessionResults.Keys.First(s => s.acSessionId == acSessionId);
                    List<dynamic> sessionMessages = messages.Return(map => map.ContainsKey(acSessionId) ? map[acSessionId] : new List<dynamic>(), new List<dynamic>());
                    var participants = sessionResults[acSession].members.Select(p => new 
                                     {
                                         acSessionId,
                                         profile = string.IsNullOrWhiteSpace(p.ParticipantProfile) ? null : Url.ActionAbsolute(EdugameCloudT4.File.GetProfileVCard(acSessionId, (int)p.Id)),
                                         participant = p.Participant,
                                         totalMessages = (int)p.ParsedProfile.id != 0 ? sessionMessages.Count(m => (int)m.userId == (int)p.ParsedProfile.id) : sessionMessages.Count(m => m.userName == p.Participant || m.userName == p.ParsedProfile.name),
                                         totalLikes = (int)p.ParsedProfile.id != 0 ? sessionMessages.Where(m => (int)m.userId == (int)p.ParsedProfile.id).Sum(m => m.likes) : sessionMessages.Where(m => m.userName == p.Participant || m.userName == p.ParsedProfile.name).Sum(m => m.likes), 
                                         totalDislikes = (int)p.ParsedProfile.id != 0 ? sessionMessages.Where(m => (int)m.userId == (int)p.ParsedProfile.id).Sum(m => m.dislikes) : sessionMessages.Where(m => m.userName == p.Participant || m.userName == p.ParsedProfile.name).Sum(m => m.dislikes),  
                                     }).ToList();
                    args.DataSources.Add(new ReportDataSource("ItemDataSet", participants));
                };

            SubreportProcessingEventHandler messagesHandler = (sender, args) =>
                {
                    args.DataSources.Clear();
                    int acSessionId = int.Parse(args.Parameters["acSessionId"].Values.First());
                    List<dynamic> sessionMessages = messages.Return(map => map.ContainsKey(acSessionId) ? map[acSessionId] : new List<dynamic>(), new List<dynamic>());

                    var messageList = sessionMessages.Select(m => new { acSessionId, m.text, participant = m.userName, m.likes, m.dislikes, }).ToList();
                    args.DataSources.Add(new ReportDataSource("ItemDataSet", messageList));
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
                        o => new KeyValuePair<string, SubreportProcessingEventHandler>(o.reportName, o.action));

            return this.GetResultReport(
                sessionResults, 
                s => s.acUserModeId, 
                resultConverter, 
                "SessionsReport", 
                outputName, 
                format, 
                subReports);
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
////        [Authorize]
        public virtual ActionResult GetCrosswordsReport(int userId, int? sessionId, string format = "pdf")
        {
            User user = IoC.Resolve<UserModel>().GetOneById(userId).Value;
            string outputName = user.FullName.Replace(" ", "-") + "-crossword-report-";
            var appletItemModel = IoC.Resolve<AppletItemModel>();
            List<CrosswordSessionDTO> userSessions = appletItemModel.GetCrosswordSessionsByUserId(userId).ToList();
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

            Dictionary<CrosswordSessionDTO, List<CrosswordResultByAcSessionDTO>> sessionResults =
                userSessions.ToDictionary(
                    s => s, 
                    s => appletItemModel.GetCrosswordResultByACSessionId(s.acSessionId).ToList());

            Dictionary<CrosswordSessionDTO, List<KeyValuePair<string, string>>> crosswords =
                sessionResults.ToDictionary(s => s.Key, s => this.ReadCrosswordDefinition(s.Value.First().documentXML));

            Func<CrosswordSessionDTO, IDictionary<int, string>, object> resultConverter =
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
                    CrosswordSessionDTO acSession = sessionResults.Keys.First(s => s.acSessionId == acSessionId);
                    var details =
                        new[] { acSession }.Select(
                            s =>
                            new
                                {
                                    acSessionId, 
                                    total = s.totalParticipants, 
                                    active = s.activeParticipants, 
                                    averageScore = sessionResults[s].Average(p => p.score), 
                                    averageTime = (long)sessionResults[s].Average(p => (p.endTime - p.startTime).Ticks)
                                })
                            .ToList();
                    args.DataSources.Add(new ReportDataSource("ItemDataSet", details));
                };

            SubreportProcessingEventHandler participantsHandler = (sender, args) =>
                {
                    args.DataSources.Clear();
                    int acSessionId = int.Parse(args.Parameters["acSessionId"].Values.First());
                    CrosswordSessionDTO acSession = sessionResults.Keys.First(s => s.acSessionId == acSessionId);
                    var questions = crosswords[acSession];
                    var participants = sessionResults[acSession].Select(p => new
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
                    CrosswordSessionDTO acSession = sessionResults.Keys.First(s => s.acSessionId == acSessionId);
                    var count = crosswords[acSession].Count;
                    var questions = crosswords[acSession].Select(q => new { acSessionId, question = q.Key, answer = q.Value, totalQuestions = count }).ToList();
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
                        o => new KeyValuePair<string, SubreportProcessingEventHandler>(o.reportName, o.action));

            return this.GetResultReport(
                sessionResults, 
                s => s.acUserModeId, 
                resultConverter, 
                "SessionsReport", 
                outputName, 
                format, 
                subReports);
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
        [ActionName("get")]
        public virtual ActionResult GetFile(string id)
        {
            File file = null;
            Guid webOrbId;
            int idVal;
            if (Guid.TryParse(id, out webOrbId))
            {
                file = this.fileModel.GetOneByWebOrbId(webOrbId).Value;
            }
            else if (int.TryParse(id, out idVal))
            {
                file = this.fileModel.GetOneById(idVal).Value;
            }

            if (file != null)
            {
                byte[] buffer = this.fileModel.GetData(file);
                if (buffer != null)
                {
                    return this.File(buffer, file.FileName.GetContentTypeByExtension(), file.FileName);
                }
            }

            return new HttpStatusCodeResult(HttpStatusCode.NotFound);
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
////        [Authorize]
        public virtual ActionResult GetGroupDiscussion(int id)
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
////        [Authorize]
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
////        [Authorize]
        public virtual ActionResult GetQuizReport(int userId, int? sessionId, string format = "pdf", string type = "full")
        {
            type = (string.IsNullOrWhiteSpace(type) ? "full" : type).ToLower();
            User user = IoC.Resolve<UserModel>().GetOneById(userId).Value;
            string outputName = user.FullName.Replace(" ", "-") + "-quiz-report-";
            List<QuizSessionDTO> userSessions = this.sessionModel.GetQuizSessionsByUserId(userId).ToList();
            if (sessionId.HasValue)
            {
                userSessions = userSessions.Where(us => us.acSessionId == sessionId.Value).ToList();
                outputName += userSessions.FirstOrDefault().Return(x => x.dateCreated, DateTime.Today).ToString("MM-dd-yyyy");
            }
            else
            {
                outputName += DateTime.Today.ToString("MM-dd-yyyy");
            }

            Dictionary<QuizSessionDTO, QuizResultDataDTO> sessionResults = userSessions.ToDictionary(
                s => s, 
                s => IoC.Resolve<QuizResultModel>().GetQuizResultByACSessionId(s.acSessionId, s.subModuleItemId));

            Func<QuizSessionDTO, IDictionary<int, string>, object> resultConverter =
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
                    QuizSessionDTO acSession = sessionResults.Keys.First(s => s.acSessionId == acSessionId);
                    var details =
                        new[] { acSession }.Select(
                            s =>
                            new
                                {
                                    acSessionId, 
                                    totalQuestions = s.TotalQuestion, 
                                    s.TotalScore, 
                                    total = s.totalParticipants, 
                                    active = s.activeParticipants, 
                                    averageScore = sessionResults[s].players.Where(p => p.score > 0).Average(p => p.score), 
                                    averageTime = (long)sessionResults[s].players.Where(p => p.score > 0).Average(p => (p.endTime - p.startTime).Ticks)
                                })
                            .ToList();
                    args.DataSources.Add(new ReportDataSource("ItemDataSet", details));
                };

            SubreportProcessingEventHandler participantsHandler = (sender, args) =>
                {
                    args.DataSources.Clear();
                    int acSessionId = int.Parse(args.Parameters["acSessionId"].Values.First());
                    QuizSessionDTO acSession = sessionResults.Keys.First(s => s.acSessionId == acSessionId);
                    var questions = sessionResults[acSession].questions;
                    var participants = sessionResults[acSession].players.Select(p => new
                                                                                         {
                                                                                             acSessionId, 
                                                                                             rank = p.position, 
                                                                                             p.score, 
                                                                                             startTime = p.score > 0 ? p.startTime : DateTime.Today, 
                                                                                             endTime = p.score > 0 ? p.endTime : DateTime.Today, 
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
                    QuizSessionDTO acSession = sessionResults.Keys.First(s => s.acSessionId == acSessionId);
                    var questions =
                        sessionResults[acSession].questions.Select(
                            q =>
                            new
                                {
                                    acSessionId, 
                                    q.question, 
                                    questionType = q.questionTypeName, 
                                    q.isMandatory,
                                    totalCorrect = q.CorrectAnswerCount,
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
                        o => new KeyValuePair<string, SubreportProcessingEventHandler>(o.reportName, o.action));

            return this.GetResultReport(
                sessionResults, 
                s => s.acUserModeId, 
                resultConverter,
                type == "full" ? "SessionsReport" : "SessionsReportNoQuestionsNoParticipants", 
                outputName, 
                format, 
                subReports);
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
        [ActionName("survey-report")]
////        [Authorize]
        public virtual ActionResult GetSurveyReport(int userId, int? sessionId, string format = "pdf", string type = "full")
        {
            type = (string.IsNullOrWhiteSpace(type) ? "full" : type).ToLower();
            User user = IoC.Resolve<UserModel>().GetOneById(userId).Value;
            string outputName = user.FullName.Replace(" ", "-") + "-survey-report-";

            List<SurveySessionDTO> userSessions = this.sessionModel.GetSurveySessionsByUserId(userId).ToList();
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

            var sessionResults = userSessions.ToDictionary(s => s, s => this.surveyResultModel.GetSurveyResultByACSessionId(s.acSessionId, s.subModuleItemId));

            Func<SurveySessionDTO, IDictionary<int, string>, object> resultConverter =
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

            SubreportProcessingEventHandler detailsHandler = (sender, args) =>
                {
                    args.DataSources.Clear();
                    int acSessionId = int.Parse(args.Parameters["acSessionId"].Values.First());
                    SurveySessionDTO acSession = sessionResults.Keys.First(s => s.acSessionId == acSessionId);
                    var details =
                        new[] { acSession }.Select(
                            s =>
                            new
                                {
                                    acSessionId, 
                                    totalQuestions = s.TotalQuestion, 
                                    s.TotalScore, 
                                    total = sessionResults[s].players.Count, 
                                    active = s.activeParticipants, 
                                    averageScore = sessionResults[s].players.Average(p => p.score), 
                                    averageTime =
                                (long)sessionResults[s].players.Average(p => (p.endTime - p.startTime).Ticks)
                                })
                            .ToList();
                    args.DataSources.Add(new ReportDataSource("ItemDataSet", details));
                };

            SubreportProcessingEventHandler participantsHandler = (sender, args) =>
                {
                    args.DataSources.Clear();
                    int acSessionId = int.Parse(args.Parameters["acSessionId"].Values.First());
                    SurveySessionDTO acSession = sessionResults.Keys.First(s => s.acSessionId == acSessionId);
                    var participants =
                        sessionResults[acSession].players.Select(
                            p =>
                            new
                                {
                                    acSessionId,
                                    rank = p.position,
                                    p.score,
                                    p.startTime,
                                    p.endTime,
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
                    SurveySessionDTO acSession = sessionResults.Keys.First(s => s.acSessionId == acSessionId);
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
                                    totalCorrect = q.CorrectAnswerCount,
                                    totalQuestions = acSession.TotalQuestion
                                }).ToList();
                    args.DataSources.Add(new ReportDataSource("ItemDataSet", questions));
                };

            SubreportProcessingEventHandler answersStatisticsHandler = (sender, args) =>
            {
                args.DataSources.Clear();
                int acSessionId = int.Parse(args.Parameters["acSessionId"].Values.First());
                int questionId = int.Parse(args.Parameters["questionId"].Values.First());
                SurveySessionDTO acSession = sessionResults.Keys.First(s => s.acSessionId == acSessionId);
                var results = this.СonvertAnswers(sessionResults, acSession, questionId).Select(
                        pa =>
                        new
                        {
                            resultId = pa.player.surveyResultId,
                            result = pa.player.score,
                            resultPercent = ((double)pa.player.score / acSession.TotalQuestion).ToString("0.0%"),
                            totalResults = sessionResults[acSession].players.Count,
                            answer = pa.answer.value
                        }).ToList();
                args.DataSources.Add(new ReportDataSource("ItemDataSet", results));
            };

            SubreportProcessingEventHandler participantsStatisticsHandler = (sender, args) =>
            {
                args.DataSources.Clear();
                int acSessionId = int.Parse(args.Parameters["acSessionId"].Values.First());
                int questionId = int.Parse(args.Parameters["questionId"].Values.First());
                SurveySessionDTO acSession = sessionResults.Keys.First(s => s.acSessionId == acSessionId);
                var results = sessionResults[acSession].players.SelectMany(p => p.answers.Where(x => x.questionId == questionId).Select(a => new { player = p, answer = a })).Select(
                        pa =>
                        new
                        {
                            participantId = pa.player.surveyResultId,
                            participant = pa.player.participantName,
                            answer = pa.answer.value
                        }).ToList();
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
                    }.ToDictionary(o => o.placeholder, o => new KeyValuePair<string, SubreportProcessingEventHandler>(o.reportName, o.action));

            return this.GetResultReport(
                sessionResults, 
                s => s.acUserModeId, 
                resultConverter, 
                type == "full" ? "SessionsReport" : "SessionsReportNoQuestionsNoParticipants", 
                outputName, 
                format, 
                subReports);
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
        [ActionName("test-report")]
////        [Authorize]
        public virtual ActionResult GetTestReport(int userId, int? sessionId, string format = "pdf", string type = "full")
        {
            type = (string.IsNullOrWhiteSpace(type) ? "full" : type).ToLower();
            User user = IoC.Resolve<UserModel>().GetOneById(userId).Value;
            string outputName = user.FullName.Replace(" ", "-") + "-test-report-";

            List<TestSessionDTO> userSessions = this.sessionModel.GetTestSessionsByUserId(userId).ToList();
            if (sessionId.HasValue)
            {
                userSessions = userSessions.Where(us => us.acSessionId == sessionId.Value).ToList();
                outputName += userSessions.FirstOrDefault().Return(x => x.dateCreated, DateTime.Today).ToString("MM-dd-yyyy");
            }
            else
            {
                outputName += DateTime.Today.ToString("MM-dd-yyyy");
            }

            var testResultModel = IoC.Resolve<TestResultModel>();
            var sessionResults = userSessions.ToDictionary(s => s, s => testResultModel.GetTestResultByACSessionId(s.acSessionId, s.subModuleItemId));

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
                        s.dateCreated, 
                    };

            SubreportProcessingEventHandler detailsHandler = (sender, args) =>
                {
                    args.DataSources.Clear();
                    int acSessionId = int.Parse(args.Parameters["acSessionId"].Values.First());
                    TestSessionDTO acSession = sessionResults.Keys.First(s => s.acSessionId == acSessionId);
                    
                    var passingScore = tests[acSession].PassingScore.HasValue ? tests[acSession].PassingScore.Value : 0;
                    var questions = sessionResults[acSession].questions;
                    var details =
                        new[] { acSession }.Select(
                            s =>
                            new
                                {
                                    acSessionId,
                                    totalQuestions = questions.Count, 
                                    totalScore = s.TotalScore, 
                                    total = s.totalParticipants, 
                                    active = s.activeParticipants, 
                                    averageScore = acSession.avgScore,
                                    averageScorePercent = (acSession.avgScore / questions.Count).ToString("0.0%"),
                                    averageTime = (long)sessionResults[s].players.Average(p => (p.endTime - p.startTime).Ticks),
                                    passingScore,
                                    passingScorePercent = ((double)passingScore / 100).ToString("0.0%"),
                                    timeLimit = tests[s].TimeLimit.HasValue ? tests[s].TimeLimit.Value : 0, 
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
                                return new
                                        {
                                            acSessionId, 
                                            rank = p.position,
                                            p.score, 
                                            p.startTime, 
                                            p.endTime, 
                                            p.participantName, 
                                            totalScore = acSession.TotalScore,
                                            scorePassed = p.scorePassed ? 1 : 0, 
                                            timePassed = p.timePassed ? 1 : 0,
                                            totalQuestions = questions.Count,
                                            scorePercent = ((double)p.score / questions.Count).ToString("0.0%")
                                        };
                            }).ToList();
                    args.DataSources.Add(new ReportDataSource("ItemDataSet", participants));
                };

            SubreportProcessingEventHandler questionsHandler = (sender, args) =>
                {
                    args.DataSources.Clear();
                    int acSessionId = int.Parse(args.Parameters["acSessionId"].Values.First());
                    TestSessionDTO acSession = sessionResults.Keys.First(s => s.acSessionId == acSessionId);
                    var preQuestions = sessionResults[acSession].questions;
                    var questions = preQuestions.Select(
                            q =>
                            new
                                {
                                    acSessionId, 
                                    q.question, 
                                    questionType = q.questionTypeName, 
                                    q.isMandatory, 
                                    totalCorrect = q.CorrectAnswerCount, 
                                    q.restrictions,
                                    totalQuestions = preQuestions.Count
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
                        o => new KeyValuePair<string, SubreportProcessingEventHandler>(o.reportName, o.action));

            return this.GetResultReport(
                sessionResults, 
                s => s.acUserModeId, 
                resultConverter,
                type == "full" ? "SessionsReport" : "SessionsReportNoQuestionsNoParticipants", 
                outputName, 
                format, 
                subReports);
        }

        /// <summary>
        ///     The import of users from excel spread sheet.
        /// </summary>
        /// <returns>
        ///     The <see cref="ActionResult" />.
        /// </returns>
        [HttpGet]
        [ActionName("import-users")]
        [Authorize]
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
////        [Authorize]
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

        private IEnumerable<dynamic> СonvertAnswers(Dictionary<SurveySessionDTO, SurveyResultDataDTO> sessionResults, SurveySessionDTO acSession, int questionId)
        {
            var answers = sessionResults[acSession].players.SelectMany(p => p.answers.Where(x => x.questionId == questionId).Select(a => new { player = p, answer = a })).ToList();
//            foreach (var ap in answers)
//            {
//                if (ap.answer.questionTypeId == QuestionType.)
//            }

            return answers;
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
            catch (Exception)
            {
            }

            return "csv";
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
            string format = "pdf", 
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
                        if (subReports.ContainsKey(placeholder))
                        {
                            subReports[placeholder].Value(sender, args);
                        }
                    };
            }

            localReport.DataSources.Clear();

            Dictionary<int, string> userModes =
                this.userModeModel.GetAllByIds(sessionResults.Keys.Select(userModeIdSelector).Distinct().ToList())
                    .ToDictionary(m => m.Id, m => m.UserMode);

            List<object> results = (from s in sessionResults.Keys select resultConverter(s, userModes)).ToList();

            localReport.DataSources.Add(new ReportDataSource("ItemDataSet", results));
            string encoding;
            string fileNameExtension;

            Warning[] warnings;
            string[] streams;
            byte[] renderedBytes = localReport.Render(
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

        /// <summary>
        /// Gets hint-word values from crossword's XML definition
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

        /// <summary>
        ///     The supported report formats.
        /// </summary>
        /// <returns>
        ///     The <see cref="List{String}" />.
        /// </returns>
        [NonAction]
        private List<string> SupportedReportFormats()
        {
            return new List<string> { "IMAGE", "PDF", "EXCEL" };
        }

        #endregion
    }
}