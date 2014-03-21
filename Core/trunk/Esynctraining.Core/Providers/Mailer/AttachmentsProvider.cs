using System.Net.Mime;
using Esynctraining.Core.Utils;

namespace Esynctraining.Core.Providers.Mailer
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net.Mail;
    using System.Threading;
    using System.Web;

    using Esynctraining.Core.Extensions;
    using Esynctraining.Core.Providers.Mailer.Configuration;

    /// <summary>
    ///     Defines contracts for getting attachments
    /// </summary>
    public class AttachmentsProvider : IAttachmentsProvider
    {
        #region Fields

        /// <summary>
        /// The attachments directory.
        /// </summary>
        private readonly string attachmentsDirectory;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AttachmentsProvider" /> class. Gets templates folder path from configuration file
        /// </summary>
        public AttachmentsProvider()
        {
            this.attachmentsDirectory = MailerConfigurationSection.Current.With(x => x.AttachmentsFolderPath);
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Gets the attachments collection with template that is got by model type.
        /// </summary>
        /// <typeparam name="TTemplateModel">The type of the template model.</typeparam>
        /// <returns>
        /// Instance of the <see cref="IEnumerable{Attachment}" /> with loaded attachments
        /// </returns>
        public IEnumerable<Attachment> GetAttachments<TTemplateModel>()
        {
            var attachments = this.LoadTemplateAttachments(typeof(TTemplateModel));
	        if (attachments != null)
	        {
		        return
			        attachments.Select(
				        attachment =>
				        {
					        var at =  new Attachment(attachment);
					        at.ContentDisposition.Inline = true;
					        at.ContentId = typeof (TTemplateModel).Name.Replace("Model", string.Empty);
					        return at;

				        }
	        ).
	        ToList();
                }
            return null;
        }

        /// <summary>
        /// Gets the attachments collection with template that is got by model type.
        /// </summary>
        /// <param name="templateName">
        /// The template name.
        /// </param>
        /// <returns>
        /// Instance of the <see cref="IEnumerable{Attachment}"/> with loaded attachments
        /// </returns>
        public IEnumerable<Attachment> GetAttachments(string templateName)
        {
            if (!string.IsNullOrWhiteSpace(this.attachmentsDirectory))
            {
                var attachments = this.LoadTemplateAttachments(templateName);
                if (attachments != null)
                {
                    return attachments.Select(attachment => new Attachment(attachment)).ToList();
                }
            }
            return null;
        }

        #endregion

        #region Methods

        /// <summary>
        /// The get default template file name.
        /// </summary>
        /// <param name="templateType">
        /// The template type.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string GetDefaultTemplateAttachmentsFolderName(Type templateType)
        {
            return string.Format("{0}", templateType.Name.Replace("Model", string.Empty));
        }

        /// <summary>
        /// The get default template file name.
        /// </summary>
        /// <param name="templateName">
        /// The template name.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string GetDefaultTemplateAttachmentsFolderName(string templateName)
        {
            return string.Format("{0}", templateName);
        }

        /// <summary>
        /// The get full path to template.
        /// </summary>
        /// <param name="templateAttachmentsFolderName">
        /// The template file name.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string GetFullPathToTemplate(string templateAttachmentsFolderName)
        {
            if (HttpContext.Current != null)
            {
                return HttpContext.Current.Server.MapPath(VirtualPathUtility.Combine(this.attachmentsDirectory, templateAttachmentsFolderName));
            }
			return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, templateAttachmentsFolderName);
        }

        /// <summary>
        /// The get template file name.
        /// </summary>
        /// <param name="templateType">
        /// The template type.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string GetTemplateAttachmentFolderName(Type templateType)
        {
            string cultureName = Thread.CurrentThread.CurrentCulture.Name;

            return string.Format("{0}.{1}", templateType.Name.Replace("Model", string.Empty), cultureName);
        }

        /// <summary>
        /// The get template file name.
        /// </summary>
        /// <param name="templateName">
        /// The template name.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string GetTemplateAttachmentFolderName(string templateName)
        {
            string cultureName = Thread.CurrentThread.CurrentCulture.Name;

            return string.Format("{0}.{1}", templateName, cultureName);
        }

        /// <summary>
        /// The load template attachments.
        /// </summary>
        /// <param name="templateName">
        /// The template name.
        /// </param>
        /// <returns>
        /// The <see cref="string"/> array of files.
        /// </returns>
        private IEnumerable<string> LoadTemplateAttachments(string templateName)
        {
            string attachmentFolderPath = this.GetFullPathToTemplate(this.GetTemplateAttachmentFolderName(templateName));
            if (Directory.Exists(attachmentFolderPath))
            {
                return Directory.GetFiles(attachmentFolderPath);
            }
            attachmentFolderPath = this.GetFullPathToTemplate(this.GetDefaultTemplateAttachmentsFolderName(templateName));
            if (Directory.Exists(attachmentFolderPath))
            {
                return Directory.GetFiles(attachmentFolderPath);
            }

            return null;
        }

        /// <summary>
        /// The load template attachments.
        /// </summary>
        /// <param name="templateType">
        /// The template type.
        /// </param>
        /// <returns>
        /// The <see cref="string"/> array of files.
        /// </returns>
        private IEnumerable<string> LoadTemplateAttachments(Type templateType)
        {
            string attachmentFolderPath = this.GetFullPathToTemplate(this.GetTemplateAttachmentFolderName(templateType));

            if (Directory.Exists(attachmentFolderPath))
            {
                return Directory.GetFiles(attachmentFolderPath);
            }
            attachmentFolderPath = this.GetFullPathToTemplate(this.GetDefaultTemplateAttachmentsFolderName(templateType));
            if (Directory.Exists(attachmentFolderPath))
            {
                return Directory.GetFiles(attachmentFolderPath);
            }
            return null;
        }

        #endregion
    }
}