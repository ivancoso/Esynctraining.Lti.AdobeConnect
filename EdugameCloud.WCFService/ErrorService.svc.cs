// ReSharper disable once CheckNamespace
namespace EdugameCloud.WCFService
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.ServiceModel;
    using System.ServiceModel.Activation;

    using EdugameCloud.WCFService.Base;
    using EdugameCloud.WCFService.Contracts;
    using EdugameCloud.WCFService.Mail.Models;
    using Esynctraining.Core.Extensions;

    using Resources;

    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.PerSession,
        IncludeExceptionDetailInFaults = true)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class ErrorService : BaseService, IErrorService
    {
        /// <summary>
        /// The send email about error.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="details">
        /// The details.
        /// </param>
        public void SendEmailAboutError(string message, string details)
        {
            var emails = ((string)(this.Settings.ErrorEmailRecipients ?? string.Empty))
                .Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                .ToList();

            this.MailModel.SendEmail(
                new List<string>(),
                emails,
                Emails.ErrorSubject,
                new ErrorMailModel(this.Settings) { ErrorDetails = details, ErrorMessage = message, MailSubject = Emails.ErrorSubject },
                Common.AppEmailName,
                Common.AppEmail);
        }

        /// <summary>
        /// Logs error to server log.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="details">
        /// The details.
        /// </param>
        public void LogError(string message, string details)
        {
            this.Logger.Error(
                    string.Format(
                        "LogError from Client: Error Message: {0}, Error Details: {1}, Client: {2}",
                        message,
                        details,
                        CurrentUser.With(x => x.Email)));
        }

    }

}