using System.Collections.Generic;
using System.Net.Mail;

namespace Esynctraining.Mail
{
    public interface ISmtpClientEngine
    {
        bool SendEmail(
            IEnumerable<string> toName,
            IEnumerable<string> toEmail,
            string subject,
            string body,
            string fromName,
            string fromEmail,
            IEnumerable<Attachment> attachments,
            IEnumerable<MailAddress> cced = null,
            IEnumerable<MailAddress> bcced = null,
            IEnumerable<LinkedResource> linkedResources = null);

        bool SendEmail<TModel>(IEnumerable<string> toName, IEnumerable<string> toEmail, string subject, TModel model, string fromName = null, string fromEmail = null,
            IEnumerable<MailAddress> cced = null,
            IEnumerable<MailAddress> bcced = null,
            IEnumerable<Attachment> attachments = null,
            IEnumerable<LinkedResource> linkedResources = null);

        bool SendEmailSync<TModel>(string toName, string toEmail, string subject, TModel model, string fromName = null, string fromEmail = null, 
            IEnumerable<MailAddress> cced = null, 
            IEnumerable<MailAddress> bcced = null,
            IEnumerable<Attachment> attachments = null,
            IEnumerable<LinkedResource> linkedResources = null);

        bool SendEmailSync<TModel>(string subject, TModel model,
            MailAddress from,
            IEnumerable<MailAddress> to,
            IEnumerable<MailAddress> cced = null,
            IEnumerable<MailAddress> bcced = null);

    }

}
