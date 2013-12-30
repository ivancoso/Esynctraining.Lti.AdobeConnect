namespace EdugameCloud.MVC.HtmlHelpers
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Web.Mvc;
    using System.Web.Routing;
    using System.Web.WebPages;

    /// <summary>
    /// The validated label extensions.
    /// </summary>
    public static class ValidatedLabelExtensions
    {
        #region Public Methods and Operators

        /// <summary>
        /// The validated label for.
        /// </summary>
        /// <param name="htmlHelper">
        /// The html helper.
        /// </param>
        /// <param name="expression">
        /// The expression.
        /// </param>
        /// <param name="template">
        /// The template.
        /// </param>
        /// <param name="htmlAttributes">
        /// The html attributes.
        /// </param>
        /// <typeparam name="TModel">
        /// Model type
        /// </typeparam>
        /// <typeparam name="TValue">
        /// Value type
        /// </typeparam>
        /// <returns>
        /// The <see cref="MvcHtmlString"/>.
        /// </returns>
        public static MvcHtmlString ValidatedLabelFor<TModel, TValue>(
            this HtmlHelper<TModel> htmlHelper, 
            Expression<Func<TModel, TValue>> expression, 
            Func<dynamic, HelperResult> template = null, 
            object htmlAttributes = null)
        {
            string fieldName = ExpressionHelper.GetExpressionText(expression);
            string name = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(fieldName);

            var metadata = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);

            string fieldTitle = metadata.DisplayName ?? (metadata.PropertyName ?? fieldName.Split(new[] { '.' }).ToList().Last());
            if (string.IsNullOrEmpty(fieldTitle))
            {
                return MvcHtmlString.Empty;
            }

            var builder = new TagBuilder("label");
            builder.Attributes.Add("for", htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldId(fieldName));
            builder.SetInnerText(fieldTitle);

            if (template != null)
            {
                builder.InnerHtml += " " + template(null).ToHtmlString();
            }

            if (htmlAttributes != null)
            {
                builder.MergeAttributes(new RouteValueDictionary(htmlAttributes));
            }

            ModelState state;
            if (htmlHelper.ViewData.ModelState.TryGetValue(name, out state) && (state.Errors.Count > 0))
            {
                var errorBuilder = new TagBuilder("label");
                errorBuilder.AddCssClass("error");
                errorBuilder.SetInnerText(state.Errors.First().ErrorMessage);
                return MvcHtmlString.Create(builder.ToString(TagRenderMode.Normal) + "&nbsp;&nbsp;" + errorBuilder.ToString(TagRenderMode.Normal));
            }

            return MvcHtmlString.Create(builder.ToString(TagRenderMode.Normal));
        }

        #endregion
    }
}