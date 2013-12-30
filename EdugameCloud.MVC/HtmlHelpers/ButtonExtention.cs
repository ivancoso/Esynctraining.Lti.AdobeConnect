namespace EdugameCloud.MVC.HtmlHelpers
{
    using System.Collections.Generic;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;

    /// <summary>
    /// The anchor buttons.
    /// </summary>
    public static class ButtonExtention
    {
        #region Public Methods and Operators

        /// <summary>
        /// The anchor button.
        /// </summary>
        /// <param name="htmlHelper">
        /// The html helper.
        /// </param>
        /// <param name="linkText">
        /// The link text.
        /// </param>
        /// <param name="actionName">
        /// The action name.
        /// </param>
        /// <param name="controllerName">
        /// The controller name.
        /// </param>
        /// <param name="routeValues">
        /// The route values.
        /// </param>
        /// <param name="htmlAttributes">
        /// The html attributes.
        /// </param>
        /// <returns>
        /// The <see cref="MvcHtmlString"/>.
        /// </returns>
        public static MvcHtmlString AnchorButton(
            this HtmlHelper htmlHelper, 
            string linkText, 
            string actionName, 
            string controllerName, 
            object routeValues, 
            object htmlAttributes)
        {
            return
                MvcHtmlString.Create(
                    GenerateAnchorButton(
                        htmlHelper.ViewContext.RequestContext, 
                        htmlHelper.RouteCollection, 
                        linkText, 
                        actionName, 
                        controllerName, 
                        new RouteValueDictionary(routeValues), 
                        new RouteValueDictionary(htmlAttributes)));
        }

        /// <summary>
        /// The anchor button.
        /// </summary>
        /// <param name="htmlHelper">
        /// The html helper.
        /// </param>
        /// <param name="linkText">
        /// The link text.
        /// </param>
        /// <param name="actionResult">
        /// The action result.
        /// </param>
        /// <param name="routeValues">
        /// The route values.
        /// </param>
        /// <param name="htmlAttributes">
        /// The html attributes.
        /// </param>
        /// <returns>
        /// The <see cref="MvcHtmlString"/>.
        /// </returns>
        public static MvcHtmlString AnchorButton(
            this HtmlHelper htmlHelper, 
            string linkText, 
            ActionResult actionResult, 
            RouteValueDictionary routeValues, 
            object htmlAttributes)
        {
            IT4MVCActionResult callInfo = actionResult.GetT4MVCResult();
            return
                MvcHtmlString.Create(
                    GenerateAnchorButton(
                        htmlHelper.ViewContext.RequestContext, 
                        htmlHelper.RouteCollection, 
                        linkText, 
                        callInfo.Action, 
                        callInfo.Controller, 
                        routeValues, 
                        new RouteValueDictionary(htmlAttributes)));
        }

        /// <summary>
        /// The anchor button.
        /// </summary>
        /// <param name="htmlHelper">
        /// The html helper.
        /// </param>
        /// <param name="linkText">
        /// The link text.
        /// </param>
        /// <param name="actionName">
        /// The action name.
        /// </param>
        /// <param name="controllerName">
        /// The controller name.
        /// </param>
        /// <param name="routeValues">
        /// The route values.
        /// </param>
        /// <param name="htmlAttributes">
        /// The html attributes.
        /// </param>
        /// <returns>
        /// The <see cref="MvcHtmlString"/>.
        /// </returns>
        public static MvcHtmlString AnchorButton(
            this HtmlHelper htmlHelper, 
            string linkText, 
            string actionName, 
            string controllerName, 
            RouteValueDictionary routeValues, 
            object htmlAttributes)
        {
            return
                MvcHtmlString.Create(
                    GenerateAnchorButton(
                        htmlHelper.ViewContext.RequestContext, 
                        htmlHelper.RouteCollection, 
                        linkText, 
                        actionName, 
                        controllerName, 
                        routeValues, 
                        new RouteValueDictionary(htmlAttributes)));
        }

        /// <summary>
        /// The anchor button.
        /// </summary>
        /// <param name="htmlHelper">
        /// The html helper.
        /// </param>
        /// <param name="linkText">
        /// The link text.
        /// </param>
        /// <param name="actionName">
        /// The action name.
        /// </param>
        /// <param name="controllerName">
        /// The controller name.
        /// </param>
        /// <returns>
        /// The <see cref="MvcHtmlString"/>.
        /// </returns>
        public static MvcHtmlString AnchorButton(this HtmlHelper htmlHelper, string linkText, ActionResult actionResult, object html = null)
        {
            var callInfo = actionResult.GetT4MVCResult();
            return
                MvcHtmlString.Create(
                    GenerateAnchorButton(
                        htmlHelper.ViewContext.RequestContext,
                        htmlHelper.RouteCollection,
                        linkText,
                        callInfo.Action,
                        callInfo.Controller,
                        callInfo.RouteValueDictionary,
                        new RouteValueDictionary(html)));
        }

        /// <summary>
        /// The anchor button.
        /// </summary>
        /// <param name="htmlHelper">
        /// The html helper.
        /// </param>
        /// <param name="linkText">
        /// The link text.
        /// </param>
        /// <param name="actionName">
        /// The action name.
        /// </param>
        /// <param name="controllerName">
        /// The controller name.
        /// </param>
        /// <returns>
        /// The <see cref="MvcHtmlString"/>.
        /// </returns>
        public static MvcHtmlString AnchorButton(
            this HtmlHelper htmlHelper, string linkText, string actionName, string controllerName)
        {
            return
                MvcHtmlString.Create(
                    GenerateAnchorButton(
                        htmlHelper.ViewContext.RequestContext, 
                        htmlHelper.RouteCollection, 
                        linkText, 
                        actionName, 
                        controllerName, 
                        null, 
                        null));
        }

        /// <summary>
        /// The link button.
        /// </summary>
        /// <param name="htmlHelper">
        /// The html helper.
        /// </param>
        /// <param name="linkText">
        /// The link text.
        /// </param>
        /// <param name="htmlAttributes">
        /// The html attributes.
        /// </param>
        /// <returns>
        /// The <see cref="MvcHtmlString"/>.
        /// </returns>
        public static MvcHtmlString LinkButton(this HtmlHelper htmlHelper, string linkText, object htmlAttributes)
        {
            return MvcHtmlString.Create(GenerateLinkButton(linkText, new RouteValueDictionary(htmlAttributes)));
        }

        #endregion

        #region Methods

        /// <summary>
        /// The generate anchor button.
        /// </summary>
        /// <param name="requestContext">
        /// The request context.
        /// </param>
        /// <param name="routeCollection">
        /// The route collection.
        /// </param>
        /// <param name="linkText">
        /// The link text.
        /// </param>
        /// <param name="actionName">
        /// The action name.
        /// </param>
        /// <param name="controllerName">
        /// The controller name.
        /// </param>
        /// <param name="routeValues">
        /// The route values.
        /// </param>
        /// <param name="htmlAttributes">
        /// The html attributes.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string GenerateAnchorButton(
            RequestContext requestContext, 
            RouteCollection routeCollection, 
            string linkText, 
            string actionName, 
            string controllerName, 
            RouteValueDictionary routeValues, 
            IDictionary<string, object> htmlAttributes)
        {
            string str = UrlHelper.GenerateUrl(
                null, actionName, controllerName, null, null, null, routeValues, routeCollection, requestContext, true);

            var hrefBuilder = new TagBuilder("a");
            var outerSpanBuilder = new TagBuilder("span");
            var spanBuilder = new TagBuilder("span")
                                  {
                                      InnerHtml =
                                          !string.IsNullOrEmpty(linkText)
                                              ? HttpUtility.HtmlEncode(linkText)
                                              : string.Empty
                                  };

            outerSpanBuilder.InnerHtml = spanBuilder.ToString(TagRenderMode.Normal);
            hrefBuilder.InnerHtml = outerSpanBuilder.ToString(TagRenderMode.Normal);

            hrefBuilder.MergeAttributes(htmlAttributes);
            hrefBuilder.MergeAttribute("href", str);

            return hrefBuilder.ToString(TagRenderMode.Normal);
        }

        /// <summary>
        /// The generate link button.
        /// </summary>
        /// <param name="linkText">
        /// The link text.
        /// </param>
        /// <param name="htmlAttributes">
        /// The html attributes.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string GenerateLinkButton(string linkText, IDictionary<string, object> htmlAttributes)
        {
            var hrefBuilder = new TagBuilder("a");
            var outerSpanBuilder = new TagBuilder("span");
            var spanBuilder = new TagBuilder("span")
                                  {
                                      InnerHtml =
                                          !string.IsNullOrEmpty(linkText)
                                              ? HttpUtility.HtmlEncode(linkText)
                                              : string.Empty
                                  };

            outerSpanBuilder.InnerHtml = spanBuilder.ToString(TagRenderMode.Normal);
            hrefBuilder.InnerHtml = outerSpanBuilder.ToString(TagRenderMode.Normal);

            hrefBuilder.MergeAttributes(htmlAttributes);
            hrefBuilder.MergeAttribute("href", "javascript:void(0)");

            if (!hrefBuilder.Attributes.ContainsKey("onclick"))
            {
                hrefBuilder.Attributes.Add("onclick", "$(this).closest('form').submit();");
            }
            else
            {
                hrefBuilder.Attributes["onclick"] = string.Format(
                    "if({0}) $(this).closest('form').submit();", hrefBuilder.Attributes["onclick"]);
            }

            return hrefBuilder.ToString(TagRenderMode.Normal);
        }

        #endregion
    }
}