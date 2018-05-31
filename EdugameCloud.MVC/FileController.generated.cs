// <auto-generated />
// This file was generated by a T4 template.
// Don't change it directly as your change would get overwritten.  Instead, make changes
// to the .tt file (i.e. the T4 template) and save it to regenerate this file.

// Make sure the compiler doesn't complain about missing Xml comments
#pragma warning disable 1591
#region T4MVC

using System;
using System.Diagnostics;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using System.Web.Mvc.Html;
using System.Web.Routing;
using EdugameCloud.MVC;
namespace EdugameCloud.MVC.Controllers
{
    public partial class FileController
    {
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        protected FileController(Dummy d) { }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        protected RedirectToRouteResult RedirectToAction(ActionResult result)
        {
            var callInfo = result.GetT4MVCResult();
            return RedirectToRoute(callInfo.RouteValueDictionary);
        }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        protected RedirectToRouteResult RedirectToActionPermanent(ActionResult result)
        {
            var callInfo = result.GetT4MVCResult();
            return RedirectToRoutePermanent(callInfo.RouteValueDictionary);
        }

        [NonAction]
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public System.Web.Mvc.ActionResult ExportQuestions()
        {
            return new T4MVC_ActionResult(Area, Name, ActionNames.ExportQuestions);
        }
        [NonAction]
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public System.Web.Mvc.ActionResult GetCollaborationReport()
        {
            return new T4MVC_ActionResult(Area, Name, ActionNames.GetCollaborationReport);
        }
        [NonAction]
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public System.Web.Mvc.ActionResult GetCrosswordsReport()
        {
            return new T4MVC_ActionResult(Area, Name, ActionNames.GetCrosswordsReport);
        }
        [NonAction]
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public System.Web.Mvc.ActionResult GetGroupDiscussion()
        {
            return new T4MVC_ActionResult(Area, Name, ActionNames.GetGroupDiscussion);
        }
        [NonAction]
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public System.Web.Mvc.ActionResult GetProfileVCard()
        {
            return new T4MVC_ActionResult(Area, Name, ActionNames.GetProfileVCard);
        }
        [NonAction]
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public System.Web.Mvc.ActionResult GetQuizReport()
        {
            return new T4MVC_ActionResult(Area, Name, ActionNames.GetQuizReport);
        }
        [NonAction]
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public System.Web.Mvc.ActionResult GetSurveyReport()
        {
            return new T4MVC_ActionResult(Area, Name, ActionNames.GetSurveyReport);
        }
        [NonAction]
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public System.Web.Mvc.ActionResult GetTestReport()
        {
            return new T4MVC_ActionResult(Area, Name, ActionNames.GetTestReport);
        }
        [NonAction]
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public System.Web.Mvc.ActionResult MeetingHostReport()
        {
            return new T4MVC_ActionResult(Area, Name, ActionNames.MeetingHostReport);
        }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public FileController Actions { get { return EdugameCloudT4.File; } }
        [GeneratedCode("T4MVC", "2.0")]
        public readonly string Area = "";
        [GeneratedCode("T4MVC", "2.0")]
        public readonly string Name = "File";
        [GeneratedCode("T4MVC", "2.0")]
        public const string NameConst = "File";

