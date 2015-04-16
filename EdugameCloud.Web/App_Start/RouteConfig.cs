namespace EdugameCloud.Web
{
    using System.Web.Mvc;
    using System.Web.Routing;
    using EdugameCloud.Lti.Core.Routes;

    /// <summary>
    /// The route config.
    /// </summary>
    public static class RouteConfig
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
            RegisterSocialRoutes(routes);
            routes.MapLowercaseRoute("File", "file/{action}", new { controller = "File" });
            routes.MapLowercaseRoute("Default", "{action}", new { controller = "Home", action = "Admin" });
            LtiRoutes.AppendTo(routes);
        }


        private static void RegisterSocialRoutes(RouteCollection routes)
        {
            routes.MapLowercaseRoute("OAuthLogin", "social/{provider}-login", new { controller = "Social", action = "login" });
            routes.MapLowercaseRoute("OAuthCallback", "social/{provider}-callback", new { controller = "Social", action = "callback" });
            routes.MapLowercaseRoute("SubscriptionCallback", "social/{provider}-realtime", new { controller = "Social", action = "realtime-callback" });
        }

    }

}