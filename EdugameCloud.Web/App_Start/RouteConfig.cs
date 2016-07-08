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
            routes.IgnoreRoute("{*robotstxt}", new { robotstxt = @"(.*/)?robots.txt(/.*)?" });
            routes.MapLowercaseRoute("Public", "public/{fileName}", new { controller = "File", action = "Public" });
            RegisterSocialRoutes(routes);


            routes.MapLowercaseRoute("FileGet", "file/get", new { controller = "BuildDeliver", action = "get" });
            routes.MapLowercaseRoute("FilePublic", "file/public", new { controller = "BuildDeliver", action = "public" });
            routes.MapLowercaseRoute("FileGetPublicBuild", "file/get-public-build", new { controller = "BuildDeliver", action = "get-public-build" });
            routes.MapLowercaseRoute("FileGetMobileBuild", "file/get-mobile-build", new { controller = "BuildDeliver", action = "get-mobile-build" });

            routes.MapLowercaseRoute("File", "file/{action}", new { controller = "File" });
            routes.MapLowercaseRoute("Default", "{action}", new { controller = "Home", action = "Admin" });
            LtiRoutes.AppendTo(routes);
        }


        // TODO: delete
        private static void RegisterSocialRoutes(RouteCollection routes)
        {
            routes.MapLowercaseRoute("OAuthLogin", "social/{provider}-login", new { controller = "Social", action = "login" });
            routes.MapLowercaseRoute("OAuthCallback", "social/{provider}-callback", new { controller = "Social", action = "callback" });
            routes.MapLowercaseRoute("SubscriptionCallback", "social/{provider}-realtime", new { controller = "Social", action = "realtime-callback" });
        }

    }

}