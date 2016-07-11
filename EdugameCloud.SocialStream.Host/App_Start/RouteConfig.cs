using System.Web.Mvc;
using System.Web.Routing;

namespace EdugameCloud.SocialStream.Host
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("{*favicon}", new { favicon = @"(.*/)?favicon.ico(/.*)?" });
            routes.IgnoreRoute("{*robotstxt}", new { robotstxt = @"(.*/)?robots.txt(/.*)?" });

            RegisterSocialRoutes(routes);
        }

        private static void RegisterSocialRoutes(RouteCollection routes)
        {
            routes.MapRoute("OAuthLogin", "{provider}-login", new { controller = "Social", action = "login" });
            routes.MapRoute("OAuthCallback", "{provider}-callback", new { controller = "Social", action = "callback" });
            routes.MapRoute("SubscriptionCallback", "{provider}-realtime", new { controller = "Social", action = "realtime-callback" });
        }

    }

}

