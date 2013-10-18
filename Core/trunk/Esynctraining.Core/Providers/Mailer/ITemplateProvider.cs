namespace Esynctraining.Core.Providers.Mailer
{

    /// <summary>
    /// Defines contracts for getting templates
    /// </summary>
    public interface ITemplateProvider
    {
        /// <summary>
        /// Gets the template transformer with template that is got by model type.
        /// </summary>
        /// <typeparam name="TTemplateModel">The type of the template model.</typeparam>
        /// <returns>template transformer</returns>
        ITemplateTransformer GetTemplate<TTemplateModel>();
        
        /// <summary>
        /// Gets the template transformer with template that is got by name.
        /// </summary>
        /// <param name="templateName">Name of the template.</param>
        /// <returns>template  transformer</returns>
        ITemplateTransformer GetTemplate(string templateName);
    }
}
