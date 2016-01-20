using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;
using EdugameCloud.Lti.Tests.FrontEnd;

namespace EdugameCloud.Lti.Monitoring.Controllers
{
    public class HomeController : Controller
    {
        private static object Lock = new object();


        //[OutputCache(Duration = 60, Location = System.Web.UI.OutputCacheLocation.Server)]
        public ActionResult Index()
        {
            lock (Lock)
            {
                try
                {
                    string configs = Server.MapPath(ConfigurationManager.AppSettings["Config"]);

                    IEnumerable<string> messages = Enumerable.Empty<string>();
                    lock (Lock)
                    {
                        messages = new CanvasLtiChecker2(
                            Server.MapPath(ConfigurationManager.AppSettings["Config"]), 
                            ConfigurationManager.AppSettings["LoginUrl"],
                            ConfigurationManager.AppSettings["SharedSecret"]).DoCheckRequests();
                    }
                    //string sharedSecret = "4fbf260d-f8dc-4a06-a9cb-7a8bacee437d"
                    return View(messages);
                }
                catch (Exception ex)
                {
                    string message = "v.2.0 Monitoring tool crash. ";
                    if (HttpContext.IsDebuggingEnabled)
                        message += ex.Message;
                    throw new InvalidOperationException(message);
                }
            }
        }

    }

}