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

            routes.MapRoute("FileGet", "file/get", new { controller = "BuildDeliver", action = "GetFile" });
            routes.MapRoute("FilePublic", "file/public", new { controller = "BuildDeliver", action = "Public" });
            routes.MapRoute("FileGetPublicBuild", "file/get-public-build", new { controller = "BuildDeliver", action = "GetPublicBuild" });
            routes.MapRoute("FileGetMobileBuild", "file/get-mobile-build", new { controller = "BuildDeliver", action = "GetMobileBuild" });

            routes.MapRoute("FileUpload", "file/{id}/upload", new { controller = "FileUpload", action = "Upload" });

            routes.MapRoute("File", "file/{action}", new { controller = "File" });
            routes.MapRoute("Default", "{action}", new { controller = "Home", action = "Admin" });
        }

    }

}