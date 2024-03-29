﻿// ReSharper disable once CheckNamespace

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Text.RegularExpressions;
using EdugameCloud.Core.Business.Models;
using EdugameCloud.Core.Domain.DTO;
using EdugameCloud.Core.Domain.Entities;
using EdugameCloud.Lti.Core.Utils;
using EdugameCloud.WCFService.Base;
using EdugameCloud.WCFService.Contracts;
using EdugameCloud.WCFService.Mail.Models;
using Esynctraining.AC.Provider;
using Esynctraining.AC.Provider.DataObjects;
using Esynctraining.AdobeConnect;
using Esynctraining.Core.Domain.Entities;
using Esynctraining.Core.Enums;
using Esynctraining.Core.Extensions;
using Esynctraining.Core.Utils;
using FluentValidation.Results;
using Ical.Net;
using Ical.Net.DataTypes;
using Ical.Net.Serialization;
using Ical.Net.Serialization.iCalendar.Serializers;
using Resources;
using Calendar = Ical.Net.Calendar;

namespace EdugameCloud.WCFService
{
    /// <summary>
    /// The email service.
    /// </summary>
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.PerSession,
        IncludeExceptionDetailInFaults = true)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    // SG: GetHistory / GetHistoryByCompanyId / ResendEmail / SendEventQuizResultEmail
    public class EmailService : BaseService, IEmailService
    {
        private NewsletterSubscriptionModel NewsletterSubscriptionModel => IoC.Resolve<NewsletterSubscriptionModel>();

        private QuizResultModel QuizResultModel => IoC.Resolve<QuizResultModel>();

        private CompanyAcServerModel CompanyAcServerModel => IoC.Resolve<CompanyAcServerModel>();

        private CompanyEventQuizMappingModel CompanyEventQuizMappingModel => IoC.Resolve<CompanyEventQuizMappingModel>();

        #region Public Methods and Operators

        public EmailHistoryDTO[] GetHistory()
        {
            return this.EmailHistoryModel.GetAll().Select(x => new EmailHistoryDTO(x)).ToArray();
        }

        public EmailHistoryDTO SaveHistory(EmailHistoryDTO dto)
        {
            ValidationResult validationResult;
            if (this.IsValid(dto, out validationResult))
            {
                if (dto.body != null)
                {
                    var message = dto.body;
                    message = Regex.Replace(message, "<[^>]*(>|$)", string.Empty);
                    message = message.Replace("\r\n", "\n").Replace("\r", "\n").Replace("&nbsp;", " ").Replace("&#39;", @"'");
                    message = Regex.Replace(message, @"[ ]{2,}", " ");
                    message = message.Replace("\n ", "\n");
                    message = Regex.Replace(message, @"[\n]{2,}", "\n");
                    message = message.Replace("Enriching online interaction for meetings, training and education on Adobe Connect", string.Empty);
                    message = message.TrimStart("\n".ToCharArray());

                    dto.message = message;
                }

                var emailHistory = ConvertDto(dto);

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
                             ? FormEmailList(item.SentCc)
                             : new List<MailAddress>();

                var bcc = item.SentBcc != null
                             ? FormEmailList(item.SentBcc)
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

                var newHistoryItem = new EmailHistoryDTO(item)
                {
                    body = item.Body,
                    date = DateTime.Now.ConvertToUnixTimestamp(),
                    emailHistoryId = 0
                };
                return this.SaveHistory(newHistoryItem);
            }

            var error = new Error(Errors.CODE_ERRORTYPE_GENERIC_ERROR, ErrorsTexts.GetResultError_NotFound, "No item with such id found");
            this.LogError("EmailService.Save", error);
            throw new FaultException<Error>(error, error.errorMessage);
        }

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
                var newsletterSubscription = ConvertDto(dto);
                this.NewsletterSubscriptionModel.RegisterSave(newsletterSubscription, true);
                ////IoC.Resolve<RealTimeNotificationModel>().NotifyClientsAboutChangesInTable<EmailHistory>(NotificationType.Update, emailHistory.Id);

                return dto;
            }

            var error = this.GenerateValidationError(validationResult);
            this.LogError("Email.SubscribeToNewsletter", error);
            throw new FaultException<Error>(error, error.errorMessage);
        }

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

        // TRICK: used by OfflineQuizService.SendAnswers
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

            var acDomain = CompanyAcServerModel.GetOneById(postQuizResult.EventQuizMapping.CompanyAcDomain.Id).Value;
            var acUrl = acDomain.AcServer;
            var apiUrl = new Uri(acUrl);

            var scoId = postQuizResult.EventQuizMapping.AcEventScoId;
            var proxy = new AdobeConnectProxy(new AdobeConnectProvider(new ConnectionDetails(apiUrl)), Logger, apiUrl);
            var eventInfo = proxy.GetEventInfo(scoId);
            if (!eventInfo.Success)
                throw new InvalidOperationException();


            var emailsNotSend = new List<string>();
            if (string.IsNullOrEmpty(postQuizResult.ACEmail))
            {
                Logger.Warn($"[SendCertificate] Email is empty. quizResultId={postQuizResult.Id}");
                emailsNotSend.Add(postQuizResult.ParticipantName);
            }

            var model = new CertificateEmailModel(Settings)
            {
                CertificateLink = postQuizResult.BuildDownloadUrl(Settings.CertificatesUrl),
                ParticipantName = postQuizResult.ParticipantName,
                EventName = eventInfo.EventInfo.Name
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

        //public OperationResultDto SendCertificateToTeacher(string postQuizResultGuid)
        //{
        //    Guid guid;
        //    var canParse = Guid.TryParse(postQuizResultGuid, out guid);
        //    if (!canParse)
        //    {
        //        var error = new Error(Errors.CODE_ERRORTYPE_GENERIC_ERROR, ErrorsTexts.GetResultError_NotFound, "No item with such id found");
        //        this.LogError("EmailService.SendCertificateToTeacher", error);
        //        throw new FaultException<Error>(error, error.errorMessage);
        //    }
        //    var postQuizResult = QuizResultModel.GetOneByGuid(guid).Value;
        //    var emailsNotSend = new List<string>();
        //    if (string.IsNullOrEmpty(postQuizResult.ACEmail))
        //    {
        //        Logger.Warn($"[SendCertificateToTeacher] Email is empty. quizResultId={postQuizResult.Id}");
        //        emailsNotSend.Add(postQuizResult.ParticipantName);
        //    }

        //    var eventMapping = CompanyEventQuizMappingModel.GetOneById(postQuizResult.EventQuizMapping.Id).Value;
        //    var acUrl = eventMapping.CompanyAcDomain.AcServer;
        //    var login = eventMapping.CompanyAcDomain.Username;
        //    var password = eventMapping.CompanyAcDomain.Password;
        //    var adobeConnectProxy = AdobeConnectAccountService.GetProvider(new AdobeConnectAccess(new Uri(acUrl), login, password), false);

        //    var loginResult = adobeConnectProxy.Login(new UserCredentials(login, password));
        //    if (!loginResult.Success)
        //        throw new InvalidOperationException("Can't login to AC");

        //    var dynamicQuestionAnswers = GetDynamicQuestionAnswers(acUrl, eventMapping.AcEventScoId,
        //        loginResult.Status.SessionInfo, postQuizResult.ACEmail);
        //    var schoolNumber = dynamicQuestionAnswers["school"];
        //    var school = SchoolModel.GetOneByName(schoolNumber);
        //    var schoolEmail = school.SchoolEmail;
        //    if (string.IsNullOrEmpty(schoolEmail))
        //    {
        //        Logger.Warn($"[SendCertificateToTeacher] Teacher Email is empty. quizResultId={postQuizResult.Id}");
        //        emailsNotSend.Add(schoolEmail);
        //    }

        //    var schoolAccountName = school.AccountName;
        //    var model = new TeacherCertificateEmailModel(Settings)
        //    {
        //        CertificateLink =
        //        $"{Settings.CertificatesUrl}/quiz-certificate/{postQuizResult.Guid}/download",
        //        ParticipantName = postQuizResult.ParticipantName,
        //        TeacherName = schoolAccountName
        //    };
        //    bool sentSuccessfully = MailModel.SendEmailSync(schoolAccountName, schoolEmail,
        //                Emails.CertificateSubject,
        //                model, Common.AppEmailName, Common.AppEmail);
        //    if (!sentSuccessfully)
        //    {
        //        emailsNotSend.Add(schoolEmail);
        //    }
        //    return emailsNotSend.Any() ? OperationResultDto.Error("Not all emails were sent correctly") : OperationResultDto.Success();
        //}

        //private static Dictionary<string, string> GetDynamicQuestionAnswers(string acUrl, string scoId, string breezeSession, string userEmail)
        //{
        //    var userStateSchoolAnswersUrl =
        //        $"{acUrl}/api/xml?action=report-event-participants-complete-information&sco-id={scoId}&session={breezeSession}";

        //    var reply = string.Empty;

        //    using (var client = new HttpClient())
        //    {
        //        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/xml"));
        //        var request = new HttpRequestMessage(HttpMethod.Get, userStateSchoolAnswersUrl);
        //        reply = client.SendAsync(request).Result.Content.ReadAsStringAsync().Result;
        //    }

        //    var doc = XDocument.Parse(reply);
        //    var questions = doc.Root?.Descendants("registration_questions").Descendants("question").ToList();
        //    var stateNode =
        //        questions?.FirstOrDefault(
        //            x =>
        //                x.Attribute("description")
        //                    .Value.ToString()
        //                    .ToLower()
        //                    .Equals("state", StringComparison.OrdinalIgnoreCase));
        //    var schoolNode =
        //        questions?.FirstOrDefault(
        //            x =>
        //                x.Attribute("description")
        //                    .Value.ToString()
        //                    .ToLower()
        //                    .Equals("school", StringComparison.OrdinalIgnoreCase));

        //    var stateQuestionId = stateNode?.Attribute("id")?.Value.ToString() ?? string.Empty;
        //    var schoolQuestionId = schoolNode?.Attribute("id")?.Value.ToString() ?? string.Empty;

        //    var userAnswers = doc.Root?.Descendants("user_list").Descendants("user").ToList();
        //    var userAnswer =
        //        userAnswers?.FirstOrDefault(
        //            x =>
        //                x.Attribute("login")
        //                    .Value.ToString()
        //                    .ToLower()
        //                    .Equals(userEmail, StringComparison.OrdinalIgnoreCase));
        //    var stateAnswer = userAnswer?.Attribute("registration_question_" + stateQuestionId)?.Value ?? String.Empty;
        //    var schoolAnswer = userAnswer?.Attribute("registration_question_" + schoolQuestionId)?.Value ?? String.Empty;
        //    var result = new Dictionary<string, string>() { { "state", stateAnswer }, { "school", schoolAnswer } };
        //    return result;
        //}

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
            var eventInfo = proxy.GetEventInfo(scoId);
            if (!eventInfo.Success)
                throw new InvalidOperationException();

            var emailsNotSend = new List<string>();
            foreach (var quizResult in quizResults)
            {
                if (quizResult.Quiz.Id != mapping.PreQuiz.Id)
                {
                    throw new InvalidOperationException("Wrong quiz mapping");
                }
                if (string.IsNullOrEmpty(quizResult.ACEmail) && string.IsNullOrEmpty(quizResult.Email))
                {
                    Logger.Warn($"[SendEventQuizResultEmail] Email is empty. quizResultId={quizResult.Id}");
                    emailsNotSend.Add(quizResult.ParticipantName);
                    continue;
                }

                string email = null;
                try
                {
                    //todo: create model based on success/fail
                    var model = new EventQuizResultSuccessModel(Settings)
                    {
                        Name = quizResult.ParticipantName,
                        EventName = eventInfo.EventInfo?.Name ?? scoId,
                        MailSubject = "AC Event Post quiz result",
                        PostQuizUrl = Settings.CertificatesUrl + "/UI/#/?quizResultGuid=" + quizResult.Guid,
                        Date = quizResult.EndTime
                        //PostQuizUrl = "https://app.edugamecloud.com"
                    };

                    email = !string.IsNullOrEmpty(quizResult.ACEmail) ? quizResult.ACEmail : quizResult.Email;
                    bool sentSuccessfully = MailModel.SendEmailSync(quizResult.ParticipantName, email,
                        Emails.GoddardPostQuizSubject,
                        model, Common.AppEmailName, Common.AppEmail);
                    if (!sentSuccessfully)
                    {
                        emailsNotSend.Add(email);
                    }
                }
                catch (Exception e)
                {
                    Logger.Error($"[SendEventQuizResultEmail] error.", e);
                    emailsNotSend.Add(email);
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
            var apiUrl = new Uri(acDomain.AcServer);
            var proxy = new AdobeConnectProxy(new AdobeConnectProvider(new ConnectionDetails(apiUrl)), Logger, apiUrl);

            var loginResult = proxy.Login(new UserCredentials(acDomain.Username, acDomain.Password));
            if (!loginResult.Success)
                throw new InvalidOperationException($"Can't login to AC url {acDomain.AcServer} user {acDomain.Username}");

            var eventInfo = proxy.GetScoInfo(mapping.AcEventScoId);
            if (!eventInfo.Success)
                throw new InvalidOperationException(eventInfo.Status.GetErrorInfo());

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
                    EventStartDate = DateTimeHelper.ConvertToEST(eventInfo.ScoInfo.BeginDate).Value,
                    EventEndDate = DateTimeHelper.ConvertToEST(eventInfo.ScoInfo.EndDate).Value,
                    MailSubject = Emails.RegistrationSubject,
                    MeetingUrl = acDomain.AcServer.TrimEnd('/') + "/" + eventInfo.ScoInfo.SourceSco.UrlPath.TrimStart('/'),
                    Email = registrationInfo.Email
                };

                var attachments = new List<System.Net.Mail.Attachment>();

                var e = new Event
                {
                    //DtStart = new CalDateTime(DateTimeHelper.ConvertToEST(model.EventStartDate).Value),
                    //DtEnd = new CalDateTime(DateTimeHelper.ConvertToEST(model.EventEndDate).Value),
                    DtStart = new CalDateTime(eventInfo.ScoInfo.BeginDate),
                    DtEnd = new CalDateTime(eventInfo.ScoInfo.EndDate),
                    Summary = $"{model.EventName}",
                    Description = model.MeetingUrl,
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
                var calendarItem = new System.Net.Mail.Attachment(new MemoryStream(bytes), contype);
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

        #endregion

        #region Private Methods

        private static List<MailAddress> FormEmailList(string emailsList)
        {
            return emailsList.Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                .Where(i => !string.IsNullOrWhiteSpace(i))
                .Select(i => new MailAddress(i, i))
                .ToList();
        }

        private EmailHistory ConvertDto(EmailHistoryDTO history)
        {
            return new EmailHistory
            {
                Body = history.body,
                Date = history.date.ConvertFromUnixTimeStamp(),
                Message = history.message,
                SentBcc = history.sentBcc,
                SentCc = history.sentCc,
                SentFrom = history.sentFrom,
                SentTo = history.sentTo,
                Subject = history.subject,
                User = this.UserModel.GetOneByEmail(history.sentTo).Value,
                Status = history.status,
            };
        }
        
        private NewsletterSubscription ConvertDto(NewsletterSubscriptionDTO h)
        {
            return new NewsletterSubscription
            {
                Email = h.email,
                IsActive = h.isActive,
                DateSubscribed = h.dateSubscribed.ConvertFromUnixTimeStamp(),
                DateUnsubscribed = h.dateUnsubscribed.ConvertFromUnixTimeStamp(),
            };
        }

        #endregion

    }

}