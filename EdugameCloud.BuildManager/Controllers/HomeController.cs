using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Mvc;
using EdugameCloud.Lti.Tests.FrontEnd;

namespace EdugameCloud.BuildManager.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult CheckLti()
        {
            try
            {
                string exePath = ConfigurationManager.AppSettings["curlExePath"];
                string configs = Server.MapPath("~/App_Data/Prod/");

                var messages = new CanvasLtiChecker(exePath, configs).DoCheckRequests();
                ViewBag.Message = "Your application description page.";

                return View(messages);
            }
            catch (Exception ex)
            {
                return View(new List<string> { ex.Message, ex.StackTrace });
            }
        }

        [Authorize]
        public ActionResult Population()
        {
            return View();
        }
    }
}