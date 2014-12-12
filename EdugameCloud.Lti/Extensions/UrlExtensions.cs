namespace EdugameCloud.Lti.Extensions
{
    using System;
    using System.Web.Mvc;


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


        #endregion
    }
}