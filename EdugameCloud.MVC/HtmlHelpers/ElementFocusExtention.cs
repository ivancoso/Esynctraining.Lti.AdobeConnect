namespace EdugameCloud.MVC.HtmlHelpers
{
    using System;
    using System.Linq.Expressions;
    using System.Web.Mvc;

    using Esynctraining.Core.Utils;

    /// <summary>
    /// The element focus extention.
    /// </summary>
    public static class ElementFocusExtention
    {
        #region Public Methods and Operators

        /// <summary>
        /// The focus element.
        /// </summary>
        /// <param name="htmlHelper">
        /// The html helper.
        /// </param>
        /// <param name="expression">
        /// The expression.
        /// </param>
        /// <typeparam name="TModel">
        /// Model type
        /// </typeparam>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static MvcHtmlString FocusElement<TModel>(
            this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, object>> expression)
        {
            var script = new TagBuilder("script");
            script.Attributes["type"] = "text/javascript";
            script.InnerHtml = string.Format(
                "$(document).ready(function () {0} {1} {2});", 
                "{", 
                string.Format("FocusElement('{0}');", Lambda.Property(expression)), 
                "}");
            return MvcHtmlString.Create(script.ToString(TagRenderMode.Normal));
        }

        #endregion
    }
}