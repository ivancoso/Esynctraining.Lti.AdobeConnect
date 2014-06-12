﻿// ReSharper disable once CheckNamespace
namespace EdugameCloud.WCFService
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.ServiceModel;
    using System.ServiceModel.Activation;

    using EdugameCloud.Core.Contracts;
    using EdugameCloud.WCFService.Base;
    using EdugameCloud.WCFService.Mail.Models;

    using Esynctraining.Core.Domain.Contracts;
    using Esynctraining.Core.Enums;
    using Esynctraining.Core.Extensions;

    using Resources;

    /// <summary>
    ///     The account service.
    /// </summary>
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.PerSession,
        IncludeExceptionDetailInFaults = true)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class ErrorService : BaseService, IErrorService
    {
        #region Properties

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The send email about error.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="details">
        /// The details.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse SendEmailAboutError(string message, string details)
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

            return new ServiceResponse();
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
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse LogError(string message, string details)
        {
            this.logger.Error(
                    string.Format(
                        "LogError from Client: Error Message: {0}, Error Details: {1}, Client: {2}",
                        message,
                        details,
                        CurrentUser.With(x => x.Email)));
            return new ServiceResponse { status = Errors.CODE_RESULTTYPE_SUCCESS };
        }

        #endregion
    }
}