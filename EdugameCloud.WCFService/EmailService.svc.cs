// ReSharper disable once CheckNamespace

using System.Globalization;
using System.IO;
using EdugameCloud.Core.Business.Models;
using EdugameCloud.WCFService.Mail.Models;
using Esynctraining.AC.Provider;
using Esynctraining.AC.Provider.DataObjects;
using Esynctraining.AdobeConnect;
using Esynctraining.Core.Providers;
using Esynctraining.Mail;
using Ical.Net;
using Ical.Net.DataTypes;
using Ical.Net.Serialization;
using Ical.Net.Serialization.iCalendar.Serializers;
using Calendar = Ical.Net.Calendar;

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
    using EdugameCloud.Core.Domain.DTO;
    using EdugameCloud.Core.Domain.Entities;
    using EdugameCloud.WCFService.Base;
    using EdugameCloud.WCFService.Contracts;

    using Esynctraining.Core.Domain.Entities;
    using Esynctraining.Core.Enums;
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
        private NewsletterSubscriptionModel NewsletterSubscriptionModel => IoC.Resolve<NewsletterSubscriptionModel>();
        private QuizResultModel QuizResultModel => IoC.Resolve<QuizResultModel>();
        private CompanyAcServerModel CompanyAcServerModel => IoC.Resolve<CompanyAcServerModel>();
        private QuizModel QuizModel => IoC.Resolve<QuizModel>();
        protected dynamic Settings => IoC.Resolve<ApplicationSettingsProvider>();

        private CompanyEventQuizMappingModel CompanyEventQuizMappingModel => IoC.Resolve<CompanyEventQuizMappingModel>();

        #region Public Methods and Operators

        public EmailHistoryDTO[] GetHistory()
        {
            return this.EmailHistoryModel.GetAll().Select(x => new EmailHistoryDTO(x)).ToArray();
        }

        /// <summary>
        /// The save update.
        /// </summary>
        /// <param name="dto">
        /// The DTO.
        /// </param>
        /// <returns>
        /// The <see cref="EmailHistoryDTO"/>.
        /// </returns>
        public EmailHistoryDTO SaveHistory(EmailHistoryDTO dto)
        {
            ValidationResult validationResult;
            if (this.IsValid(dto, out validationResult))
            {
                if (dto.body != null)
                {
                    var message = dto.body;//.With(x => x.InnerXml);
                    message = Regex.Replace(message, "<[^>]*(>|$)", string.Empty);
                    message = message.Replace("\r\n", "\n").Replace("\r", "\n").Replace("&nbsp;", " ").Replace("&#39;", @"'");
                    message = Regex.Replace(message, @"[ ]{2,}", " ");
                    message = message.Replace("\n ", "\n");
                    message = Regex.Replace(message, @"[\n]{2,}", "\n");
                    message = message.Replace("Enriching online interaction for meetings, training and education on Adobe Connect", string.Empty);
                    message = message.TrimStart("\n".ToCharArray());

                    dto.message = message;
                }

                var isTransient = dto.emailHistoryId == 0;
                var emailHistory = isTransient ? null : EmailHistoryModel.GetOneById(dto.emailHistoryId).Value;
                emailHistory = this.ConvertDto(dto, emailHistory);

                this.EmailHistoryModel.RegisterSave(emailHistory, true);

                return new EmailHistoryDTO(emailHistory);
            }

            var error = this.GenerateValidationError(validationResult);
            this.LogError("EmailHistory.Save", error);
            throw new FaultException<Error>(error, error.errorMessage);
        }

        /// <summary>
        /// The get history by company id.
        /// </summary>
        /// <param name="companyId">
        /// The company id.
        /// </param>
        /// <returns>
        /// The <see cref="EmailHistoryDTO"/>.
        /// </returns>
        public EmailHistoryDTO[] GetHistoryByCompanyId(int companyId)
        {
            var result =
                this.EmailHistoryModel.GetAll()
                    .Where(x => (x != null && x.User != null && x.User.Company != null ? x.User.Company.Id : 0) == companyId)
                    .ToList()
                    .Select(x => new EmailHistoryDTO(x))
                    .ToArray();
            return result;
        }

        /// <summary>
        /// The resend email.
        /// </summary>
        /// <param name="emailHistoryId">
        /// The email history id.
        /// </param>
        /// <returns>
        /// The <see cref="EmailHistoryDTO"/>.
        /// </returns>
        /// <exception cref="FaultException{T}">
        /// Exception if sending failed
        /// </exception>
        public EmailHistoryDTO ResendEmail(int emailHistoryId)
        {
            var item = EmailHistoryModel.GetOneById(emailHistoryId).Value;
            if (item != null)
            {
                var cc = item.SentCc != null
                             ? this.FormEmailList(item.SentCc)
                             : new List<MailAddress>();

                var bcc = item.SentBcc != null
                             ? this.FormEmailList(item.SentBcc)
                             : new List<MailAddress>();

                MailModel.SendEmail(
                    item.SentToName != null ? item.SentToName.Split(';').ToArray() : new string[] { },
                    item.SentTo != null ? item.SentTo.Split(';').ToArray() : new string[] { },
                    item.Subject,
                    item.Body,
                    item.SentFromName,
                    item.SentFrom,
                    null,
                    cc,
                    bcc);

                var newHistoryItem = new EmailHistoryDTO(item);
                newHistoryItem.body = item.Body;
                newHistoryItem.date = DateTime.Now.ConvertToUnixTimestamp();
                newHistoryItem.emailHistoryId = 0;

                return this.SaveHistory(newHistoryItem);
            }

            var error = new Error(Errors.CODE_ERRORTYPE_GENERIC_ERROR, ErrorsTexts.GetResultError_NotFound, "No item with such id found");
            this.LogError("EmailService.Save", error);
            throw new FaultException<Error>(error, error.errorMessage);
        }

        /// <summary>
        /// Logs error to server log.
        /// </summary>
        /// <returns>
        /// The <see cref="NewsletterSubscriptionDTO"/>.
        /// </returns>      
        public NewsletterSubscriptionDTO[] GetNewsletterSubscription()
        {
            return this.NewsletterSubscriptionModel.GetAll().Select(x => new NewsletterSubscriptionDTO(x)).ToArray();
        }

        /// <summary>
        /// The subscribe.
        /// </summary>
        /// <param name="email">
        /// The email.
        /// </param>
        /// <returns>
        /// The <see cref="NewsletterSubscriptionDTO"/>.
        /// </returns>
        public NewsletterSubscriptionDTO SubscribeToNewsletter(string email)
        {
            ValidationResult validationResult;
            var dto = new NewsletterSubscriptionDTO { email = email, dateSubscribed = DateTime.Now.ConvertToUnixTimestamp(), isActive = true };
            if (this.IsValid(dto, out validationResult))
            {
                var newsletterSubscription = this.ConvertDto(dto, null);
                this.NewsletterSubscriptionModel.RegisterSave(newsletterSubscription, true);
                ////IoC.Resolve<RealTimeNotificationModel>().NotifyClientsAboutChangesInTable<EmailHistory>(NotificationType.Update, emailHistory.Id);

                return dto;
            }

            var error = this.GenerateValidationError(validationResult);
            this.LogError("Email.SubscribeToNewsletter", error);
            throw new FaultException<Error>(error, error.errorMessage);
        }

        /// <summary>
        /// The subscribe.
        /// </summary>
        /// <param name="email">
        /// The email.
        /// </param>
        /// <returns>
        /// The <see cref="UserDTO"/>.
        /// </returns>
        public UserDTO[] SetUserUnsubscribed(string email)
        {
            var users = UserModel.GetAllByEmails(new List<string> { email }).ToList();
            foreach (var user in users)
            {
                user.IsUnsubscribed = true;
                UserModel.RegisterSave(user, true);
            }

            return users.Select(u => new UserDTO(u)).ToArray();
        }

        public OperationResultDto SendCertificate(string postQuizResultGuid)
        {
            Guid guid;
            var canParse = Guid.TryParse(postQuizResultGuid, out guid);
            if (!canParse)
            {
                var error = new Error(Errors.CODE_ERRORTYPE_GENERIC_ERROR, ErrorsTexts.GetResultError_NotFound, "No item with such id found");
                this.LogError("EmailService.SendCertificate", error);
                throw new FaultException<Error>(error, error.errorMessage);
            }
            var postQuizResult = QuizResultModel.GetOneByGuid(guid).Value;
            var emailsNotSend = new List<string>();
            if (string.IsNullOrEmpty(postQuizResult.ACEmail))
            {
                Logger.Warn($"[SendCertificate] Email is empty. quizResultId={postQuizResult.Id}");
                emailsNotSend.Add(postQuizResult.ParticipantName);
            }

            var model = new CertificateEmailModel(Settings)
            {
                CertificateLink = Settings.CertificatesUrl + "/UI/#/?quizResultGuid=" + postQuizResult.Guid,
                ParticipantName = postQuizResult.ParticipantName
            };
            bool sentSuccessfully = MailModel.SendEmailSync(postQuizResult.ParticipantName, postQuizResult.ACEmail,
                        Emails.CertificateSubject,
                        model, Common.AppEmailName, Common.AppEmail);
            if (!sentSuccessfully)
            {
                emailsNotSend.Add(postQuizResult.ACEmail);
            }
            return emailsNotSend.Any() ? OperationResultDto.Error("Not all emails were sent correctly") : OperationResultDto.Success();
        }

        public OperationResultDto SendEventQuizResultEmail(int[] quizResultIds)
        {
            var quizResults = QuizResultModel.GetAllByIds(quizResultIds.ToList());
            var mapping = quizResults.FirstOrDefault()?.EventQuizMapping;
            if (mapping == null)
                return OperationResultDto.Error("There is no event for this result.");
            //            var mapping = CompanyEventQuizMappingModel.GetOneById(quizResults.First().EventQuizMappingId).Value;

            var acDomain = CompanyAcServerModel.GetOneById(mapping.CompanyAcDomain.Id).Value;
            var acUrl = acDomain.AcServer;
            var apiUrl = new Uri(acUrl);

            var scoId = mapping.AcEventScoId;
            var proxy = new AdobeConnectProxy(new AdobeConnectProvider(new ConnectionDetails(apiUrl)), Logger, apiUrl);
            var eventInfo = proxy.GetScoInfo(scoId);
            if (!eventInfo.Success)
                throw new InvalidOperationException("");

            List<string> emailsNotSend = new List<string>();
            foreach (var quizResult in quizResults)
            {
                if (quizResult.Quiz.Id != mapping.PreQuiz.Id)
                {
                    throw new InvalidOperationException("Wrong quiz mapping");
                }
                if (string.IsNullOrEmpty(quizResult.ACEmail))
                {
                    Logger.Warn($"[SendEventQuizResultEmail] Email is empty. quizResultId={quizResult.Id}");
                    emailsNotSend.Add(quizResult.ParticipantName);
                    continue;
                }
                try
                {
                    //todo: create model based on success/fail
                    var model = new EventQuizResultSuccessModel(Settings)
                    {
                        Name = quizResult.ParticipantName,
                        EventName = eventInfo.ScoInfo?.Name ?? scoId,
                        MailSubject = "AC Event Post quiz result",
                        PostQuizUrl = Settings.CertificatesUrl + "/UI/#/?quizResultGuid=" + quizResult.Guid
                        //PostQuizUrl = "https://app.edugamecloud.com"
                    };
                    bool sentSuccessfully = MailModel.SendEmailSync(quizResult.ParticipantName, quizResult.ACEmail,
                        Emails.TrialSubject,
                        model, Common.AppEmailName, Common.AppEmail);
                    if (!sentSuccessfully)
                    {
                        emailsNotSend.Add(quizResult.ACEmail);
                    }
                }
                catch (Exception e)
                {
                    Logger.Error($"[SendEventQuizResultEmail] error.", e);
                    emailsNotSend.Add(quizResult.ACEmail);
                }
            }

            return emailsNotSend.Any() ? OperationResultDto.Error("Not all emails were sent correctly") : OperationResultDto.Success();
        }


        public OperationResultDto SendRegistrationEmail(EventRegistrationDTO registrationInfo)
        {
            var mapping = CompanyEventQuizMappingModel.GetOneById(registrationInfo.eventQuizMappingId).Value;
            if (mapping == null)
                return OperationResultDto.Error("There is no event for this info.");
            var acDomain = CompanyAcServerModel.GetOneById(mapping.CompanyAcDomain.Id).Value;
            var acUrl = acDomain.AcServer;
            var apiUrl = new Uri(acUrl);

            var scoId = mapping.AcEventScoId;
            var proxy = new AdobeConnectProxy(new AdobeConnectProvider(new ConnectionDetails(apiUrl)), Logger, apiUrl);
            var eventInfo = proxy.GetScoInfo(scoId);
            if (!eventInfo.Success)
                throw new InvalidOperationException("");
            List<string> emailsNotSend = new List<string>();
            try
            {
                //todo: create model based on success/fail
                var model = new EventQuizRegistrationModel(Settings)
                {
                    FirstName = registrationInfo.FirstName,
                    LastName = registrationInfo.LastName,
                    EventName = eventInfo.ScoInfo.Name,
                    EventDesc = eventInfo.ScoInfo.Description,
                    EventScoId = eventInfo.ScoInfo.ScoId,
                    EventStartDate = eventInfo.ScoInfo.BeginDate,
                    EventEndDate = eventInfo.ScoInfo.EndDate,
                    MailSubject = Emails.RegistrationSubject,
                    MeetingUrl = acDomain.AcServer.TrimEnd('/') + "/" + eventInfo.ScoInfo.SourceSco.UrlPath.TrimStart('/'),
                    Email = registrationInfo.Email
                };

                var attachments = new List<Attachment>();

                //var duration = model.EventEndDate - model.EventStartDate;

                string description = model.EventDesc;
                //string description = this.TemplateProvider.GetTemplate<EventQuizRegistrationModel>().TransformTemplate(model);

                var e = new Event
                {
                    DtStart = new CalDateTime(model.EventStartDate),
                    DtEnd = new CalDateTime(model.EventEndDate),
                    Summary = $"{model.EventName}",
                    Description = description,
                    Url = new Uri(model.MeetingUrl)
                };

                var calendar = new Calendar();
                calendar.Events.Add(e);

                var serializer = new CalendarSerializer(new SerializationContext());
                var serializedCalendar = serializer.SerializeToString(calendar);
                Logger.Debug(serializedCalendar);

                //var ms = new MemoryStream();
                //using (var writer = new StreamWriter(ms))
                //{
                //    writer.Write(serializedCalendar);
                //    writer.Flush();                    
                //}
                //ms.Position = 0;
                byte[] bytes = new byte[serializedCalendar.Length * sizeof(char)];
                Buffer.BlockCopy(serializedCalendar.ToCharArray(), 0, bytes, 0, bytes.Length);

                var contype = new System.Net.Mime.ContentType("text/calendar");
                contype.Parameters.Add("method", "REQUEST");
                contype.Parameters.Add("name", "EventInformation.ics");
                var calendarItem = new Attachment(new MemoryStream(bytes), contype);
                calendarItem.TransferEncoding = System.Net.Mime.TransferEncoding.Base64;
                attachments.Add(calendarItem);

                var sentSuccessfully = MailModel.SendEmailSync($"{model.FirstName} {model.LastName}", model.Email,
                    Emails.RegistrationSubject,
                    model, Common.AppEmailName, Common.AppEmail, attachments: attachments);
                if (!sentSuccessfully)
                {
                    emailsNotSend.Add(model.Email);
                }
            }
            catch (Exception e)
            {
                Logger.Error($"[SendRegistrationEmail] error.", e);
                emailsNotSend.Add(registrationInfo.Email);
            }


            return emailsNotSend.Any() ? OperationResultDto.Error("Not all emails were sent correctly") : OperationResultDto.Success();
        }

        /// <summary>
        /// The form email list.
        /// </summary>
        /// <param name="emailsList">
        /// The CC list.
        /// </param>
        /// <returns>
        /// The <see cref="List{MailAddress}"/>.
        /// </returns>
        private List<MailAddress> FormEmailList(string emailsList)
        {
            return emailsList.Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                .Where(i => !string.IsNullOrWhiteSpace(i))
                .Select(i => new MailAddress(i, i))
                .ToList();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// The convert DTO.
        /// </summary>
        /// <param name="history">
        /// The result DTO.
        /// </param>
        /// <param name="instance">
        /// The instance.
        /// </param>
        /// <returns>
        /// The <see cref="SubModuleCategory"/>.
        /// </returns>
        private EmailHistory ConvertDto(EmailHistoryDTO history, EmailHistory instance)
        {
            instance = instance ?? new EmailHistory();
            instance.Body = history.body; //.With(x => x.InnerXml);
            instance.Date = history.date.ConvertFromUnixTimeStamp();
            instance.Message = history.message;
            instance.SentBcc = history.sentBcc;
            instance.SentCc = history.sentCc;
            instance.SentFrom = history.sentFrom;
            instance.SentTo = history.sentTo;
            instance.Subject = history.subject;
            instance.User = this.UserModel.GetOneByEmail(history.sentTo).Value;
            instance.Status = history.status;

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
            instance.DateSubscribed = h.dateSubscribed.ConvertFromUnixTimeStamp();
            instance.DateUnsubscribed = h.dateUnsubscribed.ConvertFromUnixTimeStamp();

            return instance;
        }

        #endregion
    }
}