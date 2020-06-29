using EdugameCloud.Lti.Core.Routes;

namespace EdugameCloud.Lti.Extensions
{
    using System;
    using System.Web.Mvc;

    public static class UrlExtensions
    {
        public static string AbsoluteAction(
            this UrlHelper url, string action, string controller, object roteValues = null, string schema = "http")
        {
            Uri requestUrl = url.RequestContext.HttpContext.Request.Url;
            string absoluteAction;
            if (requestUrl != null)
            {
                absoluteAction = string.Format(
                    "{0}://{1}{2}", schema.Equals("https", StringComparison.OrdinalIgnoreCase) ? schema : requestUrl.Scheme, requestUrl.Authority, url.Action(action, controller, roteValues));
            }
            else
            {
                absoluteAction = url.Action(action, controller, roteValues, schema);
            }

            return absoluteAction;
        }

        public static string AbsoluteCallbackAction(this UrlHelper url, string schema, object routeValues)
        {
            Uri requestUrl = url.RequestContext.HttpContext.Request.Url;
            return $"{schema}://{requestUrl.Authority}{url.Action(LtiRoutes.RouteConstants.AuthCallbackActionName, LtiRoutes.RouteConstants.LtiControllerName, routeValues)}";
        }
    }

}