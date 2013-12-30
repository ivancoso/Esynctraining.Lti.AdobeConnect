namespace EdugameCloud.MVC.HtmlHelpers
{
    using System;
    using System.Web.Mvc;

    using Esynctraining.Core.Providers;
    using Esynctraining.Core.Utils;

    /// <summary>
    /// The url extensions.
    /// </summary>
    public static class UrlExtensions
    {
        #region Public Methods and Operators

        /// <summary>
        /// The absolute action.
        /// </summary>
        /// <param name="url">
        /// The url.
        /// </param>
        /// <param name="action">
        /// The action.
        /// </param>
        /// <param name="controller">
        /// The controller.
        /// </param>
        /// <param name="roteValues">
        /// The rote values.
        /// </param>
        /// <param name="schema">
        /// The schema.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string AbsoluteAction(
            this UrlHelper url, string action, string controller, object roteValues = null, string schema = "http")
        {
            Uri requestUrl = url.RequestContext.HttpContext.Request.Url;
            string absoluteAction;
            if (requestUrl != null)
            {
                absoluteAction = string.Format(
                    "{0}://{1}{2}", requestUrl.Scheme, requestUrl.Authority, url.Action(action, controller, roteValues));
            }
            else
            {
                absoluteAction = url.Action(action, controller, roteValues, schema);
            }

            return absoluteAction;
        }

        public static string HttpActionAbsolute(this UrlHelper urlHelper, ActionResult result)
        {
            var portToUse = int.Parse((string)((dynamic)IoC.Resolve<ApplicationSettingsProvider>()).HttpPort);
            var builder = new UriBuilder(urlHelper.RequestContext.HttpContext.Request.Url) { Scheme = Uri.UriSchemeHttp, Port = portToUse };
            return string.Format("{0}{1}", builder.Uri.GetLeftPart(UriPartial.Authority), urlHelper.RouteUrl(result.GetRouteValueDictionary()));
        }

        /// <summary>
        /// The absolute action.
        /// </summary>
        /// <param name="url">
        /// The url.
        /// </param>
        /// <param name="actionResult">
        /// The action result.
        /// </param>
        /// <param name="roteValues">
        /// The rote values.
        /// </param>
        /// <param name="schema">
        /// The schema.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string AbsoluteAction(this UrlHelper url, ActionResult actionResult, object roteValues = null, string schema = "http")
         {
             var callInfo = actionResult.GetT4MVCResult();
             return AbsoluteAction(url, callInfo.Action, callInfo.Controller, roteValues, schema);
         }

        #endregion
    }
}