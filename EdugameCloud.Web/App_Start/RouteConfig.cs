namespace EdugameCloud.Web
{
    using System.Web.Mvc;
    using System.Web.Routing;
    using EdugameCloud.Lti.Routes;

    /// <summary>
    /// The route config.
    /// </summary>
    public class RouteConfig
    {
        /// <summary>
        /// The register routes.
        /// </summary>
        /// <param name="routes">
        /// The routes.
        /// </param>
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("{*favicon}", new { favicon = @"(.*/)?favicon.ico(/.*)?" });
            routes.MapLowercaseRoute("Public", "public/{fileName}", new { controller = "File", action = "Public" });
            routes.MapLowercaseRoute("OAuthLogin", "social/{provider}-login", new { controller = "Social", action = "login" });
            routes.MapLowercaseRoute("OAuthCallback", "social/{provider}-callback", new { controller = "Social", action = "callback" });
            routes.MapLowercaseRoute("SubscriptionCallback", "social/{provider}-realtime", new { controller = "Social", action = "realtime-callback" });
            routes.MapLowercaseRoute("File", "file/{action}", new { controller = "File" });
//            routes.MapLowercaseRoute("LtiOAuthLogin", "lti/{provider}-login", new { controller = "Lti", action = "login" });
//            routes.MapLowercaseRoute("LtiOAuthCallback", "lti/oauth-callback", new { controller = "Lti", action = "callback" });
            routes.MapLowercaseRoute("Default", "{action}", new { controller = "Home", action = "Admin" });
            LtiRoutes.AppendTo(routes);
        }
    }
}