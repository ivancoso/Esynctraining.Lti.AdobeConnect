using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EdugameCloud.Sakai.TestClient.Models;

namespace EdugameCloud.Sakai.TestClient.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View(new SakaiParameters());
        }

        [HttpPost]
        public ActionResult Index(SakaiParameters parameters)
        {
            if (!ModelState.IsValid)
                return View(parameters);

            var result = LTI2Api.CreateSignedRequestAndGetResponse(parameters);

            parameters.RequestParams = result.Item1;
            parameters.ResponseString = result.Item2;

            return View(parameters);
        }
    }

}