namespace EdugameCloud.Lti.Controllers
{
    using System;
    using System.Web.Mvc;
    using EdugameCloud.Lti.API.AdobeConnect;

    public partial class OfficeHoursPodController : Controller
    {
        private readonly MeetingSetup meetingSetup;


        public OfficeHoursPodController(MeetingSetup meetingSetup)
        {
            this.meetingSetup = meetingSetup ?? throw new ArgumentNullException(nameof(meetingSetup));
        }


        [HttpGet]
        [OutputCache(VaryByParam = "*", NoStore = true, Duration = 0, Location = System.Web.UI.OutputCacheLocation.None)]
        public virtual JsonResult GetAuthenticationParameters(string acId, string acDomain, string scoId)
        {
            string error = null;
            var param = meetingSetup.GetLmsParameters(acId, acDomain, scoId, ref error);
            if (param != null)
            {
                return this.Json(param, JsonRequestBehavior.AllowGet);
            }

            return this.Json(new { error }, JsonRequestBehavior.AllowGet);
        }

    }

}
