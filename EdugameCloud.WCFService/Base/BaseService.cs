namespace EdugameCloud.WCFService.Base
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Net.Mail;
    using System.ServiceModel;
    using System.Text.RegularExpressions;
    using System.Web;
    using Castle.MicroKernel;
    using EdugameCloud.Core.Business.Models;
    using EdugameCloud.Core.Converters;
    using EdugameCloud.Core.Domain.DTO;
    using EdugameCloud.Core.Domain.Entities;
    using EdugameCloud.WCFService.Mail.Models;
    using Esynctraining.Core.Domain.Entities;
    using Esynctraining.Core.Enums;
    using Esynctraining.Core.Logging;
    using Esynctraining.Core.Providers;
    using Esynctraining.Core.Utils;
    using Esynctraining.Mail;
    using FluentValidation;
    using FluentValidation.Results;
    using Resources;

    /// <summary>
    /// The base service.
    /// </summary>
    public abstract class BaseService
    {
        #region Properties
        
        protected FileModel FileModel => IoC.Resolve<FileModel>();

        protected ISmtpClientEngine MailModel => IoC.Resolve<ISmtpClientEngine>();

        protected ILogger Logger => IoC.Resolve<ILogger>();

        protected dynamic Settings => IoC.Resolve<ApplicationSettingsProvider>();

        protected UserModel UserModel => IoC.Resolve<UserModel>();

        protected UserActivationModel UserActivationModel => IoC.Resolve<UserActivationModel>();

        protected SubModuleItemModel SubModuleItemModel => IoC.Resolve<SubModuleItemModel>();

        protected ACSessionModel ACSessionModel => IoC.Resolve<ACSessionModel>();

        protected ITemplateProvider TemplateProvider => IoC.Resolve<ITemplateProvider>();

        protected EmailHistoryModel EmailHistoryModel => IoC.Resolve<EmailHistoryModel>();

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The is valid.
        /// </summary>
        /// <param name="obj">
        /// The object to check.
        /// </param>
        /// <param name="validationResult">
        /// The validation result.
        /// </param>
        /// <typeparam name="T">
        /// The type of object to check
        /// </typeparam>
        /// <returns>
        /// The validation result <see cref="bool"/>.
        /// </returns>
        protected bool IsValid<T>(T obj, out ValidationResult validationResult)
        {
            validationResult = null;
            try
            {
                validationResult = IoC.Resolve<IValidator<T>>().Validate(obj);
                return validationResult.IsValid;
            }
            catch (ComponentNotFoundException ex)
            {
                Logger.Error("BaseService.IsValid", ex);

                return false;
            }
        }

        /// <summary>
        /// The convert.
        /// </summary>
        /// <param name="transferObject">
        /// The t 1.
        /// </param>
        /// <param name="instance">
        /// The t 2.
        /// </param>
        /// <param name="flush">
        /// The flush.
        /// </param>
        /// <typeparam name="TDataTransferObject">
        /// Data transfer object instance
        /// </typeparam>
        /// <typeparam name="TObjectInstance">
        /// Object instance
        /// </typeparam>
        /// <returns>
        /// The <see cref="TObjectInstance"/> instance.
        /// </returns>
        protected TObjectInstance Convert<TDataTransferObject, TObjectInstance>(TDataTransferObject transferObject, TObjectInstance instance, bool flush = false)
        {
            return IoC.Resolve<BaseConverter<TDataTransferObject, TObjectInstance>>().Convert(transferObject, instance, flush);
        }
        
        /// <summary>
        /// The update cache.
        /// </summary>
        /// <typeparam name="T">
        /// Type of entity
        /// </typeparam>
        /// <param name="expression">
        /// The method expressions.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        protected void UpdateCache<T>(Expression<Action<T>> expression, params object[] args)
        {
        }
        
        protected void SendActivation(User user)
        {
            UserActivationModel model = this.UserActivationModel;
            UserActivation userActivation;
            List<MailAddress> bcced = null;

            if ((userActivation = model.GetLatestByUser(user.Id).Value) == null)
            {
                userActivation = UserActivation.Build(user);
                model.RegisterSave(userActivation);
                bcced = GetBCCed(this.Settings.BCCNewEmail as string);
            }

            SendActivationEmail(user.FirstName, user.Email, user.Company,  userActivation.ActivationCode, bcced);
        }
        
        protected CompanyLicenseStatus GetLicenseStatus(CompanyLicenseDTO licenseVo)
        {
            if (licenseVo.isTrial)
            {
                return CompanyLicenseStatus.Trial;
            }

            if (licenseVo.isEnterprise)
            {
                return CompanyLicenseStatus.Enterprise;
            }

            return CompanyLicenseStatus.Pro;
        }
        
        protected PagedRecentReportsDTO GetBaseRecentSplashScreenReports(int userId, int pageIndex, int pageSize)
        {
            int totalCount;
            return new PagedRecentReportsDTO
            {
                objects = this.SubModuleItemModel.GetRecentSplashScreenReportsPaged(userId, pageIndex, pageSize, out totalCount).ToArray(),
                totalCount = totalCount
            };
        }
        
        protected PagedReportsDTO GetBaseSplashScreenReports(int userId, int pageIndex, int pageSize)
        {
            int totalCount;
            return new PagedReportsDTO
            {
                objects = this.ACSessionModel.GetSplashScreenReportsPaged(userId, pageIndex, pageSize, out totalCount).ToArray(),
                totalCount = totalCount
            };
        }
        
        protected void SendActivationLinkEmail(string firstName, string email, string activationCode)
        {
            var model = new ActivationLinkModel(this.Settings)
            {
                FirstName = firstName,
                ActivationCode = activationCode,
                TrialContactEmail = this.Settings.TrialContactEmail,
            };

            bool sentSuccessfully = this.MailModel.SendEmailSync(
                firstName,
                email,
                Emails.ChangePasswordSubject,
                model,
                Common.AppEmailName,
                Common.AppEmail);

            this.SaveHistory(
                sentSuccessfully,
                firstName,
                email,
                Emails.ChangePasswordSubject,
                model,
                Common.AppEmailName,
                Common.AppEmail);
        }
        
        private void SendActivationEmail(string firstName, string email, Company company, string activationCode, List<MailAddress> bcced = null)
        {
            var cced = GetCCed(email, company);
            var blindCopies = new List<MailAddress>();

            if (bcced != null)
            {
                blindCopies.AddRange(bcced);
            }

            var bccedProduction = HttpContext.Current != null && !HttpContext.Current.IsDebuggingEnabled ? GetBCCed(this.Settings.BCCActivationEmail as string) : null;
            if (bccedProduction != null)
            {
                blindCopies.AddRange(bccedProduction);
            }

            var model = new ActivationInvitationModel(this.Settings)
            {
                FirstName = firstName,
                UserName = email,
                ActivationCode = activationCode,
                TrialContactEmail = this.Settings.TrialContactEmail,
                CompanyName = company.CompanyName,
            };

            bool sentSuccessfully = this.MailModel.SendEmailSync(
                firstName,
                email,
                Emails.ActivationSubject,
                model,
                Common.AppEmailName,
                Common.AppEmail, 
                cced, 
                blindCopies);

            this.SaveHistory(
                sentSuccessfully,
                firstName,
                email,
                Emails.ActivationSubject,
                model,
                Common.AppEmailName,
                Common.AppEmail, 
                cced, 
                blindCopies);
        }

        #endregion

        #region Methods
        
        protected void LogError(string methodName, Error error, string userName = null)
        {
            this.Logger.Error(
                string.Format(
                    "{4}. Error result for user: {0}, Error code: {1}, Error Message: {2}, Error Details: {3}",
                    userName ?? "Anonymous",
                    error.errorCode,
                    error.errorMessage,
                    error.errorDetail,
                    methodName));
        }
        
        protected Error GenerateValidationError(ValidationResult validationResult)
        {
            if (validationResult != null)
            {
                foreach (ValidationFailure failure in validationResult.Errors)
                {
                    if (!string.IsNullOrWhiteSpace(failure.ErrorMessage) && failure.ErrorMessage.Contains("#_#"))
                    {
                        int errorCode;
                        var errorDetails = failure.ErrorMessage.Split(new[] { "#_#" }, StringSplitOptions.RemoveEmptyEntries);
                        if (errorDetails.FirstOrDefault() == null || !int.TryParse(errorDetails.FirstOrDefault(), out errorCode))
                        {
                            errorCode = Errors.CODE_ERRORTYPE_GENERIC_ERROR;
                        }

                        return new Error { errorCode = errorCode, errorMessage = errorDetails.ElementAtOrDefault(1) };
                    }

                    return new Error { errorCode = Errors.CODE_ERRORTYPE_GENERIC_ERROR, errorMessage = failure.ErrorMessage };
                }
            }

            return new Error { errorCode = Errors.CODE_ERRORTYPE_GENERIC_ERROR, errorMessage = "There were validation errors." };
        }
        
        protected List<string> UpdateResultToString(ValidationResult validationResult)
        {
            var list = new List<string>();
            if (validationResult != null)
            {
                list.AddRange(from failure in validationResult.Errors where !string.IsNullOrWhiteSpace(failure.ErrorMessage) select failure.ErrorMessage);
            }

            return list;
        }

        protected List<string> UpdateResultToShortString(ValidationResult validationResult)
        {
            var result = new List<string>();
            var list = UpdateResultToString(validationResult);

            foreach (var item in list)
            {
                var errorDetails = item.Split(new[] { "#_#" }, StringSplitOptions.RemoveEmptyEntries);
                result.Add(errorDetails.ElementAtOrDefault(1));
            }

            return result;
        }

        protected void SaveHistory<TModel>(
            bool mailWasSentSuccessfully,
            string toName, string toEmail, 
            string subject,
            TModel model, 
            string fromName = null, string fromEmail = null,
            List<MailAddress> cced = null, 
            List<MailAddress> bcced = null)
        {
            string body = this.TemplateProvider.GetTemplate<TModel>().TransformTemplate(model), message = body;
            if (message != null)
            {
                message = Regex.Replace(message, "<[^>]*(>|$)", string.Empty);
                message = message.Replace("\r\n", "\n").Replace("\r", "\n").Replace("&nbsp;", " ").Replace("&#39;", @"'");
                message = Regex.Replace(message, @"[ ]{2,}", " ");
                message = message.Replace("\n ", "\n");
                message = Regex.Replace(message, @"[\n]{2,}", "\n");
                message = message.Replace("Enriching online interaction for meetings, training and education on Adobe Connect", string.Empty);
                message = message.TrimStart("\n".ToCharArray());
            }

            var emailHistory = new EmailHistory
            {
                SentTo = toEmail,
                SentToName = toName,
                SentFrom = fromEmail,
                SentFromName = fromName,
                Date = DateTime.Now,
                SentBcc =
                    bcced != null
                        ? bcced.Select(ma => ma.Address)
                    .Aggregate((a1, a2) => a1 + ";" + a2)
                        : null,
                SentCc =
                    cced != null
                        ? cced.Select(ma => ma.Address)
                    .Aggregate((a1, a2) => a1 + ";" + a2)
                        : null,
                Subject = subject,
                User = UserModel.GetOneByEmail(toEmail).Value,
                Body = body,
                Message = message,
                Status = mailWasSentSuccessfully ? EmailHistory.StatusSent : EmailHistory.StatusFailed,
            };

            this.EmailHistoryModel.RegisterSave(emailHistory, true);

            if (!mailWasSentSuccessfully)
            {
                Logger.ErrorFormat("[BaseService.SaveHistory] '{0}' mail to '{1}' has not been sent.", subject, toEmail);

                var error = new Error { errorCode = 201, errorMessage = "Email has not been sent." };
                throw new FaultException<Error>(error, error.errorMessage);
            }
        }
        
        private static List<MailAddress> GetCCed(string email, Company company)
        {
            if (company != null && company.PrimaryContact != null && !email.Equals(company.PrimaryContact.Email, StringComparison.InvariantCultureIgnoreCase))
            {
                return new List<MailAddress>
                {
                    new MailAddress(company.PrimaryContact.Email, company.PrimaryContact.FullName)
                };
            }

            return null;
        }
        
        private static List<MailAddress> GetBCCed(string emails)
        {
            if (string.IsNullOrEmpty(emails))
            {
                return null;
            }

            var blindSeparator = new[] { ";" };
            var blindEmails = emails.Split(blindSeparator, StringSplitOptions.RemoveEmptyEntries);

            return blindEmails.Any() ? blindEmails.Select(email => new MailAddress(email)).ToList() : null;
        }

        #endregion

    }

}