namespace EdugameCloud.Web
{
    using System.Web.Mvc;
    using System.Web.Routing;

    public static class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            // ?? MapLowercaseRoute
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("{*favicon}", new { favicon = @"(.*/)?favicon.ico(/.*)?" });
            routes.IgnoreRoute("{*robotstxt}", new { robotstxt = @"(.*/)?robots.txt(/.*)?" });
            routes.MapRoute("Public", "public/{fileName}", new { controller = "File", action = "Public" });

            routes.MapRoute("FileGet", "file/get", new { controller = "BuildDeliver", action = "get" });
            routes.MapRoute("FilePublic", "file/public", new { controller = "BuildDeliver", action = "public" });
            routes.MapRoute("FileGetPublicBuild", "file/get-public-build", new { controller = "BuildDeliver", action = "get-public-build" });
            routes.MapRoute("FileGetMobileBuild", "file/get-mobile-build", new { controller = "BuildDeliver", action = "get-mobile-build" });

            routes.MapRoute("File", "file/{action}", new { controller = "File" });
            routes.MapRoute("Default", "{action}", new { controller = "Home", action = "Admin" });
        }

    }

}