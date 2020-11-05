using System;
using System.Globalization;
using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Yorktel.Service.Portal.Host
{
    public class IpAddressRouteConstraint : IRouteConstraint
    {
        public bool Match(HttpContext httpContext, IRouter route, string routeKey, RouteValueDictionary values, RouteDirection routeDirection)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }

            if (route == null)
            {
                throw new ArgumentNullException(nameof(route));
            }

            if (routeKey == null)
            {
                throw new ArgumentNullException(nameof(routeKey));
            }

            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }

            object routeValue;

            if (values.TryGetValue(routeKey, out routeValue)
                && routeValue != null)
            {
                var parameterValueString = Convert.ToString(routeValue, CultureInfo.InvariantCulture);

                IPAddress ipAddr;
                return IPAddress.TryParse(parameterValueString, out ipAddr);
            }

            return false;
        }
    }
}
