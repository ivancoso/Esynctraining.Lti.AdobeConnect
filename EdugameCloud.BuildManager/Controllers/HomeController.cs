using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;
using EdugameCloud.Lti.Tests.FrontEnd;

namespace EdugameCloud.BuildManager.Controllers
{
    public class HomeController : Controller
    {
        private static object Lock = new object();

        public ActionResult Index()
        {
            return View();
        }

        [OutputCache(Duration = 60, Location = System.Web.UI.OutputCacheLocation.Server)]
        public ActionResult CheckLti()
        {
            lock (Lock)
            {
                try
                {
                    string exePath = ConfigurationManager.AppSettings["curlExePath"];
                    string configs = Server.MapPath("~/App_Data/Prod/");

                    IEnumerable<string> messages = Enumerable.Empty<string>();
                    lock (Lock)
                    {
                        messages = new CanvasLtiChecker(exePath, configs).DoCheckRequests();
                    }
                    return View(messages);
                }
                catch (Exception ex)
                {
                    return View(new List<string> { "INTERNAL MONITORING TOOL ERROR" });
                }
            }
        }

        [Authorize]
        public ActionResult Population()
        {
            return View();
        }
    }
}