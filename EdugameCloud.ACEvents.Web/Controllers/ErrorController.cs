﻿//using System.Web;
//using System.Web.Mvc;
using Microsoft.AspNetCore.Mvc;

namespace EdugameCloud.ACEvents.Web.Controllers
{
    public class ErrorController : Controller
    {
        [Route("/Error")]
        public IActionResult Index()
        {
            return View("Error");
        }
    }
}