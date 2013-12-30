namespace EdugameCloud.Web.App_Start
{
    using System.Web.Mvc;
    using System.Web.Routing;

    using EdugameCloud.MVC.Routes;

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
            routes.MapLowercaseRoute("Public", "public/{fileName}", new { controller = "File", action="Public" });
            routes.MapLowercaseRoute("File", "file/{action}", new { controller = "File" });
            routes.MapLowercaseRoute("Default", "{action}", new { controller = "Home", action = "Admin" });
        }
    }
}