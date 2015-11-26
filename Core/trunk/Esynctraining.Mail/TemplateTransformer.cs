using RazorEngine;

namespace Esynctraining.Mail
{
    /// <summary>
    /// Transforms template
    /// </summary>
    public class TemplateTransformer : ITemplateTransformer
    {
        /// <summary>
        /// The template.
        /// </summary>
        private readonly string template;

        /// <summary>
        /// The ascii template.
        /// </summary>
        private readonly string asciiTemplate;

        /// <summary>
        /// Initializes a new instance of the <see cref="TemplateTransformer"/> class.
        /// </summary>
        /// <param name="template">
        /// The template to transform
        /// </param>
        /// <param name="asciiTemplate">
        /// The ascii Template.
        /// </param>
        public TemplateTransformer(string template, string asciiTemplate = null)
        {
            this.template = template;
            this.asciiTemplate = asciiTemplate;
        }

        #region Implementation of ITemplateTransformer

        /// <summary>
        /// Transforms the template.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>transformed template</returns>
        public string TransformAsciiTemplate(dynamic model)
        {
            if (!string.IsNullOrWhiteSpace(this.asciiTemplate))
            {
                return Razor.Parse(this.asciiTemplate, model);
            }
            return null;
        }

        /// <summary>
        /// Transforms the template.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>transformed template</returns>
        public string TransformTemplate(dynamic model)
        {
            return Razor.Parse(this.template, model);
        }

        #endregion

    }

}
