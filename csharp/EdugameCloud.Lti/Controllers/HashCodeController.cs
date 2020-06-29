namespace EdugameCloud.Lti.Controllers
{
    using System;
    using System.Web.Mvc;

    public class HashCodeController : Controller
    {
        public HashCodeController() { }


        [HttpGet]
        public JsonResult GetHashCode(string value)
        {
            return Json(value.GetHashCode(), JsonRequestBehavior.AllowGet);
        }


        protected override JsonResult Json(object data, string contentType,
                System.Text.Encoding contentEncoding, JsonRequestBehavior behavior)
        {
            return new JsonNetResult
            {
                Data = data,
                ContentType = contentType,
                ContentEncoding = contentEncoding,
                JsonRequestBehavior = behavior,
            };
        }

    }

}
