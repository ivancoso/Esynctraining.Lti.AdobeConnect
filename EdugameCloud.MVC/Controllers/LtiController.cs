namespace EdugameCloud.MVC.Controllers
{
    using System;
    using System.Configuration;
    using System.Web.Mvc;

    using EdugameCloud.Core.Business.Models;
    using EdugameCloud.Core.Domain.Entities;
    using EdugameCloud.Lti.API.AdobeConnect;
    using EdugameCloud.Lti.DTO;

    using Esynctraining.AC.Provider;
    using Esynctraining.Core.Utils;

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
                    this.RedirectToError("No integration settings were set for your application.");
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
                        lis_person_contact_email_primary = "no-email@test.com"
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

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public virtual void Index(LtiParamDTO model)
        {
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

            Response.Redirect("/extjs/index.html"); //this.View("Index", model);
        }

        public virtual ActionResult JoinMeeting()
        {
            var url = meetingSetup.JoinMeeting(this.Credentials, this.Provider, this.Param);

            return this.Redirect(url);
        }

        [HttpPost]
        public JsonResult UpdateUser(UserDTO user)
        {
            var updatedUser = meetingSetup.UpdateUser(this.Credentials, this.Provider, this.Param, user);

            return this.Json(updatedUser);
        }

        [HttpPost]
        public JsonResult GetUsers()
        {
            var users = meetingSetup.GetUsers(this.Credentials, this.Provider, this.Param);

            return Json(users);
        }

        [HttpPost]
        public JsonResult GetMeeting()
        {
            var meeting = meetingSetup.GetMeeting(this.Credentials, this.Provider, this.Param);

            return Json(meeting);
        }

        [HttpPost]
        public JsonResult GetRecordings()
        {
            var recordings = meetingSetup.GetRecordings(this.Credentials, this.Provider, this.Param.custom_canvas_course_id);

            return Json(recordings);
        }

        [HttpPost]
        public JsonResult GetTemplates()
        {
            var templates = meetingSetup.GetTemplates(this.Provider, Credentials.ACTemplateScoId);

            return Json(templates);
        }

        [HttpPost]
        public JsonResult UpdateMeeting(MeetingDTO meeting)
        {
            var ret = meetingSetup.SaveMeeting(this.Credentials, this.Provider, this.Param, meeting);

            return Json(ret);
        }
    }
}
