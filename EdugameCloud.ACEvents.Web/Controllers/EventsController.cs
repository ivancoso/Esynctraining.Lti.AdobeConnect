using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EdugameCloud.ACEvents.Web.Models;
using EdugameCloud.MVC.Controllers;
using Esynctraining.AC.Provider;
using Esynctraining.AC.Provider.DataObjects;
using Esynctraining.AdobeConnect;
using Esynctraining.Core.Logging;
using Esynctraining.Core.Utils;

namespace EdugameCloud.ACEvents.Web.Controllers
{
    public class EventsController : BaseController
    {
        //private readonly ILogger _logger;

        public EventsController()
        {
          //  _logger = logger;
        }

        //public ActionResult Index()
        //{


        //    //var breezeSession = Request.Cookies["BREEZESESSION"];
        //    //if (breezeSession != null)
        //    //{

        //    //}
        //    //else
        //    //{

        //    //}
        //    return RedirectToAction("Signup");
        //}

        public ActionResult Signup()
        {
            //if (Request.QueryString["licenseId"] == null || Request.QueryString["eventScoId"] == null)
            if ( Request.QueryString["eventScoId"] == null)
                throw new InvalidOperationException("You should pass licenseId and eventId in queryString");

            var eventScoId = Request.QueryString["eventScoId"];
            var acUrl = "http://esynctraining.adobeconnect.com";
            var apiUrl = acUrl + "/api/xml";
            var logger = IoC.Resolve<ILogger>();
            var proxy = new AdobeConnectProxy(new AdobeConnectProvider(new ConnectionDetails(apiUrl)), logger, apiUrl);
            var eventInfo = proxy.GetScoInfo(eventScoId);
            if (!eventInfo.Success)
                throw new InvalidOperationException("Error getting event info");
            var model = new EventModel();
            model.EventName = eventInfo.ScoInfo.Name;
            model.StartDate = eventInfo.ScoInfo.BeginDate;
            model.EndDate = eventInfo.ScoInfo.EndDate;

            model.States = new List<SelectListItem>()
            {
                new SelectListItem()
                {
                    Value = "LA",
                    Text = "LA"
                },
                new SelectListItem()
                {
                    Value = "NY",
                    Text = "NY"
                },
            };

            model.Schools = new List<SelectListItem>()
            {
                new SelectListItem()
                {
                    Value = "School N1",
                    Text = "School N1"
                },
                new SelectListItem()
                {
                    Value = "School N2",
                    Text = "School N2"
                },
            };

            return View(model);
        }

        [HttpPost]
        public void EventRegister()
        {
            
        }

        //private ActionResult Signin()
        //{
        //    return View();
        //}


    }
}