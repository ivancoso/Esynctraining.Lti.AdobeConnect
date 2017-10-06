namespace AnonymousChat.Web.Services
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Net.Mail;
    using System.Web.Services;

    using AnonymousChat.Web.DTO;
    using AnonymousChat.Web.Mail.Models;

    using Esynctraining.Core.Business.Models;
    using Esynctraining.Core.Providers;
    using Esynctraining.Core.Utils;

    using Resources;

    /// <summary>
    /// Summary description for ChatHistoryService
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1608:ElementDocumentationMustNotHaveDefaultSummary", Justification = "Reviewed. Suppression is OK here."), WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    public class ChatHistoryService : WebService
    {
        /// <summary>
        /// Gets the mail model.
        /// </summary>
        public MailModel MailModel
        {
            get
            {
                return IoC.Resolve<MailModel>();
            }
        }

        /// <summary>
        /// Gets the settings.
        /// </summary>
        public ApplicationSettingsProvider Settings
        {
            get
            {
                return IoC.Resolve<ApplicationSettingsProvider>();
            }
        }

        /// <summary>
        /// The send chat history.
        /// </summary>
        /// <param name="dto">
        /// The DTO.
        /// </param>
        [WebMethod]
        public void SendChatHistory(ChatHistoryEmailDTO dto)
        {
            if (dto != null)
            {
                var cced = dto.CcEmails != null && dto.CcEmails.Any()
                               ? dto.CcEmails.Select(x => new MailAddress(x.Email, x.Name)).ToList()
                               : null;
                var bcced = dto.BccEmails != null && dto.BccEmails.Any()
                                ? dto.BccEmails.Select(x => new MailAddress(x.Email, x.Name)).ToList()
                                : null;
                var mailModel = this.MailModel;
                mailModel.SendEmail(
                    dto.ToEmails.Select(x => x.Name).ToList(),
                    dto.ToEmails.Select(x => x.Email).ToList(),
                    dto.Subject,
                    new ChatHistoryModel(this.Settings) { MailBody = dto.BodyHTML, MailSubject = dto.Subject },
                    Common.AppEmailName,
                    Common.AppEmail,
                    cced,
                    bcced);
            }
            else
            {
                throw new ApplicationException("DTO is null");
            }
        }
    }
}
