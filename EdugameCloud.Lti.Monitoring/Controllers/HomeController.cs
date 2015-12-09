using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using EdugameCloud.Lti.Tests.FrontEnd;

namespace EdugameCloud.Lti.Monitoring.Controllers
{
    public class HomeController : Controller
    {
        private static object Lock = new object();


        [OutputCache(Duration = 60, Location = System.Web.UI.OutputCacheLocation.Server)]
        public ActionResult Index()
        {
            lock (Lock)
            {
                try
                {
                    string configs = Server.MapPath("~/App_Data/Prod/");

                    IEnumerable<string> messages = Enumerable.Empty<string>();
                    lock (Lock)
                    {
                        messages = new CanvasLtiChecker2(configs).DoCheckRequests();
                    }
                    return View(messages);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("Monitoring tool crash.");
                    //return View(new List<string> { "INTERNAL MONITORING TOOL ERROR", ex.Message });
                }
            }
        }

    }

}