using System.Web;
using System.Web.Mvc;

namespace EdugameCloud.Lti.Host.CustomAttributes
{
    public class AcSessionAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            return true;
        }
    }
}