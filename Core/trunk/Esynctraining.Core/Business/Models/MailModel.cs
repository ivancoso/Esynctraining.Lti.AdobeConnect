namespace Esynctraining.Core.Business.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Net.Mail;

    using Castle.Core.Logging;

    using Esynctraining.Core.Extensions;
    using Esynctraining.Core.Providers.Mailer;
    using Esynctraining.Core.Utils;
    using Esynctraining.Core.Wrappers;

    using MailMessage = System.Net.Mail.MailMessage;

    /// <summary>
    /// The mail model.
    /// </summary>
    public class MailModel
    {
        #region Fields

        /// <summary>
        /// The template provider.
        /// </summary>
        private readonly ITemplateProvider templateProvider;

        /// <summary>
        /// The attachments provider.
        /// </summary>
        private readonly IAttachmentsProvider attachmentsProvider;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MailModel"/> class.
        /// </summary>
        /// <param name="templateProvider">
        /// The template provider.
        /// </param>
        /// <param name="attachmentsProvider">
        /// The attachments Provider.
        /// </param>
        public MailModel(ITemplateProvider templateProvider, IAttachmentsProvider attachmentsProvider)
        {
            this .templateProvider = templateProvider;
            this.attachmentsProvider = attachmentsProvider;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the last error.
        /// </summary>
        public Exception LastError { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The send email.
        /// </summary>
        /// <param name="toName">
        /// The to name.
        /// </param>
        /// <param name="toEmail">
        /// The to email.
        /// </param>
        /// <param name="subject">
        /// The subject.
        /// </param>
        /// <param name="body">
        /// The body.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool SendEmail(IEnumerable<string> toName, IEnumerable<string> toEmail, string subject, string body)
        {
            return this.SendEmail(toName, toEmail, subject, body, null);
        }

        /// <summary>
        /// The send email.
        /// </summary>
        /// <param name="toName">
        /// The to name.
        /// </param>
        /// <param name="toEmail">
        /// The to email.
        /// </param>
        /// <param name="subject">
        /// The subject.
        /// </param>
        /// <param name="body">
        /// The body.
        /// </param>
        /// <param name="fromName">
        /// The from name.
        /// </param>
        /// <param name="fromEmail">
        /// The from email.
        /// </param>
        /// <param name="attachments">
        /// Attachments list
        /// </param>
        /// <param name="cced">
        /// The copied.
        /// </param>
        /// <param name="bcced">
        /// The hidden.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool SendEmail(
            IEnumerable<string> toName, 
            IEnumerable<string> toEmail, 
            string subject, 
            string body, 
            string fromName, 
            string fromEmail,
            IEnumerable<Attachment> attachments,
            List<MailAddress> cced = null, 
            List<MailAddress> bcced = null)
        {
            using (var smtpClientWrapper = new SmtpClientWrapper(new SmtpClient()))
            {
                return this.SendEmail(smtpClientWrapper, toName, toEmail, subject, body, fromName, fromEmail, attachments, cced, bcced);
            }
        }

        /// <summary>
        /// The send email.
        /// </summary>
        /// <param name="smtpClientWrapper">
        /// The simple mail transfer protocol client wrapper.
        /// </param>
        /// <param name="toName">
        /// The to name.
        /// </param>
        /// <param name="toEmail">
        /// The to email.
        /// </param>
        /// <param name="subject">
        /// The subject.
        /// </param>
        /// <param name="body">
        /// The body.
        /// </param>
        /// <param name="fromName">
        /// The from name.
        /// </param>
        /// <param name="fromEmail">
        /// The from email.
        /// </param>
        /// <param name="attachments">
        /// The attachments
        /// </param>
        /// <param name="cced">
        /// The copied emails list.
        /// </param>
        /// <param name="bcced">
        /// The hidden copied emails list.
        /// </param>
        /// <param name="useSsl">
        /// The use SSL.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public virtual bool SendEmail(
            SmtpClientWrapper smtpClientWrapper, 
            IEnumerable<string> toName, 
            IEnumerable<string> toEmail, 
            string subject, 
            string body, 
            string fromName, 
            string fromEmail,
            IEnumerable<Attachment> attachments,
            List<MailAddress> cced = null, 
            List<MailAddress> bcced = null,
            bool useSsl = false)
        {
            try
            {
                var message = new MailMessage
                {
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };

                var toEmailList = toEmail.ToList();
                var toNameList = toName.With(x => x.ToList());

                for (var i = 0; i < toEmailList.Count(); i++)
                {
                    var index = i;
                    var name = toNameList.With(x => x.ElementAtOrDefault(index) ?? string.Empty);
                    var email = toEmailList.ElementAt(index);
                    if (!string.IsNullOrWhiteSpace(email))
                    {
                        message.To.Add(new MailAddress(email, name));
                    }
                }

                if (!string.IsNullOrWhiteSpace(fromName) && !string.IsNullOrWhiteSpace(fromEmail))
                {
                    message.From = new MailAddress(fromEmail, fromName);
                }

                if (attachments != null)
                {
                    foreach (var attachment in attachments)
                    {
                        message.Attachments.Add(attachment);
                    }
                }

                if (cced != null && cced.Any())
                {
                    foreach (var cc in cced)
                    {
                        message.CC.Add(cc);    
                    }
                }

                if (bcced != null && bcced.Any())
                {
                    foreach (var bcc in bcced)
                    {
                        message.Bcc.Add(bcc);
                    }
                }

                if (useSsl)
                {
                    smtpClientWrapper.EnableSsl = true;
                }

                smtpClientWrapper.SendCompleted += this.MailDeliveryComplete;
                var emails = toEmailList.ToPlainString();

                smtpClientWrapper.SendAsync(message, emails);
                return true;
            }
            catch (Exception ex)
            {
                this.LastError = ex;
                var logger = IoC.Resolve<ILogger>();
                logger.Error("Error, while sending email", ex);
                return false;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The send email.
        /// </summary>
        /// <param name="toName">
        /// The to name.
        /// </param>
        /// <param name="toEmail">
        /// The to email.
        /// </param>
        /// <param name="subject">
        /// The subject.
        /// </param>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <param name="fromName">
        /// The from Name.
        /// </param>
        /// <param name="fromEmail">
        /// The from Email.
        /// </param>
        /// <param name="cced">
        /// The copied.
        /// </param>
        /// <param name="bcced">
        /// The hidden.
        /// </param>
        /// <param name="attachments">
        /// The attachments.
        /// </param>
        /// <typeparam name="TModel">
        /// Any view model type
        /// </typeparam>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool SendEmail<TModel>(IEnumerable<string> toName, IEnumerable<string> toEmail, string subject, TModel model, string fromName = null, string fromEmail = null, List<MailAddress> cced = null, List<MailAddress> bcced = null, List<Attachment> attachments = null)
        {
            var message = this.templateProvider.GetTemplate<TModel>().TransformTemplate(model);
            return this.SendEmail(toName, toEmail, subject, message, fromName, fromEmail, attachments ?? this.attachmentsProvider.GetAttachments<TModel>(), cced, bcced);
        }

        /// <summary>
        /// The send email.
        /// </summary>
        /// <param name="toName">
        /// The to name.
        /// </param>
        /// <param name="toEmail">
        /// The to email.
        /// </param>
        /// <param name="subject">
        /// The subject.
        /// </param>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <param name="fromName">
        /// The from name.
        /// </param>
        /// <param name="fromEmail">
        /// The from email.
        /// </param>
        /// <param name="cced">
        /// The copied.
        /// </param>
        /// <param name="bcced">
        /// The hidden.
        /// </param>
        /// <param name="attachments">
        /// The attachments.
        /// </param>
        /// <typeparam name="TModel">
        /// Mail model
        /// </typeparam>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool SendEmail<TModel>(string toName, string toEmail, string subject, TModel model, string fromName = null, string fromEmail = null, List<MailAddress> cced = null, List<MailAddress> bcced = null, List<Attachment> attachments = null)
        {
            return this.SendEmail(new[] { toName }, new[] { toEmail }, subject, model, fromName, fromEmail, cced, bcced, attachments);
        }

        /// <summary>
        /// The mail delivery complete.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void MailDeliveryComplete(object sender, AsyncCompletedEventArgs e)
        {
            try
            {
                var logger = IoC.Resolve<ILogger>();    
                if (e.Error != null)
                {
                    logger.Error("Error, while sending email to: " + e.UserState, e.Error);
                }
                else if (e.Cancelled)
                {
                    logger.ErrorFormat("Sending email to {0} cancelled.", e.UserState);
                }
            }
                // ReSharper disable once EmptyGeneralCatchClause
            catch
            {
            }
        }

        #endregion
    }
}