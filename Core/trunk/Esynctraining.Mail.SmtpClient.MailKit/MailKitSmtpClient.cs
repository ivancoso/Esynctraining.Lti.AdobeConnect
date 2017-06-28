using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Esynctraining.Core.Logging;
using Esynctraining.Mail.Configuration;
using MimeKit;
using MailKitClient = MailKit.Net.Smtp.SmtpClient;

namespace Esynctraining.Mail.SmtpClient.MailKit
{
    public class MailKitSmtpClient : ISmtpClient
    {
        private readonly ITemplateTransformer _viewRenderService;
        private readonly ILogger _logger;
        private readonly ISmtpSettings _smtpSettings;


        public MailKitSmtpClient(ITemplateTransformer templateTransformer, ILogger logger, ISmtpSettings smtpSettings)
        {
            _viewRenderService = templateTransformer ?? throw new ArgumentNullException(nameof(templateTransformer));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _smtpSettings = smtpSettings ?? throw new ArgumentNullException(nameof(smtpSettings));
        }


        public async Task<bool> SendEmailAsync<TModel>(
            ISystemEmail from,
            IEnumerable<ISystemEmail> to,
            string subject,
            TModel model,
            IEnumerable<ISystemEmail> cced = null,
            IEnumerable<ISystemEmail> bcced = null,
            IEnumerable<Attachment> attachments = null)
        {
            if (from == null)
                throw new ArgumentNullException(nameof(from));
            if (to == null)
                throw new ArgumentNullException(nameof(to));
            if (string.IsNullOrWhiteSpace(subject))
                throw new ArgumentException("Non-empty value expected", nameof(subject));
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            var message = new MimeMessage
            {
                Subject = subject,
            };

            message.From.Add(new MailboxAddress(from.Name, from.Email));
            message.To.AddRange(to.Select(x => new MailboxAddress(x.Name, x.Email)));

            if (cced != null)
            {
                message.Cc.AddRange(cced.Select(x => new MailboxAddress(x.Name, x.Email)));
            }

            if (bcced != null)
            {
                message.Bcc.AddRange(bcced.Select(x => new MailboxAddress(x.Name, x.Email)));
            }

            string modelName = typeof(TModel).Name;
            if (!modelName.EndsWith("Model"))
                throw new InvalidOperationException("Mail model name should ends with 'Model'");
            string templateName = modelName.Substring(0, modelName.Length - "Model".Length);
            var body = await _viewRenderService.TransformAsync(templateName, model);

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = body,
            };

            if (attachments != null)
            {
                foreach (var att in attachments)
                {
                    bodyBuilder.Attachments.Add(att.FileName, att.Stream);
                }
            }

            message.Body = bodyBuilder.ToMessageBody();

            // NOTE: use to debug smtp: new ProtocolLogger("C:\\tmp\\MailKit.log")
            using (var client = new MailKitClient())
            {
                // TODO: add to config?  client.Timeout = 10 * 60 * 1000;
                client.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
                {
                    return true;
                };

                client.AuthenticationMechanisms.Remove("XOAUTH2");
                client.Connect(_smtpSettings.Host, _smtpSettings.Port);
                client.AuthenticationMechanisms.Remove("XOAUTH2");  // due to enabling less secure apps access

                try
                {
                    client.Authenticate(_smtpSettings.UserName, _smtpSettings.Password);
                    _logger.Debug("Smtp authentication done");
                }
                catch (Exception e)
                {
                    _logger.Error($"Smtp Authenticate failure, to: { string.Join(",", message.To.Mailboxes.Select(x => x.Address).ToArray()) }", e);
                    // TRICK: after this error we are still able to send email
                    //return false;
                }

                try
                {
                    client.Send(message);
                    _logger.Info($"Email was sent to: { string.Join(",", message.To.Mailboxes.Select(x => x.Address).ToArray()) }");
                }
                catch (Exception e)
                {
                    _logger.Error($"Email sending failure, to: { string.Join(",", message.To.Mailboxes.Select(x => x.Address).ToArray()) }", e);
                    return false;
                }

                client.Disconnect(true);
            }

            return true;
        }

    }

}