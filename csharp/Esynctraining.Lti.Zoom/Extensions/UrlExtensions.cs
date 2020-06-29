using Esynctraining.Lti.Zoom.Routes;
using LtiLibrary.AspNetCore.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using System;

namespace Esynctraining.Lti.Zoom.Extensions
{
    public static class UrlExtensions
    {
        public static string AbsoluteAction(
            this UrlHelper url, string action, string controller, object roteValues = null, string schema = "http")
        {
            Uri requestUrl = url.ActionContext.HttpContext.Request.GetUri();
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

        public static string AbsoluteCallbackAction(this IUrlHelper url, string schema, object routeValues)
        {
            Uri requestUrl = url.ActionContext.HttpContext.Request.GetUri();
            return $"{schema}://{requestUrl.Authority}{url.Action(RouteConstants.AuthCallbackActionName, RouteConstants.LtiControllerName, routeValues)}";
        }
    }
}
