namespace EdugameCloud.Core.Providers.Mailer
{
    /// <summary>
    /// Defines contracts for template transformation
    /// </summary>
    public interface ITemplateTransformer
    {
        /// <summary>
        /// Transforms the template.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>transformed template</returns>
        string TransformTemplate(dynamic model);
    }
}
