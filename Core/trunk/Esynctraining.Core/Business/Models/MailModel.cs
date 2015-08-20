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
            List<MailAddress> bcced = null,
            List<LinkedResource> linkedResources = null)
        {
            using (var smtpClientWrapper = new SmtpClientWrapper(new SmtpClient()))
            {
                return this.SendEmail(smtpClientWrapper, toName, toEmail, subject, body, fromName, fromEmail, attachments, cced, bcced, linkedResources);
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
            List<LinkedResource> linkedResources = null,
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

                if (linkedResources != null && linkedResources.Any())
                {
                    var htmlView = AlternateView.CreateAlternateViewFromString(body, null, "text/html");
                    foreach (var linkedResource in linkedResources)
                    {
                        htmlView.LinkedResources.Add(linkedResource);    
                    }

                    message.AlternateViews.Add(htmlView);
                }

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

        public bool SendEmailSync(IEnumerable<string> toName, IEnumerable<string> toEmail, string subject, string body, string fromName, string fromEmail, IEnumerable<Attachment> attachments, List<MailAddress> cced = null, List<MailAddress> bcced = null, List<LinkedResource> linkedResources = null)
        {
            bool flag;
            using (SmtpClientWrapper smtpClientWrapper = new SmtpClientWrapper(new SmtpClient()))
            {
                flag = this.SendEmailSync(smtpClientWrapper, toName, toEmail, subject, body, fromName, fromEmail, attachments, cced, bcced, linkedResources, false);
            }
            return flag;
        }

        public bool SendEmailSync<TModel>(IEnumerable<string> toName, IEnumerable<string> toEmail, string subject, TModel model, string fromName = null, string fromEmail = null, List<MailAddress> cced = null, List<MailAddress> bcced = null, List<Attachment> attachments = null, List<LinkedResource> linkedResources = null)
        {
            string str = this.templateProvider.GetTemplate<TModel>().TransformTemplate(model);
            IEnumerable<string> strs = toName;
            IEnumerable<string> strs1 = toEmail;
            string str1 = subject;
            string str2 = str;
            string str3 = fromName;
            string str4 = fromEmail;
            object obj = attachments;
            if (obj == null)
            {
                obj = this.attachmentsProvider.GetAttachments<TModel>();
            }
            return this.SendEmailSync(strs, strs1, str1, str2, str3, str4, (IEnumerable<Attachment>)obj, cced, bcced, linkedResources);
        }

        public bool SendEmailSync<TModel>(string toName, string toEmail, string subject, TModel model, string fromName = null, string fromEmail = null, List<MailAddress> cced = null, List<MailAddress> bcced = null, List<Attachment> attachments = null, List<LinkedResource> linkedResources = null)
        {
            string[] strArrays = new string[] { toName };
            string[] strArrays1 = new string[] { toEmail };
            return this.SendEmailSync<TModel>(strArrays, strArrays1, subject, model, fromName, fromEmail, cced, bcced, attachments, linkedResources);
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
        public bool SendEmail<TModel>(IEnumerable<string> toName, IEnumerable<string> toEmail, string subject, TModel model, string fromName = null, string fromEmail = null, List<MailAddress> cced = null, List<MailAddress> bcced = null, List<Attachment> attachments = null, List<LinkedResource> linkedResources = null)
        {
            var message = this.templateProvider.GetTemplate<TModel>().TransformTemplate(model);
            return this.SendEmail(toName, toEmail, subject, message, fromName, fromEmail, attachments ?? this.attachmentsProvider.GetAttachments<TModel>(), cced, bcced, linkedResources);
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
        /// <param name="linkedResources">
        /// The linked Resources.
        /// </param>
        /// <typeparam name="TModel">
        /// Mail model
        /// </typeparam>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool SendEmail<TModel>(string toName, string toEmail, string subject, TModel model, string fromName = null, string fromEmail = null, List<MailAddress> cced = null, List<MailAddress> bcced = null, List<Attachment> attachments = null, List<LinkedResource> linkedResources = null)
        {
            return this.SendEmail(new[] { toName }, new[] { toEmail }, subject, model, fromName, fromEmail, cced, bcced, attachments, linkedResources);
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

        private bool SendEmailSync(SmtpClientWrapper smtpClientWrapper, IEnumerable<string> toName, IEnumerable<string> toEmail, string subject, string body, string fromName, string fromEmail, IEnumerable<Attachment> attachments, List<MailAddress> cced = null, List<MailAddress> bcced = null, List<LinkedResource> linkedResources = null, bool useSsl = false)
        {
            bool flag;
            try
            {
                MailMessage mailMessage = new MailMessage()
                {
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };
                MailMessage mailAddress = mailMessage;
                if (linkedResources != null && linkedResources.Any<LinkedResource>())
                {
                    AlternateView alternateView = AlternateView.CreateAlternateViewFromString(body, null, "text/html");
                    foreach (LinkedResource linkedResource in linkedResources)
                    {
                        alternateView.LinkedResources.Add(linkedResource);
                    }
                    mailAddress.AlternateViews.Add(alternateView);
                }
                List<string> list = toEmail.ToList<string>();
                List<string> strs = toName.With<IEnumerable<string>, List<string>>((IEnumerable<string> x) => x.ToList<string>());
                for (int i = 0; i < list.Count<string>(); i++)
                {
                    int num = i;
                    string str = strs.With<List<string>, string>((List<string> x) => x.ElementAtOrDefault<string>(num) ?? string.Empty);
                    string str1 = list.ElementAt<string>(num);
                    if (!string.IsNullOrWhiteSpace(str1))
                    {
                        mailAddress.To.Add(new MailAddress(str1, str));
                    }
                }
                if (!string.IsNullOrWhiteSpace(fromName) && !string.IsNullOrWhiteSpace(fromEmail))
                {
                    mailAddress.From = new MailAddress(fromEmail, fromName);
                }
                if (attachments != null)
                {
                    foreach (Attachment attachment in attachments)
                    {
                        mailAddress.Attachments.Add(attachment);
                    }
                }
                if (cced != null && cced.Any<MailAddress>())
                {
                    foreach (MailAddress mailAddress1 in cced)
                    {
                        mailAddress.CC.Add(mailAddress1);
                    }
                }
                if (bcced != null && bcced.Any<MailAddress>())
                {
                    foreach (MailAddress mailAddress2 in bcced)
                    {
                        mailAddress.Bcc.Add(mailAddress2);
                    }
                }
                if (useSsl)
                {
                    smtpClientWrapper.EnableSsl = true;
                }
                smtpClientWrapper.Send(mailAddress);
                flag = true;
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                this.LastError = exception;
                IoC.Resolve<ILogger>().Error("Error, while sending email", exception);
                flag = false;
            }
            return flag;
        }

        #endregion

    }

}