        static readonly ActionNamesClass s_actions = new ActionNamesClass();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionNamesClass ActionNames { get { return s_actions; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionNamesClass
        {
            public readonly string ExportQuestions = "export-questions";
            public readonly string ImportQuestions = "import-questions-from-xml";
            public readonly string TestImportQuestions = "test-import-questions";
            public readonly string ConvertToVCF = "toVcf";
            public readonly string GetCollaborationReport = "collaboration-report";
            public readonly string GetCrosswordsReport = "crossword-report";
            public readonly string GetGroupDiscussion = "get-group-discussion";
            public readonly string GetProfileVCard = "get-profile-vCard";
            public readonly string GetQuizReport = "quiz-report";
            public readonly string GetSurveyReport = "survey-report";
            public readonly string GetTestReport = "test-report";
            public readonly string ImportUsers = "import-users";
            public readonly string MeetingHostReport = "meeting-host-report";
        }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionNameConstants
        {
            public const string ExportQuestions = "export-questions";
            public const string ImportQuestions = "import-questions-from-xml";
            public const string TestImportQuestions = "test-import-questions";
            public const string ConvertToVCF = "toVcf";
            public const string GetCollaborationReport = "collaboration-report";
            public const string GetCrosswordsReport = "crossword-report";
            public const string GetGroupDiscussion = "get-group-discussion";
            public const string GetProfileVCard = "get-profile-vCard";
            public const string GetQuizReport = "quiz-report";
            public const string GetSurveyReport = "survey-report";
            public const string GetTestReport = "test-report";
            public const string ImportUsers = "import-users";
            public const string MeetingHostReport = "meeting-host-report";
        }


        static readonly ActionParamsClass_ExportQuestions s_params_ExportQuestions = new ActionParamsClass_ExportQuestions();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionParamsClass_ExportQuestions ExportQuestionsParams { get { return s_params_ExportQuestions; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionParamsClass_ExportQuestions
        {
            public readonly string id = "id";
        }
        static readonly ActionParamsClass_ImportQuestions s_params_ImportQuestions = new ActionParamsClass_ImportQuestions();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionParamsClass_ImportQuestions ImportQuestionsParams { get { return s_params_ImportQuestions; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionParamsClass_ImportQuestions
        {
            public readonly string model = "model";
        }
        static readonly ActionParamsClass_ConvertToVCF s_params_ConvertToVCF = new ActionParamsClass_ConvertToVCF();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionParamsClass_ConvertToVCF ConvertToVCFParams { get { return s_params_ConvertToVCF; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionParamsClass_ConvertToVCF
        {
            public readonly string model = "model";
        }
        static readonly ActionParamsClass_GetCollaborationReport s_params_GetCollaborationReport = new ActionParamsClass_GetCollaborationReport();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionParamsClass_GetCollaborationReport GetCollaborationReportParams { get { return s_params_GetCollaborationReport; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionParamsClass_GetCollaborationReport
        {
            public readonly string userId = "userId";
            public readonly string sessionId = "sessionId";
            public readonly string format = "format";
        }
        static readonly ActionParamsClass_GetCrosswordsReport s_params_GetCrosswordsReport = new ActionParamsClass_GetCrosswordsReport();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionParamsClass_GetCrosswordsReport GetCrosswordsReportParams { get { return s_params_GetCrosswordsReport; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionParamsClass_GetCrosswordsReport
        {
            public readonly string userId = "userId";
            public readonly string sessionId = "sessionId";
            public readonly string format = "format";
        }
        static readonly ActionParamsClass_GetGroupDiscussion s_params_GetGroupDiscussion = new ActionParamsClass_GetGroupDiscussion();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionParamsClass_GetGroupDiscussion GetGroupDiscussionParams { get { return s_params_GetGroupDiscussion; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionParamsClass_GetGroupDiscussion
        {
            public readonly string id = "id";
        }
        static readonly ActionParamsClass_GetProfileVCard s_params_GetProfileVCard = new ActionParamsClass_GetProfileVCard();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionParamsClass_GetProfileVCard GetProfileVCardParams { get { return s_params_GetProfileVCard; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionParamsClass_GetProfileVCard
        {
            public readonly string sessionId = "sessionId";
            public readonly string snMemberId = "snMemberId";
        }
        static readonly ActionParamsClass_GetQuizReport s_params_GetQuizReport = new ActionParamsClass_GetQuizReport();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionParamsClass_GetQuizReport GetQuizReportParams { get { return s_params_GetQuizReport; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionParamsClass_GetQuizReport
        {
            public readonly string userId = "userId";
            public readonly string sessionId = "sessionId";
            public readonly string format = "format";
            public readonly string type = "type";
        }
        static readonly ActionParamsClass_GetSurveyReport s_params_GetSurveyReport = new ActionParamsClass_GetSurveyReport();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionParamsClass_GetSurveyReport GetSurveyReportParams { get { return s_params_GetSurveyReport; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionParamsClass_GetSurveyReport
        {
            public readonly string userId = "userId";
            public readonly string sessionId = "sessionId";
            public readonly string format = "format";
            public readonly string type = "type";
        }
        static readonly ActionParamsClass_GetTestReport s_params_GetTestReport = new ActionParamsClass_GetTestReport();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionParamsClass_GetTestReport GetTestReportParams { get { return s_params_GetTestReport; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionParamsClass_GetTestReport
        {
            public readonly string userId = "userId";
            public readonly string sessionId = "sessionId";
            public readonly string format = "format";
            public readonly string type = "type";
        }
        static readonly ActionParamsClass_ImportUsers s_params_ImportUsers = new ActionParamsClass_ImportUsers();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionParamsClass_ImportUsers ImportUsersParams { get { return s_params_ImportUsers; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionParamsClass_ImportUsers
        {
            public readonly string model = "model";
        }
        static readonly ActionParamsClass_MeetingHostReport s_params_MeetingHostReport = new ActionParamsClass_MeetingHostReport();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionParamsClass_MeetingHostReport MeetingHostReportParams { get { return s_params_MeetingHostReport; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionParamsClass_MeetingHostReport
        {
            public readonly string lmsCompanyId = "lmsCompanyId";
            public readonly string format = "format";
        }
        static readonly ViewsClass s_views = new ViewsClass();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ViewsClass Views { get { return s_views; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ViewsClass
        {
            static readonly _ViewNamesClass s_ViewNames = new _ViewNamesClass();
            public _ViewNamesClass ViewNames { get { return s_ViewNames; } }
            public class _ViewNamesClass
            {
                public readonly string ImportQuestions = "ImportQuestions";
                public readonly string ImportUsers = "ImportUsers";
                public readonly string VCF = "VCF";
            }
            public readonly string ImportQuestions = "~/Views/File/ImportQuestions.cshtml";
            public readonly string ImportUsers = "~/Views/File/ImportUsers.cshtml";
            public readonly string VCF = "~/Views/File/VCF.cshtml";
        }
    }

    [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
    public class T4MVC_FileController : EdugameCloud.MVC.Controllers.FileController
    {
        public T4MVC_FileController() : base(Dummy.Instance) { }

        public override System.Web.Mvc.ActionResult ExportQuestions(string id)
        {
            var callInfo = new T4MVC_ActionResult(Area, Name, ActionNames.ExportQuestions);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "id", id);
            return callInfo;
        }

        public override System.Web.Mvc.ActionResult ImportQuestions(EdugameCloud.MVC.ViewModels.ImportQuestionsViewModel model)
        {
            var callInfo = new T4MVC_ActionResult(Area, Name, ActionNames.ImportQuestions);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "model", model);
            return callInfo;
        }

        public override System.Web.Mvc.ActionResult ImportQuestions()
        {
            var callInfo = new T4MVC_ActionResult(Area, Name, ActionNames.ImportQuestions);
            return callInfo;
        }

        public override System.Web.Mvc.ActionResult TestImportQuestions()
        {
            var callInfo = new T4MVC_ActionResult(Area, Name, ActionNames.TestImportQuestions);
            return callInfo;
        }

        public override System.Web.Mvc.ActionResult ConvertToVCF(EdugameCloud.MVC.ViewModels.VCFViewModel model)
        {
            var callInfo = new T4MVC_ActionResult(Area, Name, ActionNames.ConvertToVCF);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "model", model);
            return callInfo;
        }

        public override System.Web.Mvc.ActionResult ConvertToVCF()
        {
            var callInfo = new T4MVC_ActionResult(Area, Name, ActionNames.ConvertToVCF);
            return callInfo;
        }

        public override System.Web.Mvc.ActionResult GetCollaborationReport(int userId, int? sessionId, string format)
        {
            var callInfo = new T4MVC_ActionResult(Area, Name, ActionNames.GetCollaborationReport);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "userId", userId);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "sessionId", sessionId);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "format", format);
            return callInfo;
        }

        public override System.Web.Mvc.ActionResult GetCrosswordsReport(int userId, int? sessionId, string format)
        {
            var callInfo = new T4MVC_ActionResult(Area, Name, ActionNames.GetCrosswordsReport);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "userId", userId);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "sessionId", sessionId);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "format", format);
            return callInfo;
        }

        public override System.Web.Mvc.ActionResult GetGroupDiscussion(int id)
        {
            var callInfo = new T4MVC_ActionResult(Area, Name, ActionNames.GetGroupDiscussion);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "id", id);
            return callInfo;
        }

        public override System.Web.Mvc.ActionResult GetProfileVCard(int sessionId, int snMemberId)
        {
            var callInfo = new T4MVC_ActionResult(Area, Name, ActionNames.GetProfileVCard);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "sessionId", sessionId);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "snMemberId", snMemberId);
            return callInfo;
        }

        public override System.Web.Mvc.ActionResult GetQuizReport(int userId, int? sessionId, string format, string type, bool detailed, string quizType)
        {
            var callInfo = new T4MVC_ActionResult(Area, Name, ActionNames.GetQuizReport);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "userId", userId);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "sessionId", sessionId);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "format", format);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "type", type);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "quizType", quizType);
            return callInfo;
        }

        public override System.Web.Mvc.ActionResult GetSurveyReport(int userId, int? sessionId, string format, string type, bool detailed)
        {
            var callInfo = new T4MVC_ActionResult(Area, Name, ActionNames.GetSurveyReport);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "userId", userId);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "sessionId", sessionId);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "format", format);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "type", type);
            return callInfo;
        }

        public override System.Web.Mvc.ActionResult GetTestReport(int userId, int? sessionId, string format, string type, bool detailed)
        {
            var callInfo = new T4MVC_ActionResult(Area, Name, ActionNames.GetTestReport);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "userId", userId);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "sessionId", sessionId);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "format", format);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "type", type);
            return callInfo;
        }

        public override System.Web.Mvc.ActionResult ImportUsers()
        {
            var callInfo = new T4MVC_ActionResult(Area, Name, ActionNames.ImportUsers);
            return callInfo;
        }

        public override System.Web.Mvc.ActionResult ImportUsers(EdugameCloud.MVC.ViewModels.ImportUsersViewModel model)
        {
            var callInfo = new T4MVC_ActionResult(Area, Name, ActionNames.ImportUsers);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "model", model);
            return callInfo;
        }

        public override System.Web.Mvc.ActionResult MeetingHostReport(int lmsCompanyId, string format)
        {
            var callInfo = new T4MVC_ActionResult(Area, Name, ActionNames.MeetingHostReport);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "lmsCompanyId", lmsCompanyId);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "format", format);
            return callInfo;
        }

    }
}

#endregion T4MVC
#pragma warning restore 1591
