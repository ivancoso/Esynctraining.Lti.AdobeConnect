namespace EdugameCloud.Core.Providers.Mailer
{
    using System.Collections.Generic;
    using System.Net.Mail;

    /// <summary>
    /// Defines contracts for getting templates
    /// </summary>
    public interface IAttachmentsProvider
    {
        /// <summary>
        /// Gets the attachments collection with template that is got by model type.
        /// </summary>
        /// <typeparam name="TTemplateModel">The type of the template model.</typeparam>
        /// <returns>
        /// Instance of the <see cref="IEnumerable{Attachment}" /> with loaded attachments
        /// </returns>
        IEnumerable<Attachment> GetAttachments<TTemplateModel>();

        /// <summary>
        /// Gets the attachments collection with template that is got by model type.
        /// </summary>
        /// <param name="templateName">
        /// The template name.
        /// </param>
        /// <returns>
        /// Instance of the <see cref="IEnumerable{Attachment}"/> with loaded attachments
        /// </returns>
        IEnumerable<Attachment> GetAttachments(string templateName);
    }
}
