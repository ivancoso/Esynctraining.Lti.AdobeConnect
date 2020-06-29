namespace EdugameCloud.Lti.Web
{
    using System.Web.Mvc;
    using System.Web.Routing;

    using EdugameCloud.Lti.Core.Routes;

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
            LtiRoutes.AppendTo(routes);
        }
    }
}