namespace EdugameCloud.MVC.Controllers
{
    using System;
    using System.Configuration;
    using System.Drawing;
    using System.Reflection;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.SessionState;

    using EdugameCloud.Core.Business.Models;
    using EdugameCloud.Core.Domain.Entities;
    using EdugameCloud.Lti.API.AdobeConnect;
    using EdugameCloud.Lti.DTO;

    using Esynctraining.AC.Provider;
    using Esynctraining.Core.Utils;

    using Microsoft.Web.Infrastructure;

    using NHibernate.Hql.Ast.ANTLR;

    public class LtiController : BaseController
    {
        private bool IsDebug
        {
            get
            {
                return ConfigurationManager.AppSettings["IsDebug"] != null;
            }
        }

        private CanvasConnectCredentials Credentials
        {
            get
            {
                var creds = Session["Credentials"] as CanvasConnectCredentials;

                if (creds == null && IsDebug)
                {
                    creds = canvasConnectCredentialsModel.GetOneByDomain("canvas.instructure.com").Value;
                }

                if (creds == null)
                {
                    this.RedirectToError("Session timed out. Please refresh the Canvas page and press Join again.");
                    return null;
                }

                return creds;
            }
        }

        private AdobeConnectProvider Provider
        {
            get
            {
                var provider = Session["Provider"] as AdobeConnectProvider;

                if (provider == null)
                {
                    provider = meetingSetup.GetProvider(Credentials);
                    Session["Provider"] = provider;
                }

                return provider;
            }
        }

        private LtiParamDTO Param
        {
            get
            {
                var model = Session["Param"] as LtiParamDTO;

                if (model == null && IsDebug)
                {
                    model = new LtiParamDTO()
                    {
                        custom_canvas_course_id = 865831,
                        lis_person_contact_email_primary = "mike@esynctraining.com"
                    };
                }

                if (model == null)
                {
                    this.RedirectToError("You are not logged in.");
                    return null;
                }

                return model;
            }
        }

        private CanvasConnectCredentialsModel canvasConnectCredentialsModel
        {
            get
            {
                return IoC.Resolve<CanvasConnectCredentialsModel>();
            }
        }

        private MeetingSetup meetingSetup
        {
            get
            {
                return IoC.Resolve<MeetingSetup>();
            }
        }



        private void RedirectToError(string errorText)
        {
            Response.Clear();
            Response.Write(String.Format("{{ \"error\": \"{0}\" }}", errorText));
            Response.End();
        }
        private void RegenerateId()
        {
            var Context = System.Web.HttpContext.Current;
            var manager = new SessionIDManager();
            string oldId = manager.GetSessionID(Context);
            string newId = manager.CreateSessionID(Context);

            Response.Cookies.Add(new HttpCookie("ASP.NET_SessionId", newId) {Domain = ConfigurationManager.AppSettings["CookieDomain"]});

            bool isAdd = false, isRedir = false;
            manager.SaveSessionID(Context, newId, out isRedir, out isAdd);
            HttpApplication ctx = (HttpApplication)Context.ApplicationInstance;
            HttpModuleCollection mods = ctx.Modules;
            System.Web.SessionState.SessionStateModule ssm = (SessionStateModule)mods.Get("Session");
            System.Reflection.FieldInfo[] fields = ssm.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
            SessionStateStoreProviderBase store = null;
            System.Reflection.FieldInfo rqIdField = null, rqLockIdField = null, rqStateNotFoundField = null;
            foreach (System.Reflection.FieldInfo field in fields)
            {
                if (field.Name.Equals("_store")) store = (SessionStateStoreProviderBase)field.GetValue(ssm);
                if (field.Name.Equals("_rqId")) rqIdField = field;
                if (field.Name.Equals("_rqLockId")) rqLockIdField = field;
                if (field.Name.Equals("_rqSessionStateNotFound")) rqStateNotFoundField = field;
            }
            object lockId = rqLockIdField.GetValue(ssm);
            if ((lockId != null) && (oldId != null)) store.ReleaseItemExclusive(Context, oldId, lockId);
            rqStateNotFoundField.SetValue(ssm, true);
            rqIdField.SetValue(ssm, newId);
        }


        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public virtual void Index(string layout, LtiParamDTO model)
        {
            //this.RegenerateId();

            var credentials = canvasConnectCredentialsModel.GetOneByDomain(model.custom_canvas_api_domain).Value;
            if (credentials != null)
            {
                Session["Param"] = model;
                Session["Credentials"] = credentials;

                meetingSetup.SetupFolders(Credentials, Provider);
            }
            else if (!IsDebug)
            {
                this.RedirectToError("Cannot log in.");
                return;
            }

            //ViewBag["layout"] = model.layout;
            //this.View("Index", model);

            Response.Cookies.Add(new HttpCookie("ASP.NET_SessionId", Session.SessionID) { Domain = ConfigurationManager.AppSettings["CookieDomain"] });
            Response.Redirect(String.Format("/extjs/index.html?layout={0}", layout ?? "")); //this.View("Index", model);
        }

        public virtual ActionResult JoinMeeting()
        {
            var url = meetingSetup.JoinMeeting(this.Credentials, this.Param);

            return this.Redirect(url);
        }

        public virtual ActionResult JoinRecording(string recordingUrl)
        {
            var url = meetingSetup.JoinRecording(this.Credentials, this.Param, recordingUrl);

            if (url == null)
            {
                this.RedirectToError("Can not access the recording");
            }

            return this.Redirect(url);
        }

        [HttpPost]
        public JsonResult UpdateUser(UserDTO user)
        {
            var updatedUser = meetingSetup.UpdateUser(this.Credentials, meetingSetup.GetProvider(this.Credentials), this.Param, user);

            return this.Json(updatedUser);
        }

        [HttpPost]
        public JsonResult GetUsers()
        {
            var users = meetingSetup.GetUsers(this.Credentials, meetingSetup.GetProvider(this.Credentials), this.Param);

            return Json(users);
        }

        [HttpPost]
        public JsonResult GetMeeting()
        {
            var meeting = meetingSetup.GetMeeting(this.Credentials, meetingSetup.GetProvider(this.Credentials), this.Param);

            return Json(meeting);
        }

        [HttpPost]
        public JsonResult GetRecordings()
        {
            var recordings = meetingSetup.GetRecordings(this.Credentials, meetingSetup.GetProvider(this.Credentials), this.Param.custom_canvas_course_id);

            return Json(recordings);
        }

        [HttpPost]
        public JsonResult GetTemplates()
        {
            var templates = meetingSetup.GetTemplates(meetingSetup.GetProvider(this.Credentials), Credentials.ACTemplateScoId);

            return Json(templates);
        }

        [HttpPost]
        public JsonResult UpdateMeeting(MeetingDTO meeting)
        {
            var ret = meetingSetup.SaveMeeting(this.Credentials, meetingSetup.GetProvider(this.Credentials), this.Param, meeting);

            return Json(ret);
        }
    }
}
