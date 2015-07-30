namespace EdugameCloud.Lti.Controllers
{
    using System.Web.Mvc;
    
    public partial class LtiController
    {
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
