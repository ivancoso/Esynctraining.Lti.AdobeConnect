// ReSharper disable once CheckNamespace
namespace EdugameCloud.WCFService
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Mail;
    using System.ServiceModel;
    using System.ServiceModel.Activation;
    using System.Text.RegularExpressions;

    using EdugameCloud.Core.Business.Models;
    using EdugameCloud.Core.Contracts;
    using EdugameCloud.Core.Domain.DTO;
    using EdugameCloud.Core.Domain.Entities;
    using EdugameCloud.WCFService.Base;

    using Esynctraining.Core.Domain.Contracts;
    using Esynctraining.Core.Extensions;
    using Esynctraining.Core.Utils;

    using FluentValidation.Results;

    using Resources;

    /// <summary>
    ///     The email service.
    /// </summary>
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.PerSession,
        IncludeExceptionDetailInFaults = true)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class EmailService : BaseService, IEmailService
    {
        #region Properties

        /// <summary>
        /// Gets the newsletter subscription model.
        /// </summary>
        private NewsletterSubscriptionModel NewsletterSubscriptionModel
        {
            get
            {
                return IoC.Resolve<NewsletterSubscriptionModel>();
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Logs error to server log.
        /// </summary>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>      

        public ServiceResponse<EmailHistoryDTO> GetHistory()
        {
            return new ServiceResponse<EmailHistoryDTO>
            {
                objects = this.EmailHistoryModel.GetAll().Select(x => new EmailHistoryDTO(x)).ToList()
            };
        }

        /// <summary>
        /// The save update.
        /// </summary>
        /// <param name="dto">
        /// The DTO.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<EmailHistoryDTO> SaveHistory(EmailHistoryDTO dto)
        {
            var result = new ServiceResponse<EmailHistoryDTO>();
            ValidationResult validationResult;
            if (this.IsValid(dto, out validationResult))
            {
                if (dto.body != null)
                {
                    var message = dto.body;
                    if (message != null)
                    {
                        message = Regex.Replace(message, "<[^>]*(>|$)", "");
                        message = message.Replace("\r\n", "\n").Replace("\r", "\n").Replace("&nbsp;", " ").Replace("&#39;", @"'");
                        message = Regex.Replace(message, @"[ ]{2,}", " ");
                        message = message.Replace("\n ", "\n");
                        message = Regex.Replace(message, @"[\n]{2,}", "\n");
                        message =
                            message.Replace(
                                "Enriching online interaction for meetings, training and education on Adobe Connect",
                                "");
                        while (message.StartsWith("\n")) message = message.Remove(0, 1);
                    }
                    dto.message = message;
                }
                var isTransient = dto.emailHistoryId == 0;
                var emailHistory = isTransient ? null : EmailHistoryModel.GetOneById(dto.emailHistoryId).Value;
                emailHistory = this.ConvertDto(dto, emailHistory);

                this.EmailHistoryModel.RegisterSave(emailHistory, true);
                //IoC.Resolve<RTMPModel>().NotifyClientsAboutChangesInTable<EmailHistory>(NotificationType.Update, emailHistory.Id);

                result.@object = new EmailHistoryDTO(emailHistory);
                return result;
            }

            result = this.UpdateResult(result, validationResult);
            this.LogError(ErrorsTexts.EntityCreationError_Subject, result, string.Empty);
            return result;
        }

        public ServiceResponse<EmailHistoryDTO> GetHistoryByCompanyId(int companyId)
        {
            return new ServiceResponse<EmailHistoryDTO>
            {
                objects = this.EmailHistoryModel
                .GetAll()
                .Where(x => (x != null && x.User != null && x.User.Company != null ? x.User.Company.Id : 0) == companyId)
                .Select(x => new EmailHistoryDTO(x)).ToList()
            };
        }

        public ServiceResponse<EmailHistoryDTO> ResendEmail(int emailHistoryId)
        {
            var response = new ServiceResponse<EmailHistoryDTO>();

            var item = EmailHistoryModel.GetOneById(emailHistoryId);
            if (item != null && item.Value != null)
            {
                MailModel.SendEmail(
                    item.Value.SentToName != null ? item.Value.SentToName.Split(';').ToArray() : new string[]{ },
                    item.Value.SentTo != null ? item.Value.SentTo.Split(';').ToArray() : new string[] { },
                    item.Value.Subject,
                    item.Value.Body,
                    item.Value.SentFromName,
                    item.Value.SentFrom,
                    null as IEnumerable<Attachment>,
                    item.Value.SentCc != null ? 
                        item.Value.SentCc.Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                        .Where(i => !string.IsNullOrWhiteSpace(i))
                        .Select(i => new MailAddress(i, i))
                        .ToList() : new List<MailAddress>(),
                    item.Value.SentCc != null ? 
                        item.Value.SentCc.Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                        .Where(i => !string.IsNullOrWhiteSpace(i))
                        .Select(i => new MailAddress(i, i))
                        .ToList() : new List<MailAddress>(),
                    null as List<LinkedResource>);

                var newHistoryItem = item.Value.DeepClone();
                newHistoryItem.Date = DateTime.Now;
                newHistoryItem.Id = 0;
                newHistoryItem.User = item.Value.User;
                var newHistory = new EmailHistoryDTO(newHistoryItem);

                var res = this.SaveHistory(newHistory);
                if (res.error != null)
                {
                    response.SetError(res.error);
                }
                else
                {
                    response.@object = res.@object;
                }
            }
            else
            {
                response.SetError("No item with such id found", new Exception());
            }

            return response;
        }

        /// <summary>
        /// Logs error to server log.
        /// </summary>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>      

        public ServiceResponse<NewsletterSubscriptionDTO> GetNewsletterSubscription()
        {
            return new ServiceResponse<NewsletterSubscriptionDTO>
            {
                objects = this.NewsletterSubscriptionModel.GetAll().Select(x => new NewsletterSubscriptionDTO(x)).ToList()
            };
        }

        /// <summary>
        /// The subscribe.
        /// </summary>
        /// <param name="email">
        /// The email.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<NewsletterSubscriptionDTO> SubscribeToNewsletter(string email)
        {
            var result = new ServiceResponse<NewsletterSubscriptionDTO>();
            ValidationResult validationResult;
            var dto = new NewsletterSubscriptionDTO() { email = email, dateSubscribed = DateTime.Now, isActive = true };
            if (this.IsValid(dto, out validationResult))
            {
                NewsletterSubscription newsletterSubscription = null;
                newsletterSubscription = this.ConvertDto(dto, newsletterSubscription);

                this.NewsletterSubscriptionModel.RegisterSave(newsletterSubscription, true);
                //IoC.Resolve<RTMPModel>().NotifyClientsAboutChangesInTable<EmailHistory>(NotificationType.Update, emailHistory.Id);

                result.@object = dto;
                return result;
            }

            result = this.UpdateResult(result, validationResult);
            this.LogError(ErrorsTexts.EntityCreationError_Subject, result, string.Empty);
            return result;
        }

        /// <summary>
        /// The subscribe.
        /// </summary>
        /// <param name="email">
        /// The email.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<UserDTO> SetUserUnsubscribed(string email)
        {
            var users = UserModel.GetAll().Where(u => u.Email == email).ToList();
            foreach (var user in users)
            {
                user.IsUnsubscribed = true;
                UserModel.RegisterSave(user, true);
            }
            var result = new ServiceResponse<UserDTO>
                         {
                             objects = users.Select(u => new UserDTO(u))
                         };

            return result;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// The convert DTO.
        /// </summary>
        /// <param name="q">
        /// The result DTO.
        /// </param>
        /// <param name="instance">
        /// The instance.
        /// </param>
        /// <returns>
        /// The <see cref="SubModuleCategory"/>.
        /// </returns>
        private EmailHistory ConvertDto(EmailHistoryDTO h, EmailHistory instance)
        {
            instance = instance ?? new EmailHistory();
            instance.Body = h.body;
            instance.Date = h.date;
            instance.Message = h.message;
            instance.SentBcc = h.sentBcc;
            instance.SentCc = h.sentCc;
            instance.SentFrom = h.sentFrom;
            instance.SentTo = h.sentTo;
            instance.Subject = h.subject;
            instance.User = this.UserModel.GetOneByEmail(h.sentTo).Value;

            return instance;
        }

        /// <summary>
        /// The convert DTO.
        /// </summary>
        /// <param name="h">
        /// The result DTO.
        /// </param>
        /// <param name="instance">
        /// The instance.
        /// </param>
        /// <returns>
        /// The <see cref="SubModuleCategory"/>.
        /// </returns>
        private NewsletterSubscription ConvertDto(NewsletterSubscriptionDTO h, NewsletterSubscription instance)
        {
            instance = instance ?? new NewsletterSubscription();
            instance.Email = h.email;
            instance.IsActive = h.isActive;
            instance.DateSubscribed = h.dateSubscribed;
            instance.DateUnsubscribed = h.dateUnsubscribed;

            return instance;
        }

        #endregion
    }
}