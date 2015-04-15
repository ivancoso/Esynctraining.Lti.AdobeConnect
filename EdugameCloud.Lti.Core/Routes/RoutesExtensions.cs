namespace EdugameCloud.Lti.Core.Routes
{
    using System;
    using System.Web.Mvc;
    using System.Web.Routing;

    /// <summary>
    /// The routes extensions.
    /// </summary>
    public static class RoutesExtensions
    {
        #region Public Methods and Operators

        /// <summary>
        /// The map lowercase route.
        /// </summary>
        /// <param name="routes">
        /// The routes.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="url">
        /// The url.
        /// </param>
        /// <param name="defaults">
        /// The defaults.
        /// </param>
        /// <returns>
        /// The <see cref="Route"/>.
        /// </returns>
        public static Route MapLowercaseRoute(this RouteCollection routes, string name, string url, object defaults)
        {
            return MapLowercaseRoute(routes, name, url, defaults, null);
        }

        /// <summary>
        /// The map lowercase route.
        /// </summary>
        /// <param name="routes">
        /// The routes.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="url">
        /// The url.
        /// </param>
        /// <param name="defaults">
        /// The defaults.
        /// </param>
        /// <param name="constraints">
        /// The constraints.
        /// </param>
        /// <returns>
        /// The <see cref="Route"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// if routes or url is empty
        /// </exception>
        public static Route MapLowercaseRoute(this RouteCollection routes, string name, string url, object defaults, object constraints)
        {
            if (routes == null)
            {
                throw new ArgumentNullException("routes");
            }

            if (url == null)
            {
                throw new ArgumentNullException("url");
            }

            var route = new LowercaseRoute(url, new MvcRouteHandler())
                            {
                                Defaults = new RouteValueDictionary(defaults), 
                                Constraints =
                                    new RouteValueDictionary(constraints), 
                                DataTokens = new RouteValueDictionary()
                            };
            routes.Add(name, route);
            return route;
        }

        #endregion
    }
}