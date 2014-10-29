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
    public partial class LtiController
    {
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        protected LtiController(Dummy d) { }

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
        public System.Web.Mvc.ActionResult AuthenticationCallback()
        {
            return new T4MVC_ActionResult(Area, Name, ActionNames.AuthenticationCallback);
        }
        [NonAction]
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public System.Web.Mvc.JsonResult DeleteRecording()
        {
            return new T4MVC_JsonResult(Area, Name, ActionNames.DeleteRecording);
        }
        [NonAction]
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public System.Web.Mvc.ActionResult Index()
        {
            return new T4MVC_ActionResult(Area, Name, ActionNames.Index);
        }
        [NonAction]
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public System.Web.Mvc.ActionResult JoinRecording()
        {
            return new T4MVC_ActionResult(Area, Name, ActionNames.JoinRecording);
        }
        [NonAction]
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public System.Web.Mvc.ActionResult LoginWithProvider()
        {
            return new T4MVC_ActionResult(Area, Name, ActionNames.LoginWithProvider);
        }
        [NonAction]
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public System.Web.Mvc.ActionResult RedirectToErrorPage()
        {
            return new T4MVC_ActionResult(Area, Name, ActionNames.RedirectToErrorPage);
        }
        [NonAction]
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public System.Web.Mvc.JsonResult UpdateMeeting()
        {
            return new T4MVC_JsonResult(Area, Name, ActionNames.UpdateMeeting);
        }
        [NonAction]
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public System.Web.Mvc.JsonResult UpdateUser()
        {
            return new T4MVC_JsonResult(Area, Name, ActionNames.UpdateUser);
        }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public LtiController Actions { get { return EdugameCloudT4.Lti; } }
        [GeneratedCode("T4MVC", "2.0")]
        public readonly string Area = "";
        [GeneratedCode("T4MVC", "2.0")]
        public readonly string Name = "Lti";
        [GeneratedCode("T4MVC", "2.0")]
        public const string NameConst = "Lti";

        static readonly ActionNamesClass s_actions = new ActionNamesClass();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionNamesClass ActionNames { get { return s_actions; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionNamesClass
        {
            public readonly string AuthenticationCallback = "callback";
            public readonly string DeleteRecording = "DeleteRecording";
            public readonly string GetMeeting = "GetMeeting";
            public readonly string GetRecordings = "GetRecordings";
            public readonly string GetTemplates = "GetTemplates";
            public readonly string GetUsers = "GetUsers";
            public readonly string Index = "Index";
            public readonly string JoinMeeting = "JoinMeeting";
            public readonly string JoinRecording = "JoinRecording";
            public readonly string LoginWithProvider = "login";
            public readonly string RedirectToErrorPage = "RedirectToErrorPage";
            public readonly string UpdateMeeting = "UpdateMeeting";
            public readonly string UpdateUser = "UpdateUser";
        }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionNameConstants
        {
            public const string AuthenticationCallback = "callback";
            public const string DeleteRecording = "DeleteRecording";
            public const string GetMeeting = "GetMeeting";
            public const string GetRecordings = "GetRecordings";
            public const string GetTemplates = "GetTemplates";
            public const string GetUsers = "GetUsers";
            public const string Index = "Index";
            public const string JoinMeeting = "JoinMeeting";
            public const string JoinRecording = "JoinRecording";
            public const string LoginWithProvider = "login";
            public const string RedirectToErrorPage = "RedirectToErrorPage";
            public const string UpdateMeeting = "UpdateMeeting";
            public const string UpdateUser = "UpdateUser";
        }


        static readonly ActionParamsClass_AuthenticationCallback s_params_AuthenticationCallback = new ActionParamsClass_AuthenticationCallback();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionParamsClass_AuthenticationCallback AuthenticationCallbackParams { get { return s_params_AuthenticationCallback; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionParamsClass_AuthenticationCallback
        {
            public readonly string __provider__ = "__provider__";
            public readonly string __sid__ = "__sid__";
            public readonly string code = "code";
            public readonly string state = "state";
        }
        static readonly ActionParamsClass_DeleteRecording s_params_DeleteRecording = new ActionParamsClass_DeleteRecording();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionParamsClass_DeleteRecording DeleteRecordingParams { get { return s_params_DeleteRecording; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionParamsClass_DeleteRecording
        {
            public readonly string id = "id";
        }
        static readonly ActionParamsClass_Index s_params_Index = new ActionParamsClass_Index();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionParamsClass_Index IndexParams { get { return s_params_Index; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionParamsClass_Index
        {
            public readonly string model = "model";
        }
        static readonly ActionParamsClass_JoinRecording s_params_JoinRecording = new ActionParamsClass_JoinRecording();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionParamsClass_JoinRecording JoinRecordingParams { get { return s_params_JoinRecording; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionParamsClass_JoinRecording
        {
            public readonly string recordingUrl = "recordingUrl";
        }
        static readonly ActionParamsClass_LoginWithProvider s_params_LoginWithProvider = new ActionParamsClass_LoginWithProvider();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionParamsClass_LoginWithProvider LoginWithProviderParams { get { return s_params_LoginWithProvider; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionParamsClass_LoginWithProvider
        {
            public readonly string provider = "provider";
            public readonly string model = "model";
        }
        static readonly ActionParamsClass_RedirectToErrorPage s_params_RedirectToErrorPage = new ActionParamsClass_RedirectToErrorPage();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionParamsClass_RedirectToErrorPage RedirectToErrorPageParams { get { return s_params_RedirectToErrorPage; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionParamsClass_RedirectToErrorPage
        {
            public readonly string errorText = "errorText";
        }
        static readonly ActionParamsClass_UpdateMeeting s_params_UpdateMeeting = new ActionParamsClass_UpdateMeeting();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionParamsClass_UpdateMeeting UpdateMeetingParams { get { return s_params_UpdateMeeting; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionParamsClass_UpdateMeeting
        {
            public readonly string meeting = "meeting";
        }
        static readonly ActionParamsClass_UpdateUser s_params_UpdateUser = new ActionParamsClass_UpdateUser();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionParamsClass_UpdateUser UpdateUserParams { get { return s_params_UpdateUser; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionParamsClass_UpdateUser
        {
            public readonly string user = "user";
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
                public readonly string Error = "Error";
            }
            public readonly string Error = "~/Views/Lti/Error.cshtml";
        }
    }

    [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
    public class T4MVC_LtiController : EdugameCloud.MVC.Controllers.LtiController
    {
        public T4MVC_LtiController() : base(Dummy.Instance) { }

        public override System.Web.Mvc.ActionResult AuthenticationCallback(string __provider__, string __sid__, string code, string state)
        {
            var callInfo = new T4MVC_ActionResult(Area, Name, ActionNames.AuthenticationCallback);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "__provider__", __provider__);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "__sid__", __sid__);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "code", code);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "state", state);
            return callInfo;
        }

        public override System.Web.Mvc.JsonResult DeleteRecording(string id)
        {
            var callInfo = new T4MVC_JsonResult(Area, Name, ActionNames.DeleteRecording);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "id", id);
            return callInfo;
        }

        public override System.Web.Mvc.JsonResult GetMeeting()
        {
            var callInfo = new T4MVC_JsonResult(Area, Name, ActionNames.GetMeeting);
            return callInfo;
        }

        public override System.Web.Mvc.JsonResult GetRecordings()
        {
            var callInfo = new T4MVC_JsonResult(Area, Name, ActionNames.GetRecordings);
            return callInfo;
        }

        public override System.Web.Mvc.JsonResult GetTemplates()
        {
            var callInfo = new T4MVC_JsonResult(Area, Name, ActionNames.GetTemplates);
            return callInfo;
        }

        public override System.Web.Mvc.JsonResult GetUsers()
        {
            var callInfo = new T4MVC_JsonResult(Area, Name, ActionNames.GetUsers);
            return callInfo;
        }

        public override System.Web.Mvc.ActionResult Index(EdugameCloud.Lti.DTO.LtiParamDTO model)
        {
            var callInfo = new T4MVC_ActionResult(Area, Name, ActionNames.Index);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "model", model);
            return callInfo;
        }

        public override System.Web.Mvc.ActionResult JoinMeeting()
        {
            var callInfo = new T4MVC_ActionResult(Area, Name, ActionNames.JoinMeeting);
            return callInfo;
        }

        public override System.Web.Mvc.ActionResult JoinRecording(string recordingUrl)
        {
            var callInfo = new T4MVC_ActionResult(Area, Name, ActionNames.JoinRecording);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "recordingUrl", recordingUrl);
            return callInfo;
        }

        public override System.Web.Mvc.ActionResult LoginWithProvider(string provider, EdugameCloud.Lti.DTO.LtiParamDTO model)
        {
            var callInfo = new T4MVC_ActionResult(Area, Name, ActionNames.LoginWithProvider);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "provider", provider);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "model", model);
            return callInfo;
        }

        public override System.Web.Mvc.ActionResult RedirectToErrorPage(string errorText)
        {
            var callInfo = new T4MVC_ActionResult(Area, Name, ActionNames.RedirectToErrorPage);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "errorText", errorText);
            return callInfo;
        }

        public override System.Web.Mvc.JsonResult UpdateMeeting(EdugameCloud.Lti.DTO.MeetingDTO meeting)
        {
            var callInfo = new T4MVC_JsonResult(Area, Name, ActionNames.UpdateMeeting);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "meeting", meeting);
            return callInfo;
        }

        public override System.Web.Mvc.JsonResult UpdateUser(EdugameCloud.Lti.DTO.LmsUserDTO user)
        {
            var callInfo = new T4MVC_JsonResult(Area, Name, ActionNames.UpdateUser);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "user", user);
            return callInfo;
        }

    }
}

#endregion T4MVC
#pragma warning restore 1591
