using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Text;

namespace Esynctraining.Lti.Zoom.Routes
{
    public static class LtiRoutes
    {
        public static void AppendTo(IRouteBuilder routes)
        {
            routes.MapRoute("LtiOAuthLogin", "{provider}-login", new { controller = RouteConstants.LtiControllerName, action = "LoginWithProvider" });
            routes.MapRoute("LtiOAuthCallback", "oauth-callback", new { controller = RouteConstants.LtiControllerName, action = RouteConstants.AuthCallbackActionName });
            routes.MapRoute("default", "{controller=Home}/{action=Index}/{id?}");
        }
    }

    public static class RouteConstants
    {
        public const string AuthCallbackActionName = "AuthenticationCallback";
        public const string LtiControllerName = "Lti";
    }
}
