namespace EdugameCloud.Lti.Host
{
    using System.Web.Mvc;
    using System.Web.Routing;
    using EdugameCloud.Lti.Core.Routes;

    public static class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("{*favicon}", new { favicon = @"(.*/)?favicon.ico(/.*)?" });
            routes.IgnoreRoute("{*robotstxt}", new { robotstxt = @"(.*/)?robots.txt(/.*)?" });

            // TODO!!:
            routes.MapLowercaseRoute("File", "file/{action}", new { controller = "File" });
            LtiRoutes.AppendTo(routes); //AppendTo2
        }

    }

}