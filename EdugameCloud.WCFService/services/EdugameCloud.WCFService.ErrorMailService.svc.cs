// ReSharper disable once CheckNamespace
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

    using Resources;

    /// <summary>
    ///     The account service.
    /// </summary>
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.PerSession,
        IncludeExceptionDetailInFaults = true)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class ErrorMailService : BaseService, IErrorMailService
    {
        #region Properties


        #endregion

        #region Public Methods and Operators

        public ServiceResponse SendMailAboutError(string message, string details)
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

        #endregion
    }
